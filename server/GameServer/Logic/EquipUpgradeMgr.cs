using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006B9 RID: 1721
	public class EquipUpgradeMgr
	{
		// Token: 0x0600205F RID: 8287 RVA: 0x001BDE0C File Offset: 0x001BC00C
		public static int ProcessUpgrade(GameClient client, int goodsDbID)
		{
			GoodsData goodsData = Global.GetGoodsByDbID(client, goodsDbID);
			int result;
			if (null == goodsData)
			{
				result = -1;
			}
			else if (goodsData.Site != 0)
			{
				result = -9998;
			}
			else if (goodsData.Using > 0)
			{
				result = -9999;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					result = -2;
				}
				else
				{
					int categoriy = systemGoods.GetIntValue("Categoriy", -1);
					if (categoriy < 0 || categoriy >= 49)
					{
						result = -3;
					}
					else
					{
						int suitID = systemGoods.GetIntValue("SuitID", -1);
						int toOccupation = Global.GetMainOccupationByGoodsID(goodsData.GoodsID);
						int toSex = systemGoods.GetIntValue("ToSex", -1);
						int newGoodsID = -1;
						Dictionary<int, SystemXmlItem> dict = GameManager.SystemGoods.SystemXmlItemDict;
						foreach (SystemXmlItem goodsItem in dict.Values)
						{
							int nCmpCategoriy = goodsItem.GetIntValue("Categoriy", -1);
							int nCmpSuitID = goodsItem.GetIntValue("SuitID", -1);
							int nCmpToSex = goodsItem.GetIntValue("ToSex", -1);
							int nCmpOccu = goodsItem.GetIntValue("MainOccupation", -1);
							if (nCmpCategoriy == categoriy && nCmpSuitID == suitID + 1 && nCmpOccu == toOccupation && nCmpToSex == toSex)
							{
								newGoodsID = goodsItem.GetIntValue("ID", -1);
								break;
							}
						}
						if (newGoodsID < 0)
						{
							result = -5;
						}
						else
						{
							int nextSuitID = suitID + 1;
							SystemXmlItem systemEquipUpgradeItem = EquipUpgradeCacheMgr.GetEquipUpgradeCacheItem(categoriy, nextSuitID);
							if (null == systemEquipUpgradeItem)
							{
								result = -4;
							}
							else
							{
								int needGoodsID = systemEquipUpgradeItem.GetIntValue("NeedGoodsID", -1);
								if (needGoodsID < 0)
								{
									result = -6;
								}
								else
								{
									int needGoodsNum = systemEquipUpgradeItem.GetIntValue("GoodsNum", -1);
									if (needGoodsNum <= 0)
									{
										result = -7;
									}
									else
									{
										int needJiFen = systemEquipUpgradeItem.GetIntValue("JiFen", -1);
										if (needJiFen <= 0)
										{
											result = -8;
										}
										else
										{
											int succeed = systemEquipUpgradeItem.GetIntValue("Succeed", -1) * 100;
											if (succeed < 0)
											{
												result = -9;
											}
											else
											{
												int newGoodsBinding = goodsData.Binding;
												bool usedBinding = false;
												bool usedTimeLimited = false;
												if (Global.GetTotalGoodsCountByID(client, needGoodsID) < needGoodsNum)
												{
													result = -10;
												}
												else
												{
													int jiFen = GameManager.ClientMgr.GetZhuangBeiJiFenValue(client);
													if (jiFen < needJiFen)
													{
														result = -11;
													}
													else if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needGoodsID, needGoodsNum, false, out usedBinding, out usedTimeLimited, false))
													{
														result = -12;
													}
													else if (Global.GetRandomNumber(0, 10001) > succeed)
													{
														result = -1000;
													}
													else
													{
														GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(client, -needJiFen, true, true);
														if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsDbID, false, false))
														{
															result = -14;
														}
														else
														{
															int currentStrong = Math.Max(goodsData.Strong, 0);
															int currentLucky = 0;
															int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, newGoodsID, 1, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, false, 1, "装备进阶", "1900-01-01 12:00:00", goodsData.AddPropIndex, goodsData.BornIndex, currentLucky, currentStrong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, goodsData.WashProps, null, 0, true);
															if (dbRet < 0)
															{
																result = -2000;
															}
															else
															{
																Global.BroadcastEquipUpgradeOk(client, goodsData.GoodsID, newGoodsID, goodsData.Quality, goodsData.Forge_level);
																result = dbRet;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
