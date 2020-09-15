using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200045D RID: 1117
	internal class CondJudger_WingSuit : ICondJudger
	{
		// Token: 0x0600149F RID: 5279 RVA: 0x00144C04 File Offset: 0x00142E04
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && client.ClientData.MyWingData != null && !string.IsNullOrEmpty(arg))
			{
				List<int> iArgList = Global.StringToIntList(arg, '|');
				if (iArgList.Count == 2)
				{
					if (client.ClientData.MyWingData.WingID > iArgList[0] || (client.ClientData.MyWingData.WingID == iArgList[0] && client.ClientData.MyWingData.ForgeLevel >= iArgList[1]))
					{
						bOK = true;
					}
				}
				else if (iArgList.Count == 1)
				{
					if (client.ClientData.MyWingData.WingID >= iArgList[0])
					{
						bOK = true;
					}
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(675, new object[0]), arg));
			}
			return bOK;
		}
	}
}
