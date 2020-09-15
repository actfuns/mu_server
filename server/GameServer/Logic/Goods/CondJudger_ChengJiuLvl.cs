using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200045E RID: 1118
	internal class CondJudger_ChengJiuLvl : ICondJudger
	{
		// Token: 0x060014A1 RID: 5281 RVA: 0x00144D28 File Offset: 0x00142F28
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
