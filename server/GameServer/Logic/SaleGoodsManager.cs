using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007CB RID: 1995
	public class SaleGoodsManager
	{
		// Token: 0x0600382E RID: 14382 RVA: 0x002FA6F0 File Offset: 0x002F88F0
		public static void AddSaleGoodsItem(SaleGoodsItem saleGoodsItem)
		{
			SaleManager.AddSaleGoodsItem(saleGoodsItem);
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				SaleGoodsManager._SaleGoodsDict[saleGoodsItem.GoodsDbID] = saleGoodsItem;
				SaleGoodsManager._SaleGoodsDataList = null;
			}
		}

		// Token: 0x0600382F RID: 14383 RVA: 0x002FA758 File Offset: 0x002F8958
		public static void AddSaleGoodsItems(GameClient client)
		{
			List<GoodsData> goodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != goodsDataList)
			{
				lock (goodsDataList)
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						if (goodsDataList[i].Binding <= 0)
						{
							SaleGoodsItem saleGoodsItem = new SaleGoodsItem
							{
								GoodsDbID = goodsDataList[i].Id,
								SalingGoodsData = goodsDataList[i],
								Client = client
							};
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
						}
					}
				}
			}
		}

		// Token: 0x06003830 RID: 14384 RVA: 0x002FA824 File Offset: 0x002F8A24
		public static SaleGoodsItem RemoveSaleGoodsItem(int goodsDbID)
		{
			SaleManager.RemoveSaleGoodsItem(goodsDbID);
			SaleGoodsItem result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				SaleGoodsItem saleGoodsItem = null;
				if (SaleGoodsManager._SaleGoodsDict.TryGetValue(goodsDbID, out saleGoodsItem))
				{
					SaleGoodsManager._SaleGoodsDict.Remove(goodsDbID);
				}
				SaleGoodsManager._SaleGoodsDataList = null;
				result = saleGoodsItem;
			}
			return result;
		}

		// Token: 0x06003831 RID: 14385 RVA: 0x002FA8A0 File Offset: 0x002F8AA0
		public static void RemoveSaleGoodsItems(GameClient client)
		{
			List<GoodsData> goodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != goodsDataList)
			{
				lock (goodsDataList)
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						SaleGoodsManager.RemoveSaleGoodsItem(goodsDataList[i].Id);
					}
				}
			}
		}

		// Token: 0x06003832 RID: 14386 RVA: 0x002FA924 File Offset: 0x002F8B24
		public static List<SaleGoodsData> GetSaleGoodsDataList()
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				if (null != SaleGoodsManager._SaleGoodsDataList)
				{
					result = SaleGoodsManager._SaleGoodsDataList;
				}
				else
				{
					List<SaleGoodsData> saleGoodsDataList = new List<SaleGoodsData>();
					foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
					{
						saleGoodsDataList.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (saleGoodsDataList.Count >= 250)
						{
							break;
						}
					}
					SaleGoodsManager._SaleGoodsDataList = saleGoodsDataList;
					result = saleGoodsDataList;
				}
			}
			return result;
		}

		// Token: 0x06003833 RID: 14387 RVA: 0x002FAA88 File Offset: 0x002F8C88
		public static List<SaleGoodsData> FindSaleGoodsDataList(Dictionary<int, bool> goodsIDDict)
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				List<SaleGoodsData> saleGoodsDataList = new List<SaleGoodsData>();
				foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
				{
					if (goodsIDDict.ContainsKey(saleGoodsItem.SalingGoodsData.GoodsID))
					{
						saleGoodsDataList.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (saleGoodsDataList.Count >= 250)
						{
							break;
						}
					}
				}
				result = saleGoodsDataList;
			}
			return result;
		}

		// Token: 0x06003834 RID: 14388 RVA: 0x002FABE8 File Offset: 0x002F8DE8
		public static List<SaleGoodsData> FindSaleGoodsDataListByRoleName(string searchText)
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				List<SaleGoodsData> saleGoodsDataList = new List<SaleGoodsData>();
				foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
				{
					if (-1 != saleGoodsItem.Client.ClientData.RoleName.IndexOf(searchText))
					{
						saleGoodsDataList.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (saleGoodsDataList.Count >= 250)
						{
							break;
						}
					}
				}
				result = saleGoodsDataList;
			}
			return result;
		}

		// Token: 0x06003835 RID: 14389 RVA: 0x002FAD54 File Offset: 0x002F8F54
		public static int GetNewBaiTanJinBiID()
		{
			int result;
			lock (SaleGoodsManager.Mutex)
			{
				int id = SaleGoodsManager.BaseBaiTanJinBiID;
				SaleGoodsManager.BaseBaiTanJinBiID--;
				result = id;
			}
			return result;
		}

		// Token: 0x04004145 RID: 16709
		private static List<SaleGoodsData> _SaleGoodsDataList = null;

		// Token: 0x04004146 RID: 16710
		private static Dictionary<int, SaleGoodsItem> _SaleGoodsDict = new Dictionary<int, SaleGoodsItem>();

		// Token: 0x04004147 RID: 16711
		private static object Mutex = new object();

		// Token: 0x04004148 RID: 16712
		private static int BaseBaiTanJinBiID = -1;
	}
}
