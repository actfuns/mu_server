using System;
using GameServer.Logic;

namespace GameServer.Server
{
	
	public interface ICmdProcessorEx : ICmdProcessor
	{
		
		bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams);
	}
}
