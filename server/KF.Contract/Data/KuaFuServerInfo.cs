using System;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class KuaFuServerInfo
	{
		
		public int ServerId;

		
		public string Ip;

		
		public int Port;

		
		public string DbIp;

		
		public int DbPort;

		
		public string LogDbIp;

		
		public int LogDbPort;

		
		public int State;

		
		public int Flags;

		
		public int Age;

		
		public int Load;
	}
}
