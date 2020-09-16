using System;

namespace GameServer.Server
{
	
	public class CmdHandler
	{
		
		public uint CmdFlags;

		
		public short MinParamCount;

		
		public short MaxParamCount;

		
		public ICmdProcessor CmdProcessor;
	}
}
