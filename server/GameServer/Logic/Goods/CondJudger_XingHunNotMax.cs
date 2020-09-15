using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000476 RID: 1142
	public class CondJudger_XingHunNotMax : ICondJudger
	{
		// Token: 0x060014D3 RID: 5331 RVA: 0x00146164 File Offset: 0x00144364
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.GamePayerRolePartXingZuo, true))
			{
				result = false;
			}
			else
			{
				if (!GameManager.StarConstellationMgr.IfStarConstellationPerfect(client))
				{
					bOK = true;
				}
				if (!bOK)
				{
					failedMsg = GLang.GetLang(8014, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
