using OneMessenger.Core;

namespace OneMessenger.Server
{
	public class ConnectedClient
	{
		public string Username { get; set; }
		public IClient Connection;
		public bool NeedsCensoring { get; set; }
	}
}