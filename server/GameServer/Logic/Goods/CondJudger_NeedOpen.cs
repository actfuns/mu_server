using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_NeedOpen : ICondJudger
	{
		
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
