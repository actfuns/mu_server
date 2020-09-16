using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class MallPriceMgr
	{
		
		public static void ClearCache()
		{
			lock (MallPriceMgr.PriceDict)
			{
				MallPriceMgr.PriceDict.Clear();
			}
		}

		
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

		
		private static Dictionary<int, int> PriceDict = new Dictionary<int, int>();
	}
}
