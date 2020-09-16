using System;
using System.Net.Sockets;

namespace Server.TCP
{
	
	public class AsyncUserToken : IDisposable
	{
		
		public void Dispose()
		{
			this.CurrentSocket = null;
			this.Tag = null;
		}

		
		
		
		public Socket CurrentSocket { get; set; }

		
		
		
		public object Tag { get; set; }
	}
}
