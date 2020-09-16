using System;
using GameServer.Server;
using Server.Protocol;

namespace GameServer.Core.AssemblyPatch
{
	
	public class TcpResult
	{
		
		public static implicit operator TcpResult(TCPProcessCmdResults result)
		{
			return new TcpResult
			{
				cmdResult = result
			};
		}

		
		public TCPProcessCmdResults cmdResult = TCPProcessCmdResults.RESULT_FAILED;

		
		public TCPOutPacket outPacket = null;
	}
}
