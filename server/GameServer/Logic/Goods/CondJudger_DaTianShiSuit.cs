using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000464 RID: 1124
	public class CondJudger_DaTianShiSuit : ICondJudger
	{
		// Token: 0x060014AD RID: 5293 RVA: 0x001450B8 File Offset: 0x001432B8
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
