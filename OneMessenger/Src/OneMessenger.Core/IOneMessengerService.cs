using System.Collections.Generic;
using System.ServiceModel;

namespace OneMessenger.Core
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOneMessengerService" in both code and config file together.
	[ServiceContract(CallbackContract=typeof(IClient))]
	public interface IOneMessengerService
	{
		[OperationContract]
		int Login(string username, string hashedpassword);

        [OperationContract]
        int Registration(string username, string password);

        [OperationContract]
		void SendMessageToAll(string username,string message);
		[OperationContract]
		List<string> GetConnectedUsernames(string username);

    }
}
