using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_EquipAppendLvl : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = int.MaxValue;
				if (int.TryParse(arg, out iNeedLvl) && client.ClientData._ReplaceExtArg.CurrEquipZhuiJiaLevel >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format("当前操作的装备的追加等级不能低于{0}", arg);
			}
			return bOK;
		}
	}
}
