using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_YuanBaoMoreThan : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedYB = int.MaxValue;
				if (int.TryParse(arg, out iNeedYB) && client.ClientData.UserMoney >= iNeedYB)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(686, new object[0]), arg));
			}
			return bOK;
		}
	}
}
