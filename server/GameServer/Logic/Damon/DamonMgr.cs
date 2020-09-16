using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic.Damon
{
	
	internal class DamonMgr
	{
		
		public static GoodsData GetDamonGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.DamonGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Id == id)
						{
							return client.ClientData.DamonGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static void AddDamonGoodsData(GameClient client, GoodsData goodsData, bool refreshProps = true)
		{
			if (goodsData.Site == 0 || goodsData.Site == 10000)
			{
				if (null == client.ClientData.DamonGoodsDataList)
				{
					client.ClientData.DamonGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.DamonGoodsDataList)
				{
					client.ClientData.DamonGoodsDataList.Add(goodsData);
				}
				JingLingQiYuanManager.getInstance().RefreshProps(client, true);
			}
		}

		
		public static void AddOldDamonGoodsData(GameClient client)
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
							int nBagIndex = Global.GetIdleSlotOfDamonGoods(client);
							string[] dbFields = null;
							string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.GoodsDataList[i].Id,
								client.ClientData.GoodsDataList[i].Using,
								"*",
								"*",
								"*",
								5000,
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
									DamonMgr.AddDamonGoodsData(client, client.ClientData.GoodsDataList[i], false);
									client.ClientData.GoodsDataList[i].Site = 5000;
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
				JingLingQiYuanManager.getInstance().RefreshProps(client, true);
			}
		}

		
		public static void InitDemonGoodsDataList(GameClient client)
		{
			if (null == client.ClientData.DamonGoodsDataList)
			{
				client.ClientData.DamonGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 5000), client.ServerId);
				if (client.ClientData.DamonGoodsDataList == null || client.ClientData.DamonGoodsDataList.Count == 0)
				{
					client.ClientData.DamonGoodsDataList = new List<GoodsData>();
					DamonMgr.AddOldDamonGoodsData(client);
				}
			}
			JingLingQiYuanManager.getInstance().RefreshProps(client, true);
		}

		
		public static GoodsData AddDamonGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData gd = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			DamonMgr.AddDamonGoodsData(client, gd, true);
			return gd;
		}

		
		public static void RemoveDamonGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.DamonGoodsDataList)
			{
				if (goodsData.Site == 5000)
				{
					lock (client.ClientData.DamonGoodsDataList)
					{
						client.ClientData.DamonGoodsDataList.Remove(goodsData);
					}
					JingLingQiYuanManager.getInstance().RefreshProps(client, true);
				}
			}
		}

		
		public static void ResetDamonBagAllGoods(GameClient client)
		{
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
					List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using <= 0)
						{
							client.ClientData.DamonGoodsDataList[i].BagIndex = 1;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.DamonGoodsDataList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.DamonGoodsDataList[i].GoodsID, client.ClientData.DamonGoodsDataList[i].Binding, Global.DateTimeTicks(client.ClientData.DamonGoodsDataList[i].Endtime));
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.DamonGoodsDataList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.DamonGoodsDataList[i].GCount -= toAddNum;
									client.ClientData.DamonGoodsDataList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.DamonGoodsDataList[i]))
									{
										break;
									}
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.DamonGoodsDataList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.DamonGoodsDataList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.DamonGoodsDataList[i]);
										}
									}
									else if (client.ClientData.DamonGoodsDataList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.DamonGoodsDataList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.DamonGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
					{
						client.ClientData.DamonGoodsDataList.Remove(toRemovedGoodsDataList[i]);
					}
					client.ClientData.DamonGoodsDataList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int index = 0;
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.DamonGoodsDataList[i].BagIndex = index++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.DamonGoodsDataList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.DamonGoodsDataList, Global._TCPManager.TcpOutPacketPool, 449);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.DamonGoodsDataList, Global._TCPManager.TcpOutPacketPool, 449);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		
		public static bool CanAddGoodsToDamonCangKu(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.DamonGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int gridNum = Global.GetGoodsGridNumByID(goodsID);
				gridNum = Global.GMax(gridNum, 1);
				bool findOldGrid = false;
				int totalGridNum = 0;
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						totalGridNum++;
						if (canUseOld && gridNum > 1)
						{
							if (client.ClientData.DamonGoodsDataList[i].GoodsID == goodsID && client.ClientData.DamonGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.DamonGoodsDataList[i].Endtime, endTime))
							{
								if (client.ClientData.DamonGoodsDataList[i].GCount + newGoodsNum <= gridNum)
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
					int totalMaxGridCount = DamonMgr.GetDamonBagCapacity(client);
					result = (totalGridNum < totalMaxGridCount);
				}
			}
			return result;
		}

		
		public static int GetDamonBagCapacity(GameClient client)
		{
			return Global.MaxDamonGridNum;
		}

		
		public static List<GoodsData> GetDemonGoodsDataList(GameClient client)
		{
			List<GoodsData> demonGoodsDataList = new List<GoodsData>();
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					demonGoodsDataList.AddRange(client.ClientData.DamonGoodsDataList);
				}
			}
			return demonGoodsDataList;
		}
	}
}
