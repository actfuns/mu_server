using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_DaTianShiSuit : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedSuit = int.MaxValue;
				if (int.TryParse(arg, out iNeedSuit) && client.UsingEquipMgr.GetUsingEquipArchangelWeaponSuit() >= iNeedSuit)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(682, new object[0]), arg));
			}
			return bOK;
		}
	}
}
