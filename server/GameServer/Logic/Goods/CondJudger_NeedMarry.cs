using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_NeedMarry : ICondJudger
	{
		
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
