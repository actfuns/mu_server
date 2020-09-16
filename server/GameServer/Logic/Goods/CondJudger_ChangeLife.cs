using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_ChangeLife : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedChangeLife = -1;
				if (int.TryParse(arg, out iNeedChangeLife) && client.ClientData.ChangeLifeCount >= iNeedChangeLife)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(678, new object[0]), arg));
			}
			return bOK;
		}
	}
}
