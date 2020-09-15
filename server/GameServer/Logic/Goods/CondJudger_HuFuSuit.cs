using System;
using Server.Data;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000463 RID: 1123
	public class CondJudger_HuFuSuit : ICondJudger
	{
		// Token: 0x060014AB RID: 5291 RVA: 0x00145004 File Offset: 0x00143204
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
