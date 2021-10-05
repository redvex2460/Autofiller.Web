using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofiller.Data.Models
{
    public class DatabaseCommand
    {
        private SqliteConnection _connection;
        private SqliteCommand _cmd;
        private SqliteTransaction _transaction;
        private string _table;

        public object Result { get; set; }
        public DatabaseCommand(SqliteConnection connection)
        {
            _connection = connection;
        }

        public static DatabaseCommand Init(SqliteConnection connection)
        {
            return new DatabaseCommand(connection);
        }

        public DatabaseCommand OpenDatabase()
        {
            _connection.Open();
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

        internal DatabaseCommand SaveTable<T>(List<T> data)
        {
            ClearTable();
            BeginTransaction();

            foreach(var obj in data)
            {
                AddObject(obj);
            }
            return this;
        }

        public DatabaseCommand AddObject<T>(T obj)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach(var property in obj.GetType().GetProperties())
            {
                if(property.Name != "Table")
                    keyValuePairs.Add(property.Name, property.GetValue(obj).ToString());
            }
            var command = _connection.CreateCommand();
            command.CommandText =
                @$"     INSERT INTO {_table}
                        ({string.Join(",", keyValuePairs.Keys.ToList())}) VALUES (${string.Join(",$", keyValuePairs.Keys.ToList())})
                  ";

            foreach(var key in keyValuePairs.Keys)
            {
                var parameter = new SqliteParameter() { ParameterName = key, Value=keyValuePairs[key] };
                command.Parameters.Add(parameter);
            }

            command.ExecuteNonQuery();
            return this;
        }

        public DatabaseCommand SelectTable(string table)
        {
            _table = table;
            return this;
        }
        public DatabaseCommand UpdateValue(string column, string value, string where)
        {
            if(_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }
            var command = _connection.CreateCommand();
            command.CommandText = $"UPDATE {_table} SET {column} = \"{value}\" {where}";
            command.ExecuteNonQuery();
            return this;
        }
        public DatabaseCommand GetObjects<T>(string query)
        {
            var result = new List<T>();
            var command = _connection.CreateCommand();
            command.CommandText = query;
            using (var reader = command.ExecuteReader())
            {
                while(reader.Read())
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
        public DatabaseCommand Execute()
        {
            if(_transaction != null)
            {
                _transaction.Commit();
            }

            _connection.Close();
            return this;
        }
    }
}
