using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic.Damon
{
	// Token: 0x0200029A RID: 666
	internal class JingLingYaoSaiManager
	{
		// Token: 0x060009E1 RID: 2529 RVA: 0x0009D56C File Offset: 0x0009B76C
		public static GoodsData GetPaiZhuDamonGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.PaiZhuDamonGoodsDataList[i].Id == id)
						{
							return client.ClientData.PaiZhuDamonGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0009D634 File Offset: 0x0009B834
		public static void AddPaiZhuDamonGoodsData(GameClient client, GoodsData goodsData, bool refreshProps = true)
		{
			if (goodsData.Site == 0)
			{
				if (null == client.ClientData.PaiZhuDamonGoodsDataList)
				{
					client.ClientData.PaiZhuDamonGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					client.ClientData.PaiZhuDamonGoodsDataList.Add(goodsData);
				}
			}
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0009D6C8 File Offset: 0x0009B8C8
		public static void AddOldPaiZhuDamonGoodsData(GameClient client)
		{
			if (null != client.ClientData.GoodsDataList)
			{
				List<GoodsData> listDamon = new List<GoodsData>();
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					int nCategories = Global.GetGoodsCatetoriy(client.ClientData.GoodsDataList[i].GoodsID);
					if (nCategories >= 9 && nCategories <= 10)
					{
						if (client.ClientData.GoodsDataList[i].Using > 0 && client.ClientData.GoodsDataList[i].Site == 0)
						{
							int nBagIndex = JingLingYaoSaiManager.GetIdleSlotOfPaiZhuDamonGoods(client);
							string[] dbFields = null;
							string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.GoodsDataList[i].Id,
								client.ClientData.GoodsDataList[i].Using,
								"*",
								"*",
								"*",
								10000,
								"*",
								"*",
								client.ClientData.GoodsDataList[i].GCount,
								"*",
								nBagIndex,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(TCPClientPool.getInstance(), TCPOutPacketPool.getInstance(), 10006, strcmd, out dbFields, client.ServerId);
							if (dbRequestResult != TCPProcessCmdResults.RESULT_FAILED)
							{
								if (dbFields.Length > 0 && Convert.ToInt32(dbFields[1]) >= 0)
								{
									JingLingYaoSaiManager.AddPaiZhuDamonGoodsData(client, client.ClientData.GoodsDataList[i], false);
									client.ClientData.GoodsDataList[i].Site = 10000;
									client.ClientData.GoodsDataList[i].BagIndex = nBagIndex;
									listDamon.Add(client.ClientData.GoodsDataList[i]);
								}
							}
						}
					}
					IL_2AC:
					i++;
					continue;
					goto IL_2AC;
				}
				for (i = 0; i < listDamon.Count; i++)
				{
					Global.RemoveGoodsData(client, listDamon[i]);
				}
			}
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0009D9C8 File Offset: 0x0009BBC8
		public static void InitPaiZhuDemonGoodsDataList(GameClient client)
		{
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				client.ClientData.PaiZhuDamonGoodsDataList = Global.sendToDB<List<GoodsData>, string>(20314, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 10000, 10001), client.ServerId);
				if (client.ClientData.PaiZhuDamonGoodsDataList == null || client.ClientData.PaiZhuDamonGoodsDataList.Count == 0)
				{
					client.ClientData.PaiZhuDamonGoodsDataList = new List<GoodsData>();
					JingLingYaoSaiManager.AddOldPaiZhuDamonGoodsData(client);
				}
			}
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0009DA80 File Offset: 0x0009BC80
		public static void RemovePaiZhuDamonGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.PaiZhuDamonGoodsDataList)
			{
				if (goodsData.Site == 10000)
				{
					lock (client.ClientData.PaiZhuDamonGoodsDataList)
					{
						client.ClientData.PaiZhuDamonGoodsDataList.Remove(goodsData);
					}
				}
			}
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0009DB08 File Offset: 0x0009BD08
		public static bool CanAddGoodsToPaiZhuDamonCangKu(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.PaiZhuDamonGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int gridNum = Global.GetGoodsGridNumByID(goodsID);
				gridNum = Global.GMax(gridNum, 1);
				bool findOldGrid = false;
				int totalGridNum = 0;
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
					{
						totalGridNum++;
						if (canUseOld && gridNum > 1)
						{
							if (client.ClientData.PaiZhuDamonGoodsDataList[i].GoodsID == goodsID && client.ClientData.PaiZhuDamonGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.PaiZhuDamonGoodsDataList[i].Endtime, endTime))
							{
								if (client.ClientData.PaiZhuDamonGoodsDataList[i].GCount + newGoodsNum <= gridNum)
								{
									findOldGrid = true;
									break;
								}
							}
						}
					}
				}
				if (findOldGrid)
				{
					result = true;
				}
				else
				{
					int totalMaxGridCount = JingLingYaoSaiManager.GetPaiZhuDamonBagCapacity(client);
					result = (totalGridNum < totalMaxGridCount);
				}
			}
			return result;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0009DC7C File Offset: 0x0009BE7C
		public static int GetPaiZhuDamonBagCapacity(GameClient client)
		{
			return Global.ManorPetMax;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0009DC94 File Offset: 0x0009BE94
		public static int GetIdleSlotOfPaiZhuDamonGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
				{
					if (client.ClientData.PaiZhuDamonGoodsDataList[i].Site == 10000)
					{
						if (usedBagIndex.IndexOf(client.ClientData.PaiZhuDamonGoodsDataList[i].BagIndex) < 0)
						{
							usedBagIndex.Add(client.ClientData.PaiZhuDamonGoodsDataList[i].BagIndex);
						}
					}
				}
				int nCapacity = JingLingYaoSaiManager.GetPaiZhuDamonBagCapacity(client);
				for (int j = 0; j < nCapacity; j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						idelPos = j;
						break;
					}
				}
				result = idelPos;
			}
			return result;
		}
	}
}
