using System.ServiceModel;
using System.Windows;
using OneMessenger.Core;
using static OneMessenger.Core.DataBaseUtils;

namespace OneMessenger.Client
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class ClientCallback:IClient
	{
		public void GetMessage(string username, string message)
		{
			((MainWindow)Application.Current.MainWindow).TakeMessage(username,message);
		}
	}
}