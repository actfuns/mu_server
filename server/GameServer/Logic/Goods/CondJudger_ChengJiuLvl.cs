using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_ChengJiuLvl : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = -1;
				if (int.TryParse(arg, out iNeedLvl) && ChengJiuManager.GetChengJiuLevel(client) >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(676, new object[0]), arg));
			}
			return bOK;
		}
	}
}
