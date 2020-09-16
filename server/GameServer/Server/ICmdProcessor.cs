using System;
using GameServer.Logic;

namespace GameServer.Server
{
	
	public interface ICmdProcessor
	{
		
		bool processCmd(GameClient client, string[] cmdParams);
	}
}
