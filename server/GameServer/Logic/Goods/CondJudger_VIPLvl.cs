using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000462 RID: 1122
	public class CondJudger_VIPLvl : ICondJudger
	{
		// Token: 0x060014A9 RID: 5289 RVA: 0x00144F70 File Offset: 0x00143170
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = -1;
				if (int.TryParse(arg, out iNeedLvl) && client.ClientData.VipLevel >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(680, new object[0]), arg));
			}
			return bOK;
		}
	}
}
