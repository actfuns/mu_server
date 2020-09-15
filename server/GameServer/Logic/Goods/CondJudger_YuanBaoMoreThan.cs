using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046A RID: 1130
	internal class CondJudger_YuanBaoMoreThan : ICondJudger
	{
		// Token: 0x060014B9 RID: 5305 RVA: 0x0014542C File Offset: 0x0014362C
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
