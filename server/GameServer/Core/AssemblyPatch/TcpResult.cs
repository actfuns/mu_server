using System;
using GameServer.Server;
using Server.Protocol;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x020000C3 RID: 195
	public class TcpResult
	{
		// Token: 0x06000360 RID: 864 RVA: 0x0003B910 File Offset: 0x00039B10
		public static implicit operator TcpResult(TCPProcessCmdResults result)
		{
			return new TcpResult
			{
				cmdResult = result
			};
		}

		// Token: 0x040004C7 RID: 1223
		public TCPProcessCmdResults cmdResult = TCPProcessCmdResults.RESULT_FAILED;

		// Token: 0x040004C8 RID: 1224
		public TCPOutPacket outPacket = null;
	}
}
