using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000748 RID: 1864
	public class MallPriceMgr
	{
		// Token: 0x06002EE1 RID: 12001 RVA: 0x0029FBCC File Offset: 0x0029DDCC
		public static void ClearCache()
		{
			lock (MallPriceMgr.PriceDict)
			{
				MallPriceMgr.PriceDict.Clear();
			}
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x0029FC1C File Offset: 0x0029DE1C
		public static int GetPriceByGoodsID(int goodsID)
		{
			int goodsPrice = -1;
			if (!MallPriceMgr.PriceDict.TryGetValue(goodsID, out goodsPrice))
			{
				goodsPrice = MallPriceMgr.GetPriceByGoodsIDFromCfg(goodsID);
				if (goodsPrice > 0)
				{
					lock (MallPriceMgr.PriceDict)
					{
						MallPriceMgr.PriceDict.Add(goodsID, goodsPrice);
					}
				}
			}
			return goodsPrice;
		}

		// Token: 0x06002EE3 RID: 12003 RVA: 0x0029FCA0 File Offset: 0x0029DEA0
		private static int GetPriceByGoodsIDFromCfg(int goodsID)
		{
			foreach (SystemXmlItem systemMallItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int myGoodsID = systemMallItem.GetIntValue("GoodsID", -1);
				if (myGoodsID == goodsID)
				{
					int price = systemMallItem.GetIntValue("Price", -1);
					if (price <= 0)
					{
						return -1;
					}
					string pubStartTime = systemMallItem.GetStringValue("PubStartTime");
					string pubEndTime = systemMallItem.GetStringValue("PubEndTime");
					if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
					{
						long startTime = Global.SafeConvertToTicks(pubStartTime);
						long endTime = Global.SafeConvertToTicks(pubEndTime);
						long nowTicks = TimeUtil.NOW();
						if (nowTicks < startTime || nowTicks > endTime)
						{
							return -1;
						}
					}
					return price;
				}
			}
			return -1;
		}

		// Token: 0x06002EE4 RID: 12004 RVA: 0x0029FDBC File Offset: 0x0029DFBC
		public static int GetPriceTypeByGoodsIDFromCfg(int goodsID)
		{
			foreach (SystemXmlItem systemMallItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int myGoodsID = systemMallItem.GetIntValue("GoodsID", -1);
				if (myGoodsID == goodsID)
				{
					int tabID = systemMallItem.GetIntValue("TabID", -1);
					if (10000 == tabID)
					{
						return 1;
					}
				}
			}
			return 0;
		}

		// Token: 0x04003C7F RID: 15487
		private static Dictionary<int, int> PriceDict = new Dictionary<int, int>();
	}
}
