using System;
using System.Runtime.Remoting.Messaging;

namespace KF.Contract
{
	
	[Serializable]
	public class KuaFuClientContext : ILogicalThreadAffinative
	{
		
		public int ServerId;

		
		public int ClientId;

		
		public int GameType;

		
		public string Token;
	}
}
