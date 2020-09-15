using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000460 RID: 1120
	internal class CondJudger_ChangeLife : ICondJudger
	{
		// Token: 0x060014A5 RID: 5285 RVA: 0x00144E48 File Offset: 0x00143048
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
