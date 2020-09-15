using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046B RID: 1131
	internal class CondJudger_NeedOpen : ICondJudger
	{
		// Token: 0x060014BB RID: 5307 RVA: 0x001454C4 File Offset: 0x001436C4
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = true;
			GongNengIDs gongnengId = (GongNengIDs)Convert.ToInt32(arg);
			if (!GlobalNew.IsGongNengOpened(client, gongnengId, false))
			{
				bOK = false;
			}
			if (!bOK)
			{
				failedMsg = string.Format("物品对应的功能没有开启", new object[0]);
			}
			return bOK;
		}
	}
}
