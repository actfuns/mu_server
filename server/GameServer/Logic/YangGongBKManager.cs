using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007EC RID: 2028
	public class YangGongBKManager
	{
		// Token: 0x06003952 RID: 14674 RVA: 0x0030AE94 File Offset: 0x00309094
		public static YangGongBKItem OpenYangGongBK(GameClient client, bool isBaoWuBinding)
		{
			YangGongBKItem yangGongBKItem = null;
			int yangGongFallID = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID1", -1);
			int yangGongFallID2 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID2", -1);
			int yangGongFallID3 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID3", -1);
			int yangGongFallID4 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID4", -1);
			int yangGongFallID5 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID5", -1);
			int yangGongFallID6 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID6", -1);
			int yangGongFallID7 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID7", -1);
			int yangGongFallID8 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID8", -1);
			YangGongBKItem result;
			if (yangGongFallID <= 0 || yangGongFallID2 <= 0 || yangGongFallID3 <= 0 || yangGongFallID4 <= 0)
			{
				result = yangGongBKItem;
			}
			else if (yangGongFallID5 <= 0 || yangGongFallID6 <= 0 || yangGongFallID7 <= 0 || yangGongFallID8 <= 0)
			{
				result = yangGongBKItem;
			}
			else
			{
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID, 1, false);
				List<FallGoodsItem> gallGoodsItemList2 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID2, 1, false);
				List<FallGoodsItem> gallGoodsItemList3 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID3, 1, false);
				List<FallGoodsItem> gallGoodsItemList4 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID4, 1, false);
				List<FallGoodsItem> gallGoodsItemList5 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID5, 1, true);
				List<FallGoodsItem> gallGoodsItemList6 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID6, 1, true);
				List<FallGoodsItem> gallGoodsItemList7 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID7, 1, true);
				List<FallGoodsItem> gallGoodsItemList8 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(yangGongFallID8, 1, true);
				if (gallGoodsItemList == null || gallGoodsItemList2 == null || gallGoodsItemList3 == null || null == gallGoodsItemList4)
				{
					result = yangGongBKItem;
				}
				else if (gallGoodsItemList5 == null || gallGoodsItemList6 == null || gallGoodsItemList7 == null || null == gallGoodsItemList8)
				{
					result = yangGongBKItem;
				}
				else if (1 != gallGoodsItemList.Count || 1 != gallGoodsItemList2.Count || 1 != gallGoodsItemList3.Count || 1 != gallGoodsItemList4.Count)
				{
					result = yangGongBKItem;
				}
				else if (1 != gallGoodsItemList5.Count || 1 != gallGoodsItemList6.Count || 1 != gallGoodsItemList7.Count || 1 != gallGoodsItemList8.Count)
				{
					result = yangGongBKItem;
				}
				else
				{
					gallGoodsItemList.AddRange(gallGoodsItemList2);
					gallGoodsItemList.AddRange(gallGoodsItemList3);
					gallGoodsItemList.AddRange(gallGoodsItemList4);
					gallGoodsItemList.AddRange(gallGoodsItemList5);
					gallGoodsItemList.AddRange(gallGoodsItemList6);
					gallGoodsItemList.AddRange(gallGoodsItemList7);
					gallGoodsItemList.AddRange(gallGoodsItemList8);
					gallGoodsItemList = Global.RandomSortList<FallGoodsItem>(gallGoodsItemList);
					List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(gallGoodsItemList);
					List<GoodsData> tempGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						tempGoodsDataList.Add(goodsDataList[i]);
					}
					yangGongBKItem = new YangGongBKItem
					{
						FallGoodsItemList = gallGoodsItemList,
						GoodsDataList = goodsDataList,
						IsBaoWuBinding = isBaoWuBinding,
						TempGoodsDataList = tempGoodsDataList
					};
					result = yangGongBKItem;
				}
			}
			return result;
		}

		// Token: 0x06003953 RID: 14675 RVA: 0x0030B188 File Offset: 0x00309388
		public static int ClickYangGongBK(GameClient client, YangGongBKItem yangGongBKItem, out GoodsData goodsData)
		{
			goodsData = null;
			if (null == YangGongBKManager.YangGongBKNumPercents)
			{
				YangGongBKManager.YangGongBKNumPercents = GameManager.systemParamsList.GetParamValueDoubleArrayByName("YangGongBKNumPercents", ',');
			}
			int result;
			if (YangGongBKManager.YangGongBKNumPercents == null || YangGongBKManager.YangGongBKNumPercents.Length != 4)
			{
				result = -1000;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = yangGongBKItem.FallGoodsItemList;
				List<GoodsData> goodsDataList = yangGongBKItem.GoodsDataList;
				if (fallGoodsItemList == null || null == goodsDataList)
				{
					result = -200;
				}
				else if (fallGoodsItemList.Count != goodsDataList.Count)
				{
					result = -200;
				}
				else
				{
					double numPercent = YangGongBKManager.YangGongBKNumPercents[yangGongBKItem.ClickBKNum];
					int findIndex = -1;
					for (int i = 0; i < fallGoodsItemList.Count; i++)
					{
						int randNum = Global.GetRandomNumber(1, 10001);
						int percent = fallGoodsItemList[i].SelfPercent;
						if (fallGoodsItemList[i].IsGood)
						{
							percent = (int)((double)percent * numPercent);
						}
						if (randNum <= percent)
						{
							if (!yangGongBKItem.PickUpDict.ContainsKey(goodsDataList[i].Id))
							{
								findIndex = i;
								break;
							}
						}
					}
					if (-1 == findIndex)
					{
						int maxIndex = -1;
						int maxPercent = 0;
						for (int i = 0; i < fallGoodsItemList.Count; i++)
						{
							if (!yangGongBKItem.PickUpDict.ContainsKey(goodsDataList[i].Id))
							{
								if (fallGoodsItemList[i].SelfPercent > maxPercent)
								{
									maxIndex = i;
									maxPercent = fallGoodsItemList[i].SelfPercent;
								}
							}
						}
						findIndex = maxIndex;
					}
					if (findIndex < 0)
					{
						result = -300;
					}
					else
					{
						goodsData = goodsDataList[findIndex];
						result = findIndex;
					}
				}
			}
			return result;
		}

		// Token: 0x0400433D RID: 17213
		private static double[] YangGongBKNumPercents = null;
	}
}
