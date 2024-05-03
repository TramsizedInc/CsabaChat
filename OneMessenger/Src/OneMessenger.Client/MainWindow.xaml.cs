 using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using OneMessenger.Core;
using OneMessenger.Server;
using System.IO;
using static OneMessenger.Core.DataBaseUtils;
using SysQL = MySqlConnector;


namespace OneMessenger.Client{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window{
		// ReSharper disable once InconsistentNaming
		public static IOneMessengerService _server;
        public DataBaseUtils.ConnectSQL db = new DataBaseUtils.ConnectSQL("localhost", "root", "", "csaba");
        public MainWindow(){
			InitializeComponent();
			var channelFactory = new DuplexChannelFactory<IOneMessengerService>(new ClientCallback(), "OneMessengerServiceEndpoint");
			try{
				_server = channelFactory.CreateChannel();
			}
			catch (Exception e){
				MessageBox.Show(e.Message,"Sever not Runing");
			}
			TextDisplay.IsReadOnly = true;
			MessageTextBox.Focus();
			ConnectedUsers.IsReadOnly = true;
			BtnUpload.IsEnabled = false;
		}
		public void TakeMessage(string username,string message){
			//message = _server.GetConnectedClients()[username].NeedsCensoring?this.CensorDirtyWords(message) : message;
			var thisclient = _server.GetConnectedClients().Where(x => x.Key == username).First().Value;
			var censurainfo = thisclient.NeedsCensoring;
			message = censurainfo ? this.CensorDirtyWords(message) : message;
			TextDisplay.Text += $"{username} : { message} \n";
		}
		private void BtnSend_Click(object sender, RoutedEventArgs e){
			if (MessageTextBox.Text!=""){
				_server.SendMessageToAll(UserNameTextBox.Text, MessageTextBox.Text);
				TakeMessage("You", MessageTextBox.Text);
				MessageTextBox.Text = "";
			}
		}
		private void btnlogin_Click(object sender, RoutedEventArgs e){
			var value = 0;
			try
			{
				if (_server == null)
					throw new Exception("server was nul");
				value = _server.Login(UserNameTextBox.Text, UserPassTextBox.Text);
			}
			catch(Exception ext) { MessageBox.Show(ext.Message, ext.Source); }
			if (value==1){
				MessageBox.Show("You are already logged in");
			}
			else{
				MessageBox.Show("Successfully logged in");
				LblWelcome.Content = $"Welcome, {UserNameTextBox.Text}";
				UserNameTextBox.IsEnabled = false;
				btnlogin.IsEnabled = false;
				_server.GetConnectedUsernames(UserNameTextBox.Text).ForEach(x => ConnectedUsers.Text += x + "\n");
                BtnUpload.IsEnabled = false;
            }
        }
		public string CensorDirtyWords(string message){
			var dw = new DirtyWord();
			message.Split(' ').ToList().ForEach(x=> dw.GetSafeWord(x));
			return message;
		}
		private (string, string, string) GetFileBin(){
			var openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.svg|All files (*.*)|*.*";
			if (!openfiledialog.ShowDialog() == true)
				throw new Exception("Fuck it, you canceled");
			return (Convert.ToBase64String(File.ReadAllBytes(openfiledialog.FileName)),openfiledialog.SafeFileName.Split('.').First(), openfiledialog.SafeFileName.Split('.').Last());
        }

        private void btnregister_Click(object sender, RoutedEventArgs e)
        {
            var value = _server.Registration(UserNameTextBox.Text, UserPassTextBox.Text);

            if (value == 1){
                MessageBox.Show("You are already logged in");
            }
            if (value == 2){
                MessageBox.Show("You are already logged in");
            }
            else{
                MessageBox.Show("Successfully logged in");
                LblWelcome.Content = $"Welcome, {UserNameTextBox.Text}";
                UserNameTextBox.IsEnabled = false;
                btnlogin.IsEnabled = false;
				btnregister.IsEnabled = false;
                UserPassTextBox.IsEnabled = false;
                BtnUpload.IsEnabled = true;
                _server.GetConnectedUsernames(UserNameTextBox.Text).ForEach(x => ConnectedUsers.Text += x + "\n");
				this.UploadImage(UserNameTextBox.Text, this.GetFileBin());
                using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT images.img_str FROM images WHERE images.uploader_id={this.GetID(UserNameTextBox.Text)}", db.Connection);
				var img = GetData(command)[0];
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e) => _server.UploadImage(UserNameTextBox.Text, this.GetFileBin());

        private void UploadImage(string username, (string, string, string) img_data)
        {
            using SysQL::MySqlCommand cmd = new SysQL::MySqlCommand($"Insert into images (img_id, uploader_id, img_str, img_ext) VALUES ('{(username + "_" + img_data.Item2 + "_" + img_data.Item3 + "_" + DateTime.Now.ToString().Replace(' ', '_'))}', {this.GetID(username)}, '{img_data.Item1}', '{img_data.Item3}')", db.Connection);
            RunNonQuery(cmd);
        }
        private string GetID(string username)
        {
            using SysQL::MySqlCommand command = new SysQL::MySqlCommand($"SELECT users.id FROM users WHERE users.username='{username}'", db.Connection);
            var user = GetData(command);
            return user.First().ToString();
        }
    }
}
