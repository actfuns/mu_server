using System;

namespace GameServer.Logic.Goods
{
	
	public interface ICondJudger
	{
		
		bool Judge(GameClient client, string arg, out string failedMsg);
	}
}
