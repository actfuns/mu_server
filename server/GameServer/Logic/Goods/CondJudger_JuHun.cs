using System;

namespace GameServer.Logic.Goods
{
	
	internal class CondJudger_JuHun : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedType = int.MaxValue;
				if (int.TryParse(arg, out iNeedType) && client.ClientData._ReplaceExtArg.CurrEquipJuHun >= iNeedType)
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
