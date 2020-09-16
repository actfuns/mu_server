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
	
	public class WingUpgradeCmdProcessor : ICmdProcessor
	{
		
		private WingUpgradeCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(609, 2, this);
		}

		
		public static WingUpgradeCmdProcessor getInstance()
		{
			return WingUpgradeCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 609;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			int nUpWingMode = Global.SafeConvertToInt32(cmdParams[1]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					-3,
					nRoleID,
					0,
					0
				});
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else if (client.ClientData.MyWingData.WingID >= MUWingsManager.MaxWingID)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					-8,
					nRoleID,
					0,
					0
				});
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				SystemXmlItem upStarXmlItem = MUWingsManager.GetWingUPCacheItem(client.ClientData.MyWingData.WingID + 1);
				if (null == upStarXmlItem)
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-3,
						nRoleID,
						0,
						0
					});
					client.sendCmd(nID, strCmd, false);
					result = true;
				}
				else
				{
					SystemXmlItem upStarXmlItem2 = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel + 1);
					if (null != upStarXmlItem2)
					{
						string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-3,
							nRoleID,
							0,
							0
						});
						client.sendCmd(nID, strCmd, false);
						result = true;
					}
					else
					{
						string strCostList = "";
						if (0 == nUpWingMode)
						{
							string strReqItemID = upStarXmlItem.GetStringValue("NeedGoods");
							string[] itemParams = strReqItemID.Split(new char[]
							{
								','
							});
							if (itemParams == null || itemParams.Length != 2)
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
								return true;
							}
							int originGoodsID = Convert.ToInt32(itemParams[0]);
							int originGoodsNum = Convert.ToInt32(itemParams[1]);
							if (originGoodsID <= 0 || originGoodsNum <= 0)
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
								return true;
							}
							GoodsReplaceResult replaceRet = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, originGoodsID);
							if (replaceRet == null || replaceRet.TotalGoodsCnt() < originGoodsNum)
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-4,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
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
										string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											-5,
											nRoleID,
											0,
											0
										});
										client.sendCmd(nID, strCmd, false);
										return true;
									}
									GoodsData goodsDataLog = new GoodsData
									{
										GoodsID = item.GoodsID,
										GCount = realCostCnt
									};
									strCostList = EventLogManager.NewGoodsDataPropString(goodsDataLog);
								}
							}
						}
						else
						{
							int nReqZuanShi = upStarXmlItem.GetIntValue("NeedZuanShi", -1);
							if (nReqZuanShi <= 0)
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
								return true;
							}
							int oldUserMoney = client.ClientData.UserMoney;
							int oldUserGlod = client.ClientData.Gold;
							if (client.ClientData.UserMoney < nReqZuanShi && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, nReqZuanShi))
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-6,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nReqZuanShi, "翅膀进阶", true, true, false, DaiBiSySType.ChiBangShengJie))
							{
								string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-7,
									nRoleID,
									0,
									0
								});
								client.sendCmd(nID, strCmd, false);
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
						int nLuckOne = upStarXmlItem.GetIntValue("LuckyOne", -1);
						int nLuckyTwo = upStarXmlItem.GetIntValue("LuckyTwo", -1);
						int nLuckTwoRate = (int)(upStarXmlItem.GetDoubleValue("LuckyTwoRate") * 100.0);
						int nNextWingID = client.ClientData.MyWingData.WingID;
						int nNextJinJieFailedNum = client.ClientData.MyWingData.JinJieFailedNum;
						int nNextStarLevel = client.ClientData.MyWingData.ForgeLevel;
						int nNextStarExp = client.ClientData.MyWingData.StarExp;
						int nOldWingID = client.ClientData.MyWingData.WingID;
						int nOldJinJieFailedNum = client.ClientData.MyWingData.JinJieFailedNum;
						int nOldStarLevel = client.ClientData.MyWingData.ForgeLevel;
						int nOldStarExp = client.ClientData.MyWingData.StarExp;
						if (nLuckOne + client.ClientData.MyWingData.JinJieFailedNum < nLuckyTwo)
						{
							nNextJinJieFailedNum++;
						}
						else if (nLuckOne + client.ClientData.MyWingData.JinJieFailedNum < 110000)
						{
							int nRandNum = Global.GetRandomNumber(0, 100);
							if (nRandNum < nLuckTwoRate)
							{
								nNextWingID++;
								nNextJinJieFailedNum = 0;
								nNextStarLevel = 0;
								nNextStarExp = 0;
							}
							else
							{
								nNextJinJieFailedNum++;
							}
						}
						else
						{
							nNextWingID++;
							nNextJinJieFailedNum = 0;
							nNextStarLevel = 0;
							nNextStarExp = 0;
						}
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingSuitStarTimes));
						int iRet = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, nNextWingID, nNextJinJieFailedNum, nNextStarLevel, nNextStarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
						if (iRet < 0)
						{
							string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-3,
								nRoleID,
								0,
								0
							});
							client.sendCmd(nID, strCmd, false);
							result = true;
						}
						else
						{
							string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								0,
								nRoleID,
								nNextWingID,
								nNextJinJieFailedNum
							});
							client.sendCmd(nID, strCmd, false);
							client.ClientData.MyWingData.JinJieFailedNum = nNextJinJieFailedNum;
							if (client.ClientData.MyWingData.WingID != nNextWingID)
							{
								if (1 == client.ClientData.MyWingData.Using)
								{
									MUWingsManager.UpdateWingDataProps(client, false);
								}
								bool oldWingLingYuOpened = GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false);
								client.ClientData.MyWingData.WingID = nNextWingID;
								client.ClientData.MyWingData.ForgeLevel = 0;
								client.ClientData.MyWingData.StarExp = 0;
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
								if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing) || client._IconStateMgr.CheckReborn(client))
								{
									client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
									client._IconStateMgr.SendIconStateToClient(client);
								}
								EventLogManager.AddWingStarEvent(client, 2, 0, nOldStarLevel, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, strCostList);
							}
							EventLogManager.AddWingUpgradeEvent(client, nUpWingMode, nOldJinJieFailedNum, client.ClientData.MyWingData.JinJieFailedNum, nOldWingID, client.ClientData.MyWingData.WingID, nOldStarLevel, client.ClientData.MyWingData.ForgeLevel, nOldStarExp, client.ClientData.MyWingData.StarExp, strCostList);
							ProcessTask.ProcessRoleTaskVal(client, TaskTypes.WingIDLevel, -1);
							result = true;
						}
					}
				}
			}
			return result;
		}

		
		private static WingUpgradeCmdProcessor instance = new WingUpgradeCmdProcessor();
	}
}
