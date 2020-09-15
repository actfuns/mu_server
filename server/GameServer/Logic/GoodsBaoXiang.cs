using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006DC RID: 1756
	public class GoodsBaoXiang
	{
		// Token: 0x060029E2 RID: 10722 RVA: 0x00259540 File Offset: 0x00257740
		public static void ProcessFallBaoXiang_StepOne(GameClient client, SystemXmlItem systemGoodsItem, GoodsData goodsData, ref int UseNum)
		{
			List<MagicActionItem> magicActionItemList = null;
			if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out magicActionItemList) && null != magicActionItemList)
			{
				if (magicActionItemList.Count > 0)
				{
					MagicActionItem magicActionItem = magicActionItemList[0];
					if (MagicActionIDs.FALL_BAOXIANG == magicActionItem.MagicActionID || MagicActionIDs.FALL_BAOXIANG_2 == magicActionItem.MagicActionID)
					{
						int fallID = (int)magicActionItem.MagicActionParams[0];
						int maxFallCount = (int)magicActionItem.MagicActionParams[1];
						int _site = 0;
						if (MagicActionIDs.FALL_BAOXIANG_2 == magicActionItem.MagicActionID)
						{
							_site = (int)magicActionItem.MagicActionParams[2];
						}
						List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
						if (null != gallGoodsItemList)
						{
							List<GoodsData> goodsDataList = new List<GoodsData>();
							int useGoodsLoop;
							for (useGoodsLoop = 0; useGoodsLoop < UseNum; useGoodsLoop++)
							{
								List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, maxFallCount, 1, 1.0);
								if (tempItemList2.Count > 0)
								{
									List<GoodsData> tempGoodsList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
									if (null != tempGoodsList)
									{
										bool canUse = true;
										foreach (GoodsData item2 in tempGoodsList)
										{
											SystemXmlItem systemGoods = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item2.GoodsID, out systemGoods))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("掉落包配置出错 fallID={0} goodsID={1}", fallID, item2.GoodsID), null, true);
												canUse = false;
												break;
											}
											int categoriy = systemGoods.GetIntValue("Categoriy", -1);
											int site = Global.GetSiteByCategoriy(categoriy);
											if (site != _site && _site != 15000)
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("掉落包配置出错 fallID={0} goodsID={1}", fallID, item2.GoodsID), null, true);
												canUse = false;
												break;
											}
											item2.Site = site;
											item2.Binding = goodsData.Binding;
										}
										if (!canUse)
										{
											break;
										}
										using (List<GoodsData>.Enumerator enumerator = tempGoodsList.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												GoodsData item = enumerator.Current;
												GoodsData findGoods = goodsDataList.Find((GoodsData x) => x.GoodsID == item.GoodsID && x.ExcellenceInfo == item.ExcellenceInfo && x.GCount + item.GCount <= Global.GetGoodsGridNumByID(x.GoodsID));
												if (null == findGoods)
												{
													goodsDataList.Add(item);
												}
												else
												{
													findGoods.GCount += item.GCount;
												}
											}
										}
										canUse = Global.CanAddGoodsNum(client, _site, goodsDataList.Count);
										if (!canUse)
										{
											using (List<GoodsData>.Enumerator enumerator = tempGoodsList.GetEnumerator())
											{
												while (enumerator.MoveNext())
												{
													GoodsData item = enumerator.Current;
													GoodsData findGoods = goodsDataList.Find((GoodsData x) => x.GoodsID == item.GoodsID);
													if (null != findGoods)
													{
														findGoods.GCount -= item.GCount;
														if (findGoods.GCount <= 0)
														{
															goodsDataList.Remove(findGoods);
														}
													}
												}
											}
											break;
										}
									}
								}
							}
							UseNum = useGoodsLoop;
							client.ClientData.FallBaoXiangGoodsList = goodsDataList;
						}
					}
				}
			}
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x00259944 File Offset: 0x00257B44
		public static void ProcessFallBaoXiang_StepTwo(GameClient client, int fallID, int maxFallCount, int binding, int actionGoodsID)
		{
			List<GoodsData> goodsDataList = client.ClientData.FallBaoXiangGoodsList;
			if (null != goodsDataList)
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					SystemXmlItem systemGoods = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsDataList[i].GoodsID, out systemGoods))
					{
						int categoriy = systemGoods.GetIntValue("Categoriy", -1);
						binding = Global.GetBindingByCategoriy(categoriy, goodsDataList[i].Binding);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, goodsDataList[i].Props, goodsDataList[i].Forge_level, binding, goodsDataList[i].Site, "", true, 1, "掉落宝箱获取", goodsDataList[i].Endtime, goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, goodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
						Global.BroadcastFallBaoXiangGoodsHint(client, goodsDataList[i], actionGoodsID);
						Global.BroadSelfFallBoxGoods(client, goodsDataList[i]);
						if (client.ClientData.RoleAwardMsgType == RoleAwardMsg.RandomBaoXiang)
						{
							client.ClientData.AddAwardRecord(RoleAwardMsg.RandomBaoXiang, new GoodsData(goodsDataList[i]), false);
						}
					}
				}
				client.ClientData.FallBaoXiangGoodsList = null;
			}
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x00259B0C File Offset: 0x00257D0C
		public static void ProcessFallBaoXiang(GameClient client, int fallID, int maxFallCount, int binding, int actionGoodsID)
		{
			if (fallID > 0)
			{
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null != gallGoodsItemList)
				{
					List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, maxFallCount, 1, 1.0);
					if (tempItemList2.Count > 0)
					{
						List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						if (Global.CanAddGoodsNum(client, goodsDataList.Count))
						{
							for (int i = 0; i < goodsDataList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, goodsDataList[i].Props, goodsDataList[i].Forge_level, binding, 0, "", true, 1, "掉落宝箱获取", goodsDataList[i].Endtime, goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, goodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
								Global.BroadcastFallBaoXiangGoodsHint(client, goodsDataList[i], actionGoodsID);
								if (client.ClientData.RoleAwardMsgType == RoleAwardMsg.RandomBaoXiang)
								{
									client.ClientData.AddAwardRecord(RoleAwardMsg.RandomBaoXiang, new GoodsData(goodsDataList[i]), false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x00259CC0 File Offset: 0x00257EC0
		public static List<GoodsData> FetchGoodListBaseFallPacketID(GameClient client, int fallID, int maxFallCount, FallAlgorithm fallAlgorithm)
		{
			List<GoodsData> goodsDataList = null;
			List<GoodsData> result;
			if (fallID <= 0)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == gallGoodsItemList)
				{
					result = null;
				}
				else
				{
					List<FallGoodsItem> fixedFixedFallGoodsItemList = GameManager.GoodsPackMgr.GetFixedFallGoodsItemList(fallID);
					List<GoodsData> fixedGoodsDataList = GameManager.GoodsPackMgr.GetFixedGoodsDataList(fixedFixedFallGoodsItemList, GameManager.GoodsPackMgr.GetFixedFallGoodsMaxCount(fallID));
					if (null != fixedGoodsDataList)
					{
						goodsDataList = fixedGoodsDataList;
					}
					List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, maxFallCount, (int)fallAlgorithm, 1.0);
					if (tempItemList2 != null && tempItemList2.Count > 0)
					{
						List<GoodsData> goodsDataRandomList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						if (null != goodsDataRandomList)
						{
							if (null == goodsDataList)
							{
								goodsDataList = goodsDataRandomList;
							}
							else
							{
								goodsDataList.AddRange(goodsDataRandomList);
							}
						}
					}
					result = goodsDataList;
				}
			}
			return result;
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x00259DAC File Offset: 0x00257FAC
		public static int ProcessFallByYaoShiWaBao(GameClient client, int fallID, int idYaoShi, int idXiangZi, out GoodsData retGoodsData, int forceBinding, int subMoney)
		{
			retGoodsData = null;
			int result;
			if (fallID <= 0)
			{
				result = -3000;
			}
			else
			{
				int maxFallCount = 1;
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == gallGoodsItemList)
				{
					result = -3100;
				}
				else
				{
					List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, maxFallCount, 1, 1.0);
					if (tempItemList2.Count <= 0)
					{
						result = -3200;
					}
					else
					{
						List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						if (!Global.CanAddGoodsNum(client, goodsDataList.Count))
						{
							result = -3300;
						}
						else if (1 == goodsDataList.Count)
						{
							retGoodsData = goodsDataList[0];
							bool usedBinding = false;
							bool usedTimeLimited = false;
							bool myUseBinding = false;
							if (idXiangZi >= 0 && !GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, idXiangZi, 1, false, out usedBinding, out usedTimeLimited, false))
							{
								result = -400;
							}
							else if (idYaoShi >= 0 && !GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, idYaoShi, 1, false, out usedBinding, out usedTimeLimited, false))
							{
								result = -500;
							}
							else
							{
								if (!myUseBinding)
								{
									myUseBinding = usedBinding;
								}
								if (subMoney > 0)
								{
									myUseBinding = false;
								}
								retGoodsData.Binding = Math.Max(forceBinding, myUseBinding ? 1 : 0);
								int goodsDbID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, retGoodsData.GoodsID, retGoodsData.GCount, retGoodsData.Quality, retGoodsData.Props, retGoodsData.Forge_level, retGoodsData.Binding, 0, "", true, 1, "精雕细琢挖宝获取", retGoodsData.Endtime, retGoodsData.AddPropIndex, retGoodsData.BornIndex, retGoodsData.Lucky, retGoodsData.Strong, 0, 0, 0, null, null, 0, true);
								retGoodsData.Id = goodsDbID;
								result = 100;
							}
						}
						else
						{
							result = -3400;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x00259FF4 File Offset: 0x002581F4
		public static void CreateGoodsBaseFallID(GameClient client, int fallID, int nMaxCount)
		{
			if (fallID > 0)
			{
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null != gallGoodsItemList)
				{
					List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, nMaxCount, 1, 1.0);
					if (tempItemList2.Count > 0)
					{
						List<GoodsData> goodsDataList = null;
						goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						if (goodsDataList != null)
						{
							List<GoodsPackItem> goodsPackItemList = new List<GoodsPackItem>();
							Dictionary<string, bool> gridDict = new Dictionary<string, bool>();
							for (int i = 0; i < goodsDataList.Count; i++)
							{
								List<GoodsData> oneGoodsDataList = new List<GoodsData>();
								oneGoodsDataList.Add(goodsDataList[i]);
								GoodsPackItem goodsPackItem = new GoodsPackItem
								{
									AutoID = GameManager.GoodsPackMgr.GetNextAutoID(),
									GoodsPackID = fallID,
									OwnerRoleID = client.ClientData.RoleID,
									OwnerRoleName = client.ClientData.RoleName,
									GoodsPackType = 0,
									ProduceTicks = TimeUtil.NOW(),
									LockedRoleID = -1,
									GoodsDataList = oneGoodsDataList,
									TeamRoleIDs = null,
									MapCode = client.ClientData.MapCode,
									CopyMapID = client.ClientData.CopyMapID,
									KilledMonsterName = null,
									BelongTo = 1,
									FallLevel = 0,
									TeamID = -1
								};
								goodsPackItem.FallPoint = GameManager.GoodsPackMgr.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, client.ClientData.MapCode, gridDict, new Point((double)((int)client.CurrentGrid.X), (double)((int)client.CurrentGrid.Y)), client.ClientData.CopyMapID, client);
								goodsPackItemList.Add(goodsPackItem);
								lock (GameManager.GoodsPackMgr.GoodsPackDict)
								{
									GameManager.GoodsPackMgr.GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
								}
								for (int j = 0; j < goodsPackItemList.Count; j++)
								{
									GameManager.GoodsPackMgr.ProcessGoodsPackItem(client, client, goodsPackItemList[i], 1);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x0025A26C File Offset: 0x0025846C
		public static int ProcessActivityAward(GameClient client, int fallID, int maxFallCount, int binding, string sMsg, List<GoodsData> goodsDataList)
		{
			int result;
			if (fallID <= 0)
			{
				result = -10;
			}
			else
			{
				List<FallGoodsItem> gallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == gallGoodsItemList)
				{
					result = -12;
				}
				else
				{
					List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(gallGoodsItemList, maxFallCount, 1, 1.0);
					if (tempItemList2.Count <= 0)
					{
						result = -13;
					}
					else
					{
						List<GoodsData> goodsDataLists = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						if (!Global.CanAddGoodsNum(client, goodsDataLists.Count))
						{
							result = -14;
						}
						else
						{
							for (int i = 0; i < goodsDataLists.Count; i++)
							{
								goodsDataList.Add(goodsDataLists[i]);
							}
							result = 1;
						}
					}
				}
			}
			return result;
		}
	}
}
