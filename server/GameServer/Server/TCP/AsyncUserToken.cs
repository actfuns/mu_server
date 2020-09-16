using System;

namespace Server.TCP
{
	
	public class AsyncUserToken : IDisposable
	{
		
		public void Dispose()
		{
			this.CurrentSocket = null;
			this.Tag = null;
		}

		
		
		
		public TMSKSocket CurrentSocket { get; set; }

		
		
		
		public object Tag { get; set; }

		
		public SendBuffer _SendBuffer;
	}
}
