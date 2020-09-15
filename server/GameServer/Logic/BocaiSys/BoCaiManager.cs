using System;
using System.Collections.Generic;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200007A RID: 122
	internal class BoCaiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x0001E650 File Offset: 0x0001C850
		public static BoCaiManager getInstance()
		{
			return BoCaiManager.instance;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0001E668 File Offset: 0x0001C868
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0001E67C File Offset: 0x0001C87C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0001E690 File Offset: 0x0001C890
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0001E6A4 File Offset: 0x0001C8A4
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2082, 4, 4, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2083, 2, 2, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2085, 1, 1, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2086, 4, 4, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			this.NotifyEnterHandler1 = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 0,
				EventType = 10039,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			this.NotifyEnterHandler2 = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 0,
				EventType = 10040,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10039, this.NotifyEnterHandler1);
			KFCallManager.MsgSource.registerListener(10040, this.NotifyEnterHandler2);
			BoCaiCaiShuZi.GetInstance().Init();
			BoCaiCaiDaXiao.GetInstance().Init();
			BoCaiShopManager.GetInstance().Init();
			return true;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			try
			{
				switch (msg.KuaFuEventType)
				{
				case 10039:
				{
					KFStageData data = msg.Get<KFStageData>();
					if (null != data)
					{
						if (data.BoCaiType == BoCaiTypeEnum.Bocai_CaiShuzi)
						{
							BoCaiCaiShuZi.GetInstance().SetStageData(data, true);
						}
						else if (data.BoCaiType == BoCaiTypeEnum.Bocai_Dice)
						{
							BoCaiCaiDaXiao.GetInstance().SetStageData(data, true);
						}
					}
					else
					{
						BoCaiCaiDaXiao.GetInstance().SetStageData(data, true);
						BoCaiCaiShuZi.GetInstance().SetStageData(data, true);
					}
					break;
				}
				case 10040:
				{
					OpenLottery data2 = msg.Get<OpenLottery>();
					if (null != data2)
					{
						if (data2.BocaiType == 2)
						{
							BoCaiCaiShuZi.GetInstance().SetOpenLotteryData(data2, false, false);
						}
						else if (data2.BocaiType == 1)
						{
							BoCaiCaiDaXiao.GetInstance().SetOpenLotteryData(data2, false, true);
						}
					}
					else
					{
						BoCaiCaiDaXiao.GetInstance().SetOpenLotteryData(data2, false, true);
						BoCaiCaiShuZi.GetInstance().SetOpenLotteryData(data2, false, false);
					}
					break;
				}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]跨服消息{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0001E934 File Offset: 0x0001CB34
		public bool showdown()
		{
			KFCallManager.MsgSource.removeListener(10039, this.NotifyEnterHandler1);
			KFCallManager.MsgSource.removeListener(10040, this.NotifyEnterHandler2);
			return true;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0001E974 File Offset: 0x0001CB74
		public List<OpenLottery> GetNewOpenLottery10(int type)
		{
			try
			{
				ReturnValue<List<OpenLottery>> Data = TcpCall.KFBoCaiManager.GetOpenLottery(0, (long)type, false);
				if (Data.IsReturn)
				{
					return Data.Value;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0001E9E0 File Offset: 0x0001CBE0
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (nID == 2082)
				{
					BuyBoCaiResult mgsData = new BuyBoCaiResult();
					int BocaiType = Convert.ToInt32(cmdParams[1]);
					int BuyNum = Convert.ToInt32(cmdParams[2]);
					string strBuyVal = cmdParams[3];
					mgsData.BocaiType = BocaiType;
					if (2 == BocaiType)
					{
						this.BuyCaiShuzi(client, BuyNum, strBuyVal, ref mgsData);
					}
					else if (1 == BocaiType)
					{
						this.BuyCaiDaXiao(client, BuyNum, strBuyVal, ref mgsData);
					}
					else
					{
						mgsData.Info = 1;
					}
					client.sendCmd<BuyBoCaiResult>(nID, mgsData, false);
				}
				else if (nID == 2083)
				{
					GetBoCaiResult mgsData2 = new GetBoCaiResult();
					this.GetBoCai(client, nID, cmdParams, ref mgsData2);
					client.sendCmd<GetBoCaiResult>(nID, mgsData2, false);
				}
				else if (nID == 2086)
				{
					client.sendCmd(nID, this.BuyItem(client, nID, cmdParams), false);
				}
				else if (nID == 2085)
				{
					this.GetShopInfo(client, nID, client.ClientData.RoleID);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0001EB48 File Offset: 0x0001CD48
		private void BuyCaiShuzi(GameClient client, int BuyNum, string strBuyVal, ref BuyBoCaiResult mgsData)
		{
			try
			{
				List<int> buyList;
				BoCaiHelper.String2ListInt(strBuyVal, out buyList);
				if (buyList.Count != 5 || BuyNum < 1)
				{
					mgsData.Info = 2;
					LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩] BuyCaiShuzi 购买内容 {0}, BuyNum={1} ", strBuyVal, BuyNum), null, true);
				}
				else
				{
					for (int i = 0; i < buyList.Count; i++)
					{
						if (buyList[i] > 9 || buyList[i] < 0)
						{
							mgsData.Info = 2;
							return;
						}
					}
					if (!BoCaiCaiShuZi.GetInstance().IsCanBuy())
					{
						mgsData.Info = 7;
					}
					else
					{
						ReturnValue<bool> msgData = TcpCall.KFBoCaiManager.IsCanBuy(2, strBuyVal, BuyNum, BoCaiCaiShuZi.GetInstance().GetDataPeriods());
						if (!msgData.IsReturn)
						{
							mgsData.Info = 8;
						}
						else if (!msgData.Value)
						{
							mgsData.Info = 5;
						}
						else
						{
							int XiaoHaoDaiBi = BoCaiCaiShuZi.GetInstance().GetXiaoHaoDaiBi();
							if (XiaoHaoDaiBi < 1)
							{
								mgsData.Info = 3;
								LogManager.WriteLog(LogTypes.Error, "[ljl_博彩]XiaoHaoDaiBi /GuDingLeiXing<1", null, true);
							}
							else
							{
								int useItemNum = BuyNum * XiaoHaoDaiBi;
								if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, useItemNum))
								{
									mgsData.Info = 4;
								}
								else
								{
									int allNum = BuyNum;
									BuyBoCai2SDB DbData = BoCaiCaiShuZi.GetInstance().BuyBocai(client, BuyNum, strBuyVal, ref allNum);
									if (null == DbData)
									{
										mgsData.Info = 8;
										BoCaiCaiShuZi.GetInstance().BuyBocai(client, -BuyNum, strBuyVal, ref allNum);
										LogManager.WriteLog(LogTypes.Error, "[ljl_博彩]BoCaiCaiShuZi.GetInstance().BuyBocai err", null, true);
									}
									else if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, useItemNum))
									{
										mgsData.Info = 4;
									}
									else
									{
										ReturnValue<bool> msgData2 = TcpCall.KFBoCaiManager.BuyBoCai(new KFBuyBocaiData
										{
											BocaiType = 2,
											RoleID = DbData.m_RoleID,
											RoleName = DbData.m_RoleName,
											ZoneID = DbData.ZoneID,
											ServerID = DbData.ServerId,
											BuyNum = DbData.BuyNum,
											BuyValue = DbData.strBuyValue
										});
										if (!msgData2.IsReturn)
										{
											mgsData.Info = 8;
										}
										else if (!msgData2.Value)
										{
											GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买失败扣物品成功中心2次通信", client.ClientData.RoleName, client.ClientData.RoleName, "减少", useItemNum, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
											mgsData.Info = 5;
										}
										else
										{
											DbData.BuyNum = allNum;
											BoCaiBuy2DBList.getInstance().AddData(DbData, useItemNum, true);
											BoCaiCaiShuZi.GetInstance().CopyBuyList(out mgsData.ItemList, DbData.m_RoleID);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0001EE98 File Offset: 0x0001D098
		private void BuyCaiDaXiao(GameClient client, int BuyNum, string strBuyVal, ref BuyBoCaiResult mgsData)
		{
			try
			{
				int value = Convert.ToInt32(strBuyVal);
				if (1 > value || value > 3 || BuyNum < 1)
				{
					mgsData.Info = 2;
					LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩]BuyCaiDaXiao 购买内容 {0}, BuyNum={1} ", strBuyVal, BuyNum), null, true);
				}
				else if (!BoCaiCaiDaXiao.GetInstance().IsCanBuy())
				{
					mgsData.Info = 7;
				}
				else
				{
					ReturnValue<bool> msgData = TcpCall.KFBoCaiManager.IsCanBuy(1, strBuyVal, BuyNum + BoCaiCaiDaXiao.GetInstance().GetBuyNum(client.ClientData.RoleID), BoCaiCaiDaXiao.GetInstance().GetDataPeriods());
					if (!msgData.IsReturn)
					{
						mgsData.Info = 8;
					}
					else if (!msgData.Value)
					{
						mgsData.Info = 7;
					}
					else
					{
						int useItemNum = BoCaiCaiDaXiao.GetInstance().GetXiaoHaoDaiBi() * BuyNum;
						if (useItemNum < 1)
						{
							mgsData.Info = 3;
							LogManager.WriteLog(LogTypes.Error, "[ljl_博彩]XiaoHaoDaiBi /GuDingLeiXing<1", null, true);
						}
						else if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, useItemNum))
						{
							mgsData.Info = 4;
						}
						else
						{
							int allNum = BuyNum;
							BuyBoCai2SDB DbData = BoCaiCaiDaXiao.GetInstance().BuyBocai(client, BuyNum, strBuyVal, ref allNum);
							if (null == DbData)
							{
								mgsData.Info = 8;
								BoCaiCaiDaXiao.GetInstance().BuyBocai(client, -BuyNum, strBuyVal, ref allNum);
								LogManager.WriteLog(LogTypes.Error, "[ljl_博彩]BoCaiCaiDaXiao.GetInstance().BuyBocai err", null, true);
							}
							else if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, useItemNum))
							{
								mgsData.Info = 4;
							}
							else
							{
								ReturnValue<bool> msgData2 = TcpCall.KFBoCaiManager.BuyBoCai(new KFBuyBocaiData
								{
									BocaiType = 1,
									RoleID = DbData.m_RoleID,
									RoleName = DbData.m_RoleName,
									ZoneID = DbData.ZoneID,
									ServerID = DbData.ServerId,
									BuyNum = DbData.BuyNum,
									BuyValue = DbData.strBuyValue
								});
								if (!msgData2.IsReturn)
								{
									mgsData.Info = 8;
								}
								else if (!msgData2.Value)
								{
									GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买失败扣物品成功中心2次通信", client.ClientData.RoleName, client.ClientData.RoleName, "减少", useItemNum, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
									mgsData.Info = 5;
								}
								else
								{
									DbData.BuyNum = allNum;
									BoCaiBuy2DBList.getInstance().AddData(DbData, useItemNum, true);
									BoCaiCaiDaXiao.GetInstance().CopyBuyList(out mgsData.ItemList, DbData.m_RoleID);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0001F1A8 File Offset: 0x0001D3A8
		private void GetBoCai(GameClient client, int nID, string[] cmdParams, ref GetBoCaiResult mgsData)
		{
			try
			{
				int BocaiType = Convert.ToInt32(cmdParams[1]);
				mgsData.Info = 0;
				mgsData.BocaiType = BocaiType;
				if (2 == BocaiType)
				{
					FunctionSendManager.GetInstance().AddFunction(FunctionType.CaiShuZi, client.ClientData.RoleID);
					BoCaiCaiShuZi.GetInstance().OpenGetBoCai(client.ClientData.RoleID, ref mgsData);
				}
				else if (1 == BocaiType)
				{
					FunctionSendManager.GetInstance().AddFunction(FunctionType.CaiDaXiao, client.ClientData.RoleID);
					BoCaiCaiDaXiao.GetInstance().OpenGetBoCai(client.ClientData.RoleID, ref mgsData);
				}
				else
				{
					mgsData.Info = 1;
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0001F298 File Offset: 0x0001D498
		private string BuyItem(GameClient client, int nID, string[] cmdParams)
		{
			string msgInfo = "";
			try
			{
				if (GameManager.systemParamsList.GetParamValueIntByName("HuanLeDuiHuan", -1) < 1L)
				{
					return string.Format("{0}{1}", msgInfo, 7);
				}
				int ID = Convert.ToInt32(cmdParams[1]);
				int BuyNum = Convert.ToInt32(cmdParams[2]);
				string WuPinID = cmdParams[3];
				msgInfo = string.Format("{0}:{1}:{2}:", cmdParams[1], cmdParams[2], cmdParams[3]);
				DuiHuanShangChengConfig cfg = BoCaiConfigMgr.GetBoCaiShopConfig(ID, WuPinID);
				if (null == cfg)
				{
					return string.Format("{0}{1}", msgInfo, 14);
				}
				int useItemNum = cfg.DaiBiJiaGe * BuyNum;
				GoodsData Goods = GlobalNew.ParseGoodsData(WuPinID);
				if (null == Goods)
				{
					return string.Format("{0}{1}", msgInfo, 14);
				}
				if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, useItemNum))
				{
					return string.Format("{0}{1}", msgInfo, 4);
				}
				if (!Global.CanAddGoods3(client, Goods.GoodsID, BuyNum, Goods.Binding, "1900-01-01 12:00:00", true))
				{
					return string.Format("{0}{1}", msgInfo, 13);
				}
				KFBoCaiShopDB item = new KFBoCaiShopDB();
				item.BuyNum = BuyNum;
				item.ID = ID;
				item.WuPinID = WuPinID;
				item.RoleID = client.ClientData.RoleID;
				item.Periods = Convert.ToInt32(TimeUtil.NowDataTimeString("yyMMdd"));
				if (cfg.MeiRiShangXianDan > -1)
				{
					if (BuyNum > cfg.MeiRiShangXianDan)
					{
						return string.Format("{0}{1}", msgInfo, 17);
					}
					if (!BoCaiShopManager.GetInstance().CanBuyItem(item, cfg.MeiRiShangXianDan))
					{
						return string.Format("{0}{1}", msgInfo, 18);
					}
				}
				if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, useItemNum))
				{
					return string.Format("{0}{1}", msgInfo, 4);
				}
				int ret = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, Goods.GoodsID, BuyNum, Goods.Quality, Goods.Props, Goods.Forge_level, Goods.Binding, Goods.Site, Goods.Jewellist, true, 1, "博彩商店购买", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
				LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩] 博彩商店购买 放在背包ret={1}，RoleID={0},WuPinID={2},name={3}", new object[]
				{
					client.ClientData.RoleID,
					ret,
					WuPinID,
					client.ClientData.RoleName
				}), null, true);
				return string.Format("{0}{1}", msgInfo, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return string.Format("{0}{1}", msgInfo, 100);
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0001F5E0 File Offset: 0x0001D7E0
		private void GetShopInfo(GameClient client, int nID, int roleID)
		{
			BoCaiShopInfo msgData = new BoCaiShopInfo();
			try
			{
				msgData.Info = 0;
				BoCaiShopManager.GetInstance().GetSelfBuyData(roleID, ref msgData);
			}
			catch (Exception ex)
			{
				msgData.Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<BoCaiShopInfo>(nID, msgData, false);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0001F654 File Offset: 0x0001D854
		public bool GetBuyList2DB(int type, long DataPeriods, out List<BuyBoCai2SDB> ItemList, int msgType = 1)
		{
			ItemList = new List<BuyBoCai2SDB>();
			try
			{
				GetBuyBoCaiList DBData = Global.sendToDB<GetBuyBoCaiList, string>(2083, string.Format("{2},{1},{0}", type, DataPeriods, msgType), 0);
				if (DBData == null || !DBData.Flag)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]GetBuyList2DB DBData={0}, type={1}, DataPeriods={2}", null == DBData, type, DataPeriods), null, true);
					return false;
				}
				if (null == DBData.ItemList)
				{
					DBData.ItemList = new List<BuyBoCai2SDB>();
				}
				ItemList.AddRange(DBData.ItemList);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0001F738 File Offset: 0x0001D938
		public bool GetOpenList2DB(int type, out GetOpenList DBData)
		{
			DBData = new GetOpenList();
			try
			{
				DBData = Global.sendToDB<GetOpenList, string>(2083, string.Format("{1},{0}", type, 3), 0);
				if (DBData == null || !DBData.Flag)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]GetOpenList2DB  DBData={0}, type={1}", null == DBData, type), null, true);
					return false;
				}
				if (null == DBData.ItemList)
				{
					DBData.ItemList = new List<OpenLottery>();
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001F808 File Offset: 0x0001DA08
		private bool ReturnItem(OpenLottery data, BuyBoCai2SDB buyItem)
		{
			try
			{
				buyItem.IsSend = true;
				BoCaiBuy2DBList.getInstance().AddData(buyItem, 0, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩]更新数据库标志失败，返回道具，没关系 线程会自动处理旧数据,{0},{1}", buyItem.m_RoleName, buyItem.DataPeriods), null, true);
					return false;
				}
				int returnNum = buyItem.BuyNum * data.XiaoHaoDaiBi;
				string strTitle = "猜大小";
				if (buyItem.BocaiType == 2)
				{
					strTitle = "猜数字";
				}
				string strIntro = string.Format("因系统维护原因导致{0}期{1}玩法没有正常开奖，系统将返还您当期下注的欢乐代币。", buyItem.DataPeriods, strTitle);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(returnNum)
				};
				return this.SendMail(buyItem, goodsData, strTitle, strIntro, returnNum, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0001F924 File Offset: 0x0001DB24
		public bool SendWinItem(OpenLottery data, BuyBoCai2SDB buyItem, double Rate, bool isSendMail, string winType)
		{
			try
			{
				buyItem.IsSend = true;
				if (!winType.Equals(buyItem.strBuyValue))
				{
					buyItem.IsWin = false;
					BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, true);
					return true;
				}
				int WinNum = (int)(Rate * (double)buyItem.BuyNum * (double)data.XiaoHaoDaiBi);
				string strTitle = "猜大小";
				string strIntro = string.Format("恭喜您在{0}期猜大小玩法中，获得欢乐代币{1}，系统将邮件的形式将您获取的欢乐代币返还与你。", buyItem.DataPeriods, WinNum);
				buyItem.IsWin = true;
				string strLog = string.Format("TYPE= {8},id={6},name={7}, {0}期开奖{1}赢得{2},自己购买{3},{4}注,info={5}", new object[]
				{
					data.DataPeriods,
					data.strWinNum,
					WinNum,
					buyItem.strBuyValue,
					buyItem.BuyNum,
					data.WinInfo,
					buyItem.m_RoleID,
					buyItem.m_RoleName,
					buyItem.BocaiType
				});
				BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩]更新数据库标志失败，不发奖励，没关系 会自动处理旧数据{0}", strLog), null, true);
					return WinNum < 1;
				}
				if (WinNum < 1)
				{
					return true;
				}
				GoodsData Goods = HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(WinNum);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					Goods
				};
				if (isSendMail)
				{
					return this.SendMail(buyItem, goodsData, strTitle, strIntro, WinNum, true);
				}
				GameClient client = GameManager.ClientMgr.FindClient(buyItem.m_RoleID);
				if (client != null && Global.CanAddGoods3(client, Goods.GoodsID, WinNum, Goods.Binding, "1900-01-01 12:00:00", true))
				{
					int ret = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, Goods.GoodsID, Goods.GCount, Goods.Quality, Goods.Props, Goods.Forge_level, Goods.Binding, Goods.Site, Goods.Jewellist, true, 1, "猜大小中奖", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩]放在背包ret={1}，{0}", strLog, ret), null, true);
					return true;
				}
				return this.SendMail(buyItem, goodsData, strTitle, strIntro, WinNum, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0001FBF0 File Offset: 0x0001DDF0
		public bool SendWinItem(OpenLottery data, BuyBoCai2SDB buyItem)
		{
			try
			{
				buyItem.IsSend = true;
				string[] files = data.WinInfo.Split(new char[]
				{
					','
				});
				List<int> winList;
				BoCaiHelper.String2ListInt(data.strWinNum, out winList);
				int No1Win = Convert.ToInt32(files[0]);
				int No2Win = Convert.ToInt32(files[1]);
				int No3Win = Convert.ToInt32(files[2]);
				List<int> selfBuy;
				BoCaiHelper.String2ListInt(buyItem.strBuyValue, out selfBuy);
				if (selfBuy.Count != winList.Count)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]selfBuy.Count != winList.Count DataPeriods={0}, type={1}, roleid={2}", data.DataPeriods, data.BocaiType, buyItem.m_RoleID), null, true);
					return false;
				}
				int sameNum = 0;
				for (int i = 0; i < selfBuy.Count; i++)
				{
					if (selfBuy[i] == winList[i])
					{
						sameNum++;
					}
				}
				int No = 0;
				int WinNum = 0;
				if (sameNum == 5)
				{
					No = 1;
					WinNum = No1Win * buyItem.BuyNum;
				}
				else if (sameNum == 4)
				{
					No = 2;
					WinNum = No2Win * buyItem.BuyNum;
				}
				else if (sameNum == 3)
				{
					No = 3;
					WinNum = No3Win * buyItem.BuyNum;
				}
				string strLog = string.Format("猜数字id={6},name={7}, {0}期开奖{1}赢得{2},自己购买{3},{4}注,info={5}", new object[]
				{
					data.DataPeriods,
					data.strWinNum,
					WinNum,
					buyItem.strBuyValue,
					buyItem.BuyNum,
					data.WinInfo,
					buyItem.m_RoleID,
					buyItem.m_RoleName
				});
				if (WinNum < 1)
				{
					buyItem.IsWin = false;
					BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, true);
					return true;
				}
				buyItem.IsWin = true;
				BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("[ljl_博彩]更新数据库标志失败，不发奖励，没关系 会自动处理旧数据{0}", strLog), null, true);
					return false;
				}
				string strTitle = "猜数字";
				string strIntro = string.Format("恭喜您在{0}期猜数字玩法中,中{2}等奖，获得欢乐代币{1}，系统将邮件的形式将您获取的欢乐代币返还与你。", buyItem.DataPeriods, WinNum, No);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(WinNum)
				};
				return this.SendMail(buyItem, goodsData, strTitle, strIntro, WinNum, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0001FEEC File Offset: 0x0001E0EC
		private bool SendMail(BuyBoCai2SDB buyItem, List<GoodsData> goodsData, string strTitle, string strIntro, int ItemNum, bool send = true)
		{
			try
			{
				string logTitle;
				if (buyItem.BocaiType == 2)
				{
					logTitle = "猜数字邮件";
				}
				else
				{
					logTitle = "猜大小邮件";
				}
				logTitle += (send ? "发奖" : "退回");
				if (Global.UseMailGivePlayerAward3(buyItem.m_RoleID, goodsData, strTitle, strIntro, 0, 0, 0))
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "欢乐代币", string.Format("{0}成功", logTitle), "系统", buyItem.m_RoleName, "增加", ItemNum, buyItem.ZoneID, buyItem.strUserID, -1, buyItem.ServerId, null);
					return true;
				}
				GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", string.Format("{0}失败", logTitle), buyItem.m_RoleName, buyItem.m_RoleName, "增加", ItemNum, buyItem.ZoneID, buyItem.strUserID, -1, buyItem.ServerId, "");
				LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]{2}失败 send email roleid={0}, num={1}, name={3}", new object[]
				{
					buyItem.m_RoleID,
					ItemNum,
					logTitle,
					buyItem.m_RoleName
				}), null, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00020074 File Offset: 0x0001E274
		public void OldtterySet(int BoCaiType, long DataPeriods)
		{
			try
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩]处理历史开奖 BoCaiType={0}, 处理界限 DataPeriods={1}", BoCaiType, DataPeriods), null, true);
				GetOpenList DBData;
				if (!BoCaiManager.getInstance().GetOpenList2DB(BoCaiType, out DBData))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]处理历史开奖 读库失败 GetOpenList2DB BoCaiType={0}, DataPeriods={1}", BoCaiType, DataPeriods), null, true);
				}
				else
				{
					foreach (OpenLottery item in DBData.ItemList)
					{
						OpenLottery Temp = item;
						if (BoCaiType != 1)
						{
							if (BoCaiType == 2)
							{
								if (item.DataPeriods >= Convert.ToInt64(string.Format("{0}1", TimeUtil.NowDataTimeString("yyMMdd"))))
								{
									continue;
								}
							}
						}
						if (string.IsNullOrEmpty(item.strWinNum) || string.IsNullOrEmpty(item.WinInfo))
						{
							ReturnValue<List<OpenLottery>> Data = TcpCall.KFBoCaiManager.GetOpenLottery(BoCaiType, item.DataPeriods, true);
							if (!Data.IsReturn)
							{
								return;
							}
							List<OpenLottery> kuafuList = Data.Value;
							if (kuafuList == null || kuafuList.Count < 1)
							{
								continue;
							}
							Temp = kuafuList[0];
						}
						if (Temp != null && Temp.DataPeriods >= 1L && Temp.XiaoHaoDaiBi >= 0)
						{
							Global.Send2DB<OpenLottery>(2084, Temp, 0);
							List<BuyBoCai2SDB> ItemList;
							if (!BoCaiManager.getInstance().GetBuyList2DB(BoCaiType, Temp.DataPeriods, out ItemList, 2))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]获取购买记录失败 BoCaiType={0},DataPeriods={1}", BoCaiType, Temp.DataPeriods), null, true);
							}
							else if (ItemList.Count < 1)
							{
								Temp.IsAward = true;
								Global.Send2DB<OpenLottery>(2084, Temp, 0);
								LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩]还有0人没处理 完成处理历史开奖 BoCaiType={0}, DataPeriods={1}", BoCaiType, Temp.DataPeriods), null, true);
								continue;
							}
							if (string.IsNullOrEmpty(Temp.strWinNum) || string.IsNullOrEmpty(Temp.WinInfo))
							{
								LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩] 没有开过奖处理历史退回{0}", Temp.DataPeriods), null, true);
								foreach (BuyBoCai2SDB buyItem in ItemList)
								{
									if (!this.ReturnItem(Temp, buyItem))
									{
										Temp.IsAward = false;
									}
								}
								if (Temp.IsAward)
								{
									Global.Send2DB<OpenLottery>(2084, Temp, 0);
								}
							}
							else
							{
								LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_博彩]处理历史开奖{0}", Temp.DataPeriods), null, true);
								foreach (BuyBoCai2SDB buyItem in ItemList)
								{
									if (buyItem.BocaiType == 1)
									{
										string winType;
										double Rate = BoCaiCaiDaXiao.GetInstance().CompensateRate(Temp.strWinNum, Temp.WinInfo, out winType);
										if (Rate < 1.0)
										{
											LogManager.WriteLog(LogTypes.Info, "[ljl_博彩] 开奖 赔率 < 1 ", null, true);
										}
										else if (!this.SendWinItem(Temp, buyItem, Rate, true, winType))
										{
											Temp.IsAward = false;
										}
									}
									else if (buyItem.BocaiType == 2)
									{
										if (!this.SendWinItem(Temp, buyItem))
										{
											Temp.IsAward = false;
										}
									}
									else
									{
										Temp.IsAward = false;
										LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_博彩]购买记录 类型不对 BoCaiType={0},DataPeriods={1}", BoCaiType, Temp.DataPeriods), null, true);
									}
								}
								if (Temp.IsAward)
								{
									Global.Send2DB<OpenLottery>(2084, Temp, 0);
								}
							}
						}
					}
					ReturnValue<List<OpenLottery>> KFData = TcpCall.KFBoCaiManager.GetOpenLottery(BoCaiType, DBData.MaxDataPeriods, false);
					if (KFData.IsReturn)
					{
						List<OpenLottery> KuaFuList = KFData.Value;
						if (null != KuaFuList)
						{
							foreach (OpenLottery item in KuaFuList)
							{
								Global.Send2DB<OpenLottery>(2084, item, 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x000205D0 File Offset: 0x0001E7D0
		public void BoCaiPriorityActivity(GameClient client)
		{
			BoCaiCaiShuZi.GetInstance().PriorityActivity(client);
			BoCaiCaiDaXiao.GetInstance().PriorityActivity(client);
		}

		// Token: 0x040002CE RID: 718
		private const int MaxCaiNum = 9;

		// Token: 0x040002CF RID: 719
		private const int MinCaiNum = 0;

		// Token: 0x040002D0 RID: 720
		public const string msgFlag = "True";

		// Token: 0x040002D1 RID: 721
		private static BoCaiManager instance = new BoCaiManager();

		// Token: 0x040002D2 RID: 722
		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler1 = null;

		// Token: 0x040002D3 RID: 723
		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler2 = null;
	}
}
