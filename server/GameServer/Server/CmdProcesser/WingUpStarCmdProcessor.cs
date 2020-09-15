using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Goods;
using GameServer.Logic.MUWings;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008AE RID: 2222
	public class WingUpStarCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003DA7 RID: 15783 RVA: 0x0034ACA0 File Offset: 0x00348EA0
		private WingUpStarCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(608, 2, this);
		}

		// Token: 0x06003DA8 RID: 15784 RVA: 0x0034ACC0 File Offset: 0x00348EC0
		public static WingUpStarCmdProcessor getInstance()
		{
			return WingUpStarCmdProcessor.instance;
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x0034ACD8 File Offset: 0x00348ED8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			int nUpStarMode = Global.SafeConvertToInt32(cmdParams[1]);
			string strCostList = "";
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
				client.sendCmd<SCWingStarUp>(608, scData, false);
				result = true;
			}
			else
			{
				SystemXmlItem upStarXmlItem = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel + 1);
				if (null == upStarXmlItem)
				{
					SCWingStarUp scData = new SCWingStarUp(-23, nRoleID, 0, 0);
					client.sendCmd<SCWingStarUp>(608, scData, false);
					result = true;
				}
				else
				{
					string strWingShengXing = GameManager.systemParamsList.GetParamValueByName("WingShengXing");
					if ("" == strWingShengXing)
					{
						SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
						client.sendCmd<SCWingStarUp>(608, scData, false);
						result = true;
					}
					else
					{
						string[] wingShengXing = strWingShengXing.Split(new char[]
						{
							','
						});
						if (3 != wingShengXing.Length)
						{
							SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
							client.sendCmd<SCWingStarUp>(608, scData, false);
							result = true;
						}
						else
						{
							int nAddExp = 0;
							int nPowRate;
							if (0 == nUpStarMode)
							{
								nPowRate = (int)(Convert.ToDouble(wingShengXing[0]) * 100.0);
								nAddExp = Convert.ToInt32(upStarXmlItem.GetIntValue("GoodsExp", -1));
							}
							else
							{
								nPowRate = (int)(Convert.ToDouble(wingShengXing[1]) * 100.0);
								nAddExp = Convert.ToInt32(upStarXmlItem.GetIntValue("ZuanShiExp", -1));
							}
							int nRandNum = Global.GetRandomNumber(0, 100);
							if (nRandNum < nPowRate)
							{
								nAddExp *= Convert.ToInt32(wingShengXing[2]);
							}
							int nUpStarReqExp = upStarXmlItem.GetIntValue("StarExp", -1);
							int nOldStarLevel = client.ClientData.MyWingData.ForgeLevel;
							int nNextStarLevel = client.ClientData.MyWingData.ForgeLevel;
							int nNextStarExp = 0;
							if (client.ClientData.MyWingData.StarExp + nAddExp >= nUpStarReqExp)
							{
								if (nNextStarLevel < MUWingsManager.MaxWingEnchanceLevel)
								{
									nNextStarLevel++;
									nNextStarExp = client.ClientData.MyWingData.StarExp + nAddExp - nUpStarReqExp;
									while (nNextStarLevel < MUWingsManager.MaxWingEnchanceLevel)
									{
										SystemXmlItem nextStarXmlItem = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, nNextStarLevel + 1);
										if (null != upStarXmlItem)
										{
											int nNextUpStarReqExp = nextStarXmlItem.GetIntValue("StarExp", -1);
											if (nNextStarExp >= nNextUpStarReqExp)
											{
												nNextStarLevel++;
												nNextStarExp -= nNextUpStarReqExp;
												continue;
											}
										}
										break;
									}
								}
								else
								{
									nNextStarExp = nUpStarReqExp;
								}
							}
							else
							{
								nNextStarExp = client.ClientData.MyWingData.StarExp + nAddExp;
							}
							if (0 == nUpStarMode)
							{
								string strReqItemID = upStarXmlItem.GetStringValue("NeedGoods");
								string[] itemParams = strReqItemID.Split(new char[]
								{
									','
								});
								if (itemParams == null || itemParams.Length != 2)
								{
									SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								int originGoodsID = Convert.ToInt32(itemParams[0]);
								int originGoodsNum = Convert.ToInt32(itemParams[1]);
								if (originGoodsID <= 0 || originGoodsNum <= 0)
								{
									SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								GoodsReplaceResult replaceRet = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, originGoodsID);
								if (replaceRet == null || replaceRet.TotalGoodsCnt() < originGoodsNum)
								{
									SCWingStarUp scData = new SCWingStarUp(-4, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								List<GoodsReplaceResult.ReplaceItem> realCostList = new List<GoodsReplaceResult.ReplaceItem>();
								realCostList.AddRange(replaceRet.BindList);
								realCostList.AddRange(replaceRet.UnBindList);
								realCostList.Add(replaceRet.OriginBindGoods);
								realCostList.Add(replaceRet.OriginUnBindGoods);
								int stillNeedCnt = originGoodsNum;
								foreach (GoodsReplaceResult.ReplaceItem item in realCostList)
								{
									if (item.GoodsCnt > 0)
									{
										int realCostCnt = Math.Min(stillNeedCnt, item.GoodsCnt);
										if (realCostCnt <= 0)
										{
											break;
										}
										bool bUsedBinding = false;
										bool bUsedTimeLimited = false;
										bool bFailed = false;
										if (item.IsBind)
										{
											if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out bUsedBinding, out bUsedTimeLimited, false))
											{
												bFailed = true;
											}
										}
										else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out bUsedBinding, out bUsedTimeLimited, false))
										{
											bFailed = true;
										}
										stillNeedCnt -= realCostCnt;
										if (bFailed)
										{
											SCWingStarUp scData = new SCWingStarUp(-5, nRoleID, 0, 0);
											client.sendCmd<SCWingStarUp>(608, scData, false);
											return true;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = item.GoodsID,
											GCount = realCostCnt
										};
										strCostList = EventLogManager.NewGoodsDataPropString(goodsData);
									}
								}
							}
							else
							{
								int nReqZuanShi = upStarXmlItem.GetIntValue("NeedZuanShi", -1);
								if (nReqZuanShi <= 0)
								{
									SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								if (client.ClientData.UserMoney < nReqZuanShi)
								{
									SCWingStarUp scData = new SCWingStarUp(-6, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								int oldUserMoney = client.ClientData.UserMoney;
								int oldUserGlod = client.ClientData.Gold;
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nReqZuanShi, "翅膀升星", true, true, false, DaiBiSySType.ChiBangShengXing))
								{
									SCWingStarUp scData = new SCWingStarUp(-7, nRoleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, scData, false);
									return true;
								}
								strCostList = EventLogManager.NewResPropString(ResLogType.FristBindZuanShi, new object[]
								{
									-nReqZuanShi,
									oldUserGlod,
									client.ClientData.Gold,
									oldUserMoney,
									client.ClientData.UserMoney
								});
							}
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingSuitStarTimes));
							int iRet = MUWingsManager.WingUpStarDBCommand(client, client.ClientData.MyWingData.DbID, nNextStarLevel, nNextStarExp);
							if (iRet < 0)
							{
								SCWingStarUp scData = new SCWingStarUp(-3, nRoleID, 0, 0);
								client.sendCmd<SCWingStarUp>(608, scData, false);
								result = true;
							}
							else
							{
								SCWingStarUp scData = new SCWingStarUp(0, nRoleID, nNextStarLevel, nNextStarExp);
								client.sendCmd<SCWingStarUp>(608, scData, false);
								client.ClientData.MyWingData.StarExp = nNextStarExp;
								if (client.ClientData.MyWingData.ForgeLevel != nNextStarLevel)
								{
									if (1 == client.ClientData.MyWingData.Using)
									{
										MUWingsManager.UpdateWingDataProps(client, false);
									}
									bool oldWingLingYuOpened = GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false);
									client.ClientData.MyWingData.ForgeLevel = nNextStarLevel;
									GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingLevel));
									bool newWingLingYuOpened = GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false);
									if (!oldWingLingYuOpened && newWingLingYuOpened)
									{
										LingYuManager.InitAsOpened(client);
									}
									if (1 == client.ClientData.MyWingData.Using)
									{
										MUWingsManager.UpdateWingDataProps(client, true);
										ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
									}
									if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
										client._IconStateMgr.SendIconStateToClient(client);
									}
								}
								EventLogManager.AddWingStarEvent(client, nUpStarMode, nAddExp, nOldStarLevel, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, strCostList);
								ProcessTask.ProcessRoleTaskVal(client, TaskTypes.WingIDLevel, -1);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040047BF RID: 18367
		private static WingUpStarCmdProcessor instance = new WingUpStarCmdProcessor();
	}
}
