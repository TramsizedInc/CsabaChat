using System;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OneMessenger.Core;
using static OneMessenger.Core.DataBaseUtils;
using SysQL = MySqlConnector;
using static OneMessenger.Core.Hashish;
using System.ServiceModel.Channels;
using System.Runtime.CompilerServices;
namespace OneMessenger.Server{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,InstanceContextMode = InstanceContextMode.Single)]
	public class OneMessengerService : IOneMessengerService{
		public ConcurrentDictionary<string, OneMessenger.Core.ConnectedClient> ConnectedClients=new ConcurrentDictionary<string, OneMessenger.Core.ConnectedClient>();
        public DataBaseUtils.ConnectSQL db = new DataBaseUtils.ConnectSQL("localhost", "root", "", "csaba");
        static OneMessenger.Core.Hashish hashish = new Hashish();
        public int Login(string username, string password){
            if (!this.IsUserExistant(username)){
                // "needs to register";
                return 1;
            }
            foreach (var client in ConnectedClients){
                if (client.Key.ToLower() == username.ToLower()){
                    return 1;// "collides with an already logged-in client"
                }
            }
            var hashed_input = hashish.HashPassword(password);
            if (hashed_input != RetriveHashedPassword(username))
                return 1; //password mismatch
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            OneMessenger.Core.ConnectedClient newClient = new OneMessenger.Core.ConnectedClient();
            newClient.Connection = establishedUserConnection;
            newClient.Username = username;
            newClient.NeedsCensoring = this.GetCensuraInfo(newClient.Username); ///
            ConnectedClients.TryAdd(username, newClient);
            return 0;
        }
        public int Registration(string username, string password){
            //this.db.Connection.Open();
            if (!this.IsUsernameValid(username))
                return 1; // invalid username

            if (this.IsUserExistant(username)){
                // "needs to login";
                return 2;
            }
            var hashed_password = hashish.HashPassword(password);
            this.CreateUser(username, hashed_password);
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            OneMessenger.Core.ConnectedClient newClient = new OneMessenger.Core.ConnectedClient();
            newClient.Connection = establishedUserConnection;
            newClient.Username = username;
            newClient.NeedsCensoring = false; ///
            ConnectedClients.TryAdd(username, newClient);
            return 0;
        }
        public void SendMessageToAll(string username,string message){
            var receivers = new List<string>();
			foreach (var client in ConnectedClients){
				if (client.Key.ToLower() != username.ToLower()){
					client.Value.Connection.GetMessage(username,message);
                    this.MessageLogger(message, username, client.Key);
                    receivers.Add(client.Key);
                }
            }
            this.MessageLogger(message, username, receivers);
		}
        public void SendMessageToUser(string receiver, string sender, string message){
            foreach (var client in ConnectedClients){
                if (client.Key.ToLower() == receiver.ToLower()){
                    client.Value.Connection.GetMessage(sender, message);
                    this.MessageLogger(message, sender, receiver);
                }
            }
        }
        private bool IsUserExistant(string username){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT users.username FROM users WHERE users.username='{username}'",db.Connection);
            var user = GetData(command);
            return user.Count() > 0;
        }
        private string RetriveHashedPassword(string username){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT users.password FROM users WHERE users.username='{username}'", db.Connection);
            var user = GetData(command);

            if (user.Count != 1)
                throw new Exception("more than one password is available for user" + username);
            return user.First().ToString();
        }
        private void CreateUser(string username, string hashed_password){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"Insert into users (username, email, password, created_at) VALUES ('{username}', '{username}.kavcsicsabcsi@gmail.com', '{hashed_password}', '{DateTime.Now}')", db.Connection);
            RunNonQuery(command);
        }
        private void SetCensura(string userid, bool switcheroo){
            using SysQL::MySqlCommand cmd = new SysQL::MySqlCommand($"UPDATE users SET censured = '{Convert.ToInt32(switcheroo).ToString().Last()}' WHERE users.id = '{userid}';");
            RunNonQuery(cmd);
        }
        private void MessageLogger(string message, string sender){
            var recivers = new List<string>();
            foreach (var client in ConnectedClients)
            {
                recivers.Add(client.Key);
            }
            string reciver = string.Join(", ",recivers);
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"Insert into messages (message, sender_id, reciver, created_at) VALUES ('{message}, {GetID(sender)}', '{reciver}', '{DateTime.Now}')", db.Connection);
            RunNonQuery(command);
        }
        private void MessageLogger(string message, string sender, List<string> receivers){
            string reciver = string.Join(", ", receivers);
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"Insert into messages (message, sender_id, reciver, created_at) VALUES ('{message}, {GetID(sender)}', '{reciver}', '{DateTime.Now}')", db.Connection);
            RunNonQuery(command);
        }
        private void MessageLogger(string message, string sender, string reciver){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"Insert into messages (message, sender_id, reciver, created_at) VALUES ('{message}, {GetID(sender)}', '{reciver}', '{DateTime.Now}')", db.Connection);
            RunNonQuery(command);
        }
        private string GetID(string username){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT users.id FROM csaba WHERE users.username='{username}'", db.Connection);
            var user = GetData(command);
            return user.First().ToString();
        }
        private bool IsUsernameValid(string username)
        {
            return !username.Contains(",");
        }
        public List<string> GetConnectedUsernames(string username)
        {
            var others = new List<string>();
            foreach (var client in ConnectedClients)
            {
                if (client.Key.ToLower() != username.ToLower())
                {
                    others.Add(client.Key);
                }
            }
            return others;
        }
        private bool GetCensuraInfo(string username){
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT users.censured FROM csaba WHERE users.username='{username}'", db.Connection);
            var censura = GetData(command);
            return censura.First().ToString().Contains("1");
        }
        public void UploadImage(string username, (string, string, string) img_data){
            using SysQL::MySqlCommand cmd = new SysQL::MySqlCommand($"Insert into images (img_id, uploader_id, img_str, img_ext) VALUES ('{(username + "_" + img_data.Item2 + "_" + img_data.Item3 + "_" + DateTime.Now)}', {GetID(username)}', '{img_data.Item1}', '{img_data.Item3}')", db.Connection);
            RunNonQuery(cmd);
        }
        public ConcurrentDictionary<string, OneMessenger.Core.ConnectedClient> GetConnectedClients() => this.ConnectedClients;
    }
}
