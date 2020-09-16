using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_XingHunNotMax : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.GamePayerRolePartXingZuo, true))
			{
				result = false;
			}
			else
			{
				if (!GameManager.StarConstellationMgr.IfStarConstellationPerfect(client))
				{
					bOK = true;
				}
				if (!bOK)
				{
					failedMsg = GLang.GetLang(8014, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
