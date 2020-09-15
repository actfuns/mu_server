using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200045F RID: 1119
	internal class CondJudger_JunXianLvl : ICondJudger
	{
		// Token: 0x060014A3 RID: 5283 RVA: 0x00144DB4 File Offset: 0x00142FB4
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = -1;
				if (int.TryParse(arg, out iNeedLvl) && GameManager.ClientMgr.GetShengWangLevelValue(client) >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(677, new object[0]), arg));
			}
			return bOK;
		}
	}
}
