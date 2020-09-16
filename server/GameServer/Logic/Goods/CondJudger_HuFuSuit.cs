using System;
using Server.Data;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_HuFuSuit : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				GoodsData usingHuFu = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 22);
				if (usingHuFu != null)
				{
					int iNeedSuit = -1;
					if (int.TryParse(arg, out iNeedSuit) && Global.GetEquipGoodsSuitID(usingHuFu.GoodsID) >= iNeedSuit)
					{
						bOK = true;
					}
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(681, new object[0]), arg));
			}
			return bOK;
		}
	}
}
