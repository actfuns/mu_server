using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000461 RID: 1121
	internal class CondJudger_RoleLevel : ICondJudger
	{
		// Token: 0x060014A7 RID: 5287 RVA: 0x00144EDC File Offset: 0x001430DC
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = -1;
				if (int.TryParse(arg, out iNeedLvl) && client.ClientData.Level >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(679, new object[0]), arg));
			}
			return bOK;
		}
	}
}
