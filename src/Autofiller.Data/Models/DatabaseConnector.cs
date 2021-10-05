using Autofiller.Data.Models.Database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Autofiller.Data.Models
{
    public class DatabaseConnector
    {
        #region Private Fields

        private static DatabaseConnector instance;
        private readonly string _filename;
        private readonly SqliteConnection _sqliteConnection;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseConnector(string filename)
        {
            _sqliteConnection = new SqliteConnection($"Data Source={filename}");
            _filename = filename;
        }

        #endregion Public Constructors

        #region Public Properties

        public static SqliteConnection SqliteConnection => instance._sqliteConnection;

        #endregion Public Properties

        #region Public Methods

        public static void ClearApps()
        {
            instance._sqliteConnection.Open();
            var command = instance._sqliteConnection.CreateCommand();
            command.CommandText = "DELETE FROM Steam_Apps";
            command.ExecuteNonQuery();
            instance._sqliteConnection.Close();
        }

        public static void CreateTableForObject(Type type)
        {
            Console.WriteLine($"Creating Table for Object: {type.Name}");
            List<string> properties = new List<string>();
            foreach (var property in type.GetProperties())
            {
                if (property.Name == "Table")
                    continue;
                if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
                {
                    properties.Add($"{property.Name} INTEGER");
                }
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(SteamPlatforms) || property.PropertyType == typeof(DownloadStatus))
                {
                    properties.Add($"{property.Name} VARCHAR");
                }
            }
            dynamic defaultObject = Activator.CreateInstance(type);
            instance._sqliteConnection.Open();
            var command = instance._sqliteConnection.CreateCommand();
            command.CommandText = $"CREATE TABLE {defaultObject.Table} ({string.Join(",", properties)})";
            command.ExecuteNonQuery();
            instance._sqliteConnection.Close();
        }

        public static void ExecuteQuery(string query)
        {
            instance._sqliteConnection.Open();
            var command = instance._sqliteConnection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
            instance._sqliteConnection.Close();
            return;
        }

        public static List<Dictionary<string, object>> GetRows(string query)
        {
            var result = new List<Dictionary<string, object>>();
            instance._sqliteConnection.Open();
            var command = instance._sqliteConnection.CreateCommand();
            command.CommandText = query;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var rowEntry = new Dictionary<string, object>();
                    for (int i = 0; i != reader.FieldCount; i++)
                    {
                        rowEntry.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    result.Add(rowEntry);
                }
            }
            instance._sqliteConnection.Close();
            return result;
        }

        public static void Initialise(string filename, DataManager dataManager)
        {
            Console.WriteLine("Initialising Database connection..");
            var newDatabase = !File.Exists(filename);
            instance = new DatabaseConnector(filename);
            if (newDatabase)
            {
                Console.WriteLine("Looks like were creating a new Database");
                CreateTableForObject(typeof(Settings));
                CreateTableForObject(typeof(QueuedApp));
                CreateTableForObject(typeof(AuthorisedUser));
                CreateTableForObject(typeof(App));
                Console.WriteLine("Getting AppList From Steam");
                dataManager.SteamWrapper.GetApps(true);
                Console.WriteLine("Done..");
            }
            dataManager.Apps.LoadFromDatabase();
            dataManager.Settings.LoadFromDatabase();
            dataManager.Queue.LoadFromDatabase();
            dataManager.AuthorisedUsers.LoadFromDatabase();
            Console.WriteLine("Done..");
        }

        public static DatabaseCommand Command()
        {
            return DatabaseCommand.Init(SqliteConnection);
        }

        #endregion Public Methods
    }
}