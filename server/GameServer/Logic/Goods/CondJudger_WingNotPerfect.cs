using System;
using GameServer.Logic.MUWings;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000472 RID: 1138
	public class CondJudger_WingNotPerfect : ICondJudger
	{
		// Token: 0x060014C9 RID: 5321 RVA: 0x0014599C File Offset: 0x00143B9C
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.GamePayerRolePartChiBang, true))
			{
				result = false;
			}
			else
			{
				if (!MUWingsManager.IfWingPerfect(client))
				{
					bOK = true;
				}
				else if (!LingYuManager.IfLingYuPerfect(client))
				{
					bOK = true;
				}
				else if (!ZhuLingZhuHunManager.IfZhuLingPerfect(client))
				{
					bOK = true;
				}
				else if (!ZhuLingZhuHunManager.IfZhuHunPerfect(client))
				{
					bOK = true;
				}
				if (!bOK)
				{
					failedMsg = GLang.GetLang(8018, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
