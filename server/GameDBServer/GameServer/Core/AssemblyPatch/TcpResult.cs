using System;
using GameDBServer.Server;
using Server.Protocol;

namespace GameServer.Core.AssemblyPatch
{
	
	public class TcpResult
	{
		
		public TCPProcessCmdResults cmdResult = TCPProcessCmdResults.RESULT_FAILED;

		
		public TCPOutPacket outPacket = null;
	}
}
