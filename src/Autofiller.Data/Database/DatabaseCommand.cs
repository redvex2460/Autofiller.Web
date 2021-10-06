using Autofiller.Data.Database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofiller.Data.Database
{
    public class DatabaseCommand
    {
        #region Private Fields

        private readonly SqliteConnection _connection;
        private string _table;
        private SqliteTransaction _transaction;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseCommand(SqliteConnection connection)
        {
            _connection = connection;
        }

        #endregion Public Constructors

        #region Public Properties

        public object Result { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static DatabaseCommand Init(SqliteConnection connection)
        {
            return new DatabaseCommand(connection);
        }

        public DatabaseCommand AddObject<T>(T obj)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.Name != "Table")
                    keyValuePairs.Add(property.Name, property.GetValue(obj).ToString());
            }
            var command = _connection.CreateCommand();
            command.CommandText =
                @$"     INSERT INTO {_table}
                        ({string.Join(",", keyValuePairs.Keys.ToList())}) VALUES (${string.Join(",$", keyValuePairs.Keys.ToList())})
                  ";

            foreach (var key in keyValuePairs.Keys)
            {
                var parameter = new SqliteParameter() { ParameterName = key, Value = keyValuePairs[key] };
                command.Parameters.Add(parameter);
            }

            command.ExecuteNonQuery();
            return this;
        }

        public DatabaseCommand BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
            return this;
        }

        public DatabaseCommand ClearTable()
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"DELETE FROM {_table}";
            command.ExecuteNonQuery();
            return this;
        }

        public DatabaseCommand Execute()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }

            _connection.Close();
            return this;
        }

        public DatabaseCommand GetObjects<T>(string query)
        {
            var result = new List<T>();
            var command = _connection.CreateCommand();
            command.CommandText = query;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var newObject = Activator.CreateInstance<T>();
                    foreach (var property in newObject.GetType().GetProperties())
                    {
                        if (property.Name.ToLower() == "table")
                            continue;
                        property.SetValue(newObject, reader[property.Name]);
                    }
                    result.Add(newObject);
                }
            }
            Result = result;
            return this;
        }

        public DatabaseCommand OpenDatabase()
        {
            _connection.Open();
            return this;
        }
        public DatabaseCommand SelectTable(string table)
        {
            _table = table;
            return this;
        }

        public DatabaseCommand UpdateValue(string column, string value, string where)
        {
            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }
            var command = _connection.CreateCommand();
            command.CommandText = $"UPDATE {_table} SET {column} = \"{value}\" {where}";
            command.ExecuteNonQuery();
            return this;
        }

        #endregion Public Methods

        #region Internal Methods

        internal DatabaseCommand SaveTable<T>(List<T> data)
        {
            ClearTable();
            BeginTransaction();


            var properties = new List<string>();
            typeof(T).GetProperties().ToList().ForEach(property => { if(property.Name != "Table") properties.Add(property.Name); });
            var command = _connection.CreateCommand();
            command.CommandText =
               @$"     INSERT INTO {_table}
                        ({string.Join(",", properties)}) VALUES (${string.Join(",$", properties)})
                  ";

            foreach(var parameter in properties)
            {
                command.Parameters.AddWithValue($"${parameter}", "");
            }

            foreach (var obj in data)
            {
                foreach (var parameter in properties)
                {
                    command.Parameters[$"${parameter}"].Value = obj.GetType().GetProperty(parameter).GetValue(obj);
                }
                command.ExecuteNonQuery();
            }
            return this;
        }

        #endregion Internal Methods
    }
}