using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_EquipSuit : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedSuit = int.MaxValue;
				if (int.TryParse(arg, out iNeedSuit) && client.ClientData._ReplaceExtArg.CurrEquipSuit >= iNeedSuit)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format("当前装备的品阶不能低于{0}", arg);
			}
			return bOK;
		}
	}
}
