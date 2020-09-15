using System;
using GameServer.Server;

namespace GameServer.Logic
{
	// Token: 0x0200061D RID: 1565
	public class DBCommandEventArgs : EventArgs
	{
		// Token: 0x04002CC1 RID: 11457
		public TCPProcessCmdResults Result;

		// Token: 0x04002CC2 RID: 11458
		public string[] fields = null;
	}
}
