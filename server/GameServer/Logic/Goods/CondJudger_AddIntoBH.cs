using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000470 RID: 1136
	public class CondJudger_AddIntoBH : ICondJudger
	{
		// Token: 0x060014C5 RID: 5317 RVA: 0x00145808 File Offset: 0x00143A08
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			int needAddIntoBH = Global.SafeConvertToInt32(arg);
			if (client != null)
			{
				if (needAddIntoBH > 0 && client.ClientData.Faction > 0)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = GLang.GetLang(2002, new object[0]);
			}
			return bOK;
		}
	}
}
