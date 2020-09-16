using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_EquipForgeLvl : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = int.MaxValue;
				if (int.TryParse(arg, out iNeedLvl) && client.ClientData._ReplaceExtArg.CurrEquipQiangHuaLevel >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format("当前装备的强化等级不能低于{0}", arg);
			}
			return bOK;
		}
	}
}
