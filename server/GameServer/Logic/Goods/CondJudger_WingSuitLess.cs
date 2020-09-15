using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000471 RID: 1137
	internal class CondJudger_WingSuitLess : ICondJudger
	{
		// Token: 0x060014C7 RID: 5319 RVA: 0x00145878 File Offset: 0x00143A78
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
				if (client != null && client.ClientData.MyWingData != null && !string.IsNullOrEmpty(arg))
				{
					List<int> iArgList = Global.StringToIntList(arg, '|');
					if (iArgList.Count == 2)
					{
						if (client.ClientData.MyWingData.WingID < iArgList[0] || (client.ClientData.MyWingData.WingID == iArgList[0] && client.ClientData.MyWingData.ForgeLevel < iArgList[1]))
						{
							bOK = true;
						}
					}
					else if (iArgList.Count == 1)
					{
						if (client.ClientData.MyWingData.WingID < iArgList[0])
						{
							bOK = true;
						}
					}
				}
				if (!bOK)
				{
					failedMsg = GLang.GetLang(8019, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
