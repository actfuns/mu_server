using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_AddIntoBH : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			int needAddIntoBH = Global.SafeConvertToInt32(arg);
			if (client != null)
			{
				if (needAddIntoBH > 0 && client.ClientData.Faction > 0)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = GLang.GetLang(2002, new object[0]);
			}
			return bOK;
		}
	}
}
