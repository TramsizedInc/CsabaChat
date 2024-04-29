using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysQL = MySqlConnector;

namespace OneMessenger.Core
{
    public class DataBaseUtils
    {
        public class ConnectSQL
        {
            private string _location;
            public string _username;
            private string _password;
            public string _database;
            private SysQL::MySqlConnection _connection;

            public string Location
            {
                get => _location;
                private set => value = _location;
            }
            public string Username
            {
                get => _username;
                private set => value = _username;
            }
            public string Password
            {
                get => _password;
                private set => value = _password;
            }
            public string DataBase
            {
                get => _database;
                private set => value = _database;
            }
            public SysQL::MySqlConnection Connection
            {
                get => _connection;
                private set => value = _connection;
            }

            public ConnectSQL(string location, string username, string password, string database)
            {
                this._location = location;
                this._password = password;
                this._username = username;
                this._database = database;
                this._connection = _InternalConnect();
            }

            private SysQL::MySqlConnection _InternalConnect() => new SysQL::MySqlConnection($"Server={Location};User={Username};Password={Password};Database={DataBase};");
        }
        public static List<object> GetData(SysQL::MySqlCommand cmd){
            cmd.Connection.Open();
            var datas = new List<object>();
            using (var reader = cmd.ExecuteReader()){
                while (reader.Read()){
                    datas.Add(reader.GetValue(0));
                }
            }
            cmd.Connection.Close();
            return datas;
        }
        public static List<object> GetDatas(SysQL::MySqlCommand cmd){
            cmd.Connection.Open();
            var datas = new List<object>();
            using (var reader = cmd.ExecuteReader()){
                while (reader.Read()){
                    datas.Add(reader.GetValue(0));
                }
            }
            cmd.Connection.Close();
            return datas;
        }
        public static void RunNonQuery(SysQL::MySqlCommand cmd) {
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }
}

