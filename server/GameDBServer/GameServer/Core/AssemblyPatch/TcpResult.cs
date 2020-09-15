using System;
using GameDBServer.Server;
using Server.Protocol;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x02000005 RID: 5
	public class TcpResult
	{
		// Token: 0x0400000C RID: 12
		public TCPProcessCmdResults cmdResult = TCPProcessCmdResults.RESULT_FAILED;

		// Token: 0x0400000D RID: 13
		public TCPOutPacket outPacket = null;
	}
}
