using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000465 RID: 1125
	public class CondJudger_NeedMarry : ICondJudger
	{
		// Token: 0x060014AF RID: 5295 RVA: 0x0014514C File Offset: 0x0014334C
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				if ("1" == arg && client.ClientData.MyMarriageData != null && client.ClientData.MyMarriageData.byMarrytype != -1)
				{
					bOK = true;
				}
				else if ("0" == arg && (client.ClientData.MyMarriageData == null || client.ClientData.MyMarriageData.byMarrytype == -1))
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				if ("1" == arg)
				{
					failedMsg = GLang.GetLang(683, new object[0]);
				}
				else
				{
					failedMsg = GLang.GetLang(684, new object[0]);
				}
			}
			return bOK;
		}
	}
}
