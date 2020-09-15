using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000477 RID: 1143
	public class CondJudger_MerlinLess : ICondJudger
	{
		// Token: 0x060014D5 RID: 5333 RVA: 0x001461C8 File Offset: 0x001443C8
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool result;
			if (!GameManager.MerlinMagicBookMgr.IsOpenMerlin(client))
			{
				result = false;
			}
			else
			{
				int nCurLevel = client.ClientData.MerlinData._Level;
				int nCurStarNum = client.ClientData.MerlinData._StarNum;
				List<int> iArgList = Global.StringToIntList(arg, '|');
				bool bOK = nCurLevel < iArgList[0] || (nCurLevel == iArgList[0] && nCurStarNum < iArgList[1]);
				if (!bOK)
				{
					failedMsg = GLang.GetLang(8013, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
