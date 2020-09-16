using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_WingSuitLess : ICondJudger
	{
		
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
