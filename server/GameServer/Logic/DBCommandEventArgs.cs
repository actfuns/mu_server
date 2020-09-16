using System;
using GameServer.Server;

namespace GameServer.Logic
{
	
	public class DBCommandEventArgs : EventArgs
	{
		
		public TCPProcessCmdResults Result;

		
		public string[] fields = null;
	}
}
