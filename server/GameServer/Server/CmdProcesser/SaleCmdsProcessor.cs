using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A8 RID: 2216
	public class SaleCmdsProcessor : ICmdProcessor
	{
		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06003D81 RID: 15745 RVA: 0x0034574C File Offset: 0x0034394C
		private TCPManager tcpMgr
		{
			get
			{
				return TCPManager.getInstance();
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06003D82 RID: 15746 RVA: 0x00345764 File Offset: 0x00343964
		private TCPOutPacketPool pool
		{
			get
			{
				return TCPManager.getInstance().TcpOutPacketPool;
			}
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06003D83 RID: 15747 RVA: 0x00345780 File Offset: 0x00343980
		private TCPClientPool tcpClientPool
		{
			get
			{
				return TCPManager.getInstance().tcpClientPool;
			}
		}

		// Token: 0x06003D84 RID: 15748 RVA: 0x0034579C File Offset: 0x0034399C
		public SaleCmdsProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		// Token: 0x06003D85 RID: 15749 RVA: 0x003457BC File Offset: 0x003439BC
		public static SaleCmdsProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new SaleCmdsProcessor(cmdID);
		}

		// Token: 0x06003D86 RID: 15750 RVA: 0x003457D4 File Offset: 0x003439D4
		private bool CanUseMarket(GameClient client)
		{
			bool result;
			try
			{
				if (Global.TradeLevelLimit(client))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x00345814 File Offset: 0x00343A14
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = (int)this.CmdID;
			bool result;
			switch (this.CmdID)
			{
			case TCPGameServerCmds.CMD_SPR_OPENMARKET2:
				result = this.OpenMarket(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETSALEMONEY2:
				result = this.MarketSaleMoney(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_SALEGOODS2:
				result = this.SaleGoods(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_SELFSALEGOODSLIST2:
				result = this.SelfSaleGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_OTHERSALEGOODSLIST2:
				result = this.OtherSaleGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETROLELIST2:
				result = this.MarketRoleList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETGOODSLIST2:
				result = this.MarketGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETBUYGOODS2:
				result = this.MarketBuyGoods(client, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x003458BC File Offset: 0x00343ABC
		private bool OpenMarket(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			int offlineMarket = Convert.ToInt32(fields[1]);
			string marketName = fields[2];
			bool result;
			if (string.IsNullOrEmpty(marketName))
			{
				client.ClientData.AllowMarketBuy = false;
				client.ClientData.MarketName = "";
				string strcmd = string.Format("{0}:{1}:{2}", roleID, marketName, offlineMarket);
				client.sendCmd((int)this.CmdID, strcmd, false);
				result = true;
			}
			else
			{
				marketName = marketName.Substring(0, Math.Min(10, marketName.Length));
				if (client.ClientData.SaleGoodsDataList.Count <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(578, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!Global.AllowOpenMarket(client))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(579, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					client.ClientData.AllowMarketBuy = true;
					client.ClientData.OfflineMarketState = 1;
					client.ClientData.MarketName = marketName;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x00345A34 File Offset: 0x00343C34
		private bool MarketSaleMoney(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			int saleOutMoney = Math.Max(0, Convert.ToInt32(fields[1]));
			int userMoneyPrice = Math.Max(0, Convert.ToInt32(fields[2]));
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int disableMarket = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (disableMarket > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string tip = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, true))
				{
					result = true;
				}
				else if (saleOutMoney > client.ClientData.YinLiang)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1,
						roleID,
						saleOutMoney,
						userMoneyPrice,
						0
					});
					client.sendCmd((int)this.CmdID, strcmd, false);
					result = true;
				}
				else if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleOutMoney, "交易市场一", false))
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-2,
						roleID,
						saleOutMoney,
						userMoneyPrice,
						0
					});
					client.sendCmd((int)this.CmdID, strcmd, false);
					result = true;
				}
				else
				{
					GoodsData goodsData = Global.GetNewGoodsData(50200, 0);
					goodsData.Site = -1;
					goodsData.SaleMoney1 = 0;
					goodsData.SaleYuanBao = userMoneyPrice;
					goodsData.SaleYinPiao = 0;
					goodsData.Quality = saleOutMoney;
					Global.AddSaleGoodsData(client, goodsData);
					int goodsDbID = Global.AddGoodsDBCommand_Hook(this.pool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Forge_level, goodsData.Site, goodsData.Jewellist, false, 0, "临时摆摊需要", false, "1900-01-01 12:00:00", goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, false, null, null, "1900-01-01 12:00:00", 0, true);
					if (goodsDbID < 0)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-3,
							roleID,
							saleOutMoney,
							userMoneyPrice,
							goodsData.Id
						});
						client.sendCmd((int)this.CmdID, strcmd, false);
						result = true;
					}
					else
					{
						goodsData.Id = goodsDbID;
						string[] dbFields = null;
						string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							roleID,
							goodsDbID,
							"*",
							"*",
							"*",
							"*",
							goodsData.Site,
							"*",
							"*",
							"*",
							"*",
							"*",
							goodsData.SaleMoney1,
							goodsData.SaleYuanBao,
							goodsData.SaleYinPiao,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(this.tcpClientPool, this.pool, 10006, strcmd, out dbFields, client.ServerId);
						if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-4,
								roleID,
								saleOutMoney,
								userMoneyPrice,
								goodsData.Id
							});
							client.sendCmd((int)this.CmdID, strcmd, false);
							result = true;
						}
						else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-5,
								roleID,
								saleOutMoney,
								userMoneyPrice,
								goodsData.Id
							});
							client.sendCmd((int)this.CmdID, strcmd, false);
							result = true;
						}
						else
						{
							Global.ModRoleGoodsEvent(client, goodsData, 0, "铜钱交易上架", false);
							EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "铜钱交易上架");
							SaleGoodsItem saleGoodsItem = new SaleGoodsItem
							{
								GoodsDbID = goodsData.Id,
								SalingGoodsData = goodsData,
								Client = client
							};
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							if (1 == client.ClientData.SaleGoodsDataList.Count)
							{
								SaleRoleManager.AddSaleRoleItem(client);
							}
							client.ClientData.AllowMarketBuy = true;
							client.ClientData.OfflineMarketState = 1;
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								roleID,
								saleOutMoney,
								userMoneyPrice,
								goodsData.Id
							});
							client.sendCmd((int)this.CmdID, strcmd, false);
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x003460D4 File Offset: 0x003442D4
		private bool SaleGoods(GameClient client, string[] fields)
		{
			TCPGameServerCmds nID = this.CmdID;
			int roleID = Convert.ToInt32(fields[0]);
			int goodsDbID = Convert.ToInt32(fields[1]);
			int site = Convert.ToInt32(fields[2]);
			int saleMoney = Convert.ToInt32(fields[3]);
			int saleYuanBao = Convert.ToInt32(fields[4]);
			int saleYinPiao = Convert.ToInt32(fields[5]);
			int saleGoodsCount = Convert.ToInt32(fields[6]);
			List<int> salemoneys = new List<int>
			{
				saleMoney,
				saleYuanBao,
				saleYinPiao
			};
			int mc = 0;
			foreach (int i in salemoneys)
			{
				if (i != 0 && mc++ > 1)
				{
					return true;
				}
			}
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int disableMarket = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (disableMarket > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string tip = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (saleYuanBao > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, true))
				{
					result = true;
				}
				else if (saleYinPiao > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.MoBi, true))
				{
					result = true;
				}
				else if (saleMoney > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YinLiang, true))
				{
					result = true;
				}
				else
				{
					int bagIndex = 0;
					GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, goodsDbID);
					string strcmd;
					if (goodsData == null)
					{
						goodsData = Global.GetGoodsByDbID(client, goodsDbID);
						if (null == goodsData)
						{
							goodsData = Global.GetSaleGoodsDataByDbID(client, goodsDbID);
							if (null == goodsData)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("从交易市场定位物品对象失败, CMD={0}, Client={1}, RoleID={2}, GoodsDbID={3}", new object[]
								{
									nID,
									Global.GetSocketRemoteEndPoint(client.ClientSocket, false),
									roleID,
									goodsDbID
								}), null, true);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && site != 15000)
							{
								return true;
							}
							if (SaleGoodsManager.RemoveSaleGoodsItem(goodsDbID) == null && null == LiXianBaiTanManager.RemoveLiXianSaleGoodsItem(goodsDbID))
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(2623, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								return true;
							}
							if (!RebornEquip.IsRebornType(goodsData.GoodsID))
							{
								if (!Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, true, false))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(581, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									return true;
								}
								bagIndex = Global.GetIdleSlotOfBagGoods(client);
							}
							else
							{
								if (!RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, true, false))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(7000, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									return true;
								}
								bagIndex = RebornEquip.GetIdleSlotOfRebornGoods(client);
							}
						}
						else
						{
							if (goodsData.Using > 0)
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-7,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
							if (goodsData.Binding > 0)
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-100,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
							if (!Global.CanExchangeCategoriy(goodsData))
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-100,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
							if (Global.IsTimeLimitGoods(goodsData))
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-101,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
							if (Global.GetSaleGoodsDataCount(client) >= SaleManager.MaxSaleNum)
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-110,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
							int gridNum = Global.GetGoodsGridNumByID(goodsData.GoodsID);
							if (gridNum > 1 && saleGoodsCount > 0 && saleGoodsCount < goodsData.GCount)
							{
								if (TCPProcessCmdResults.RESULT_OK != Global.SplitGoodsByCmdParams(client, client.ClientSocket, 133, roleID, goodsData.Id, goodsData.Site, goodsData.GoodsID, goodsData.GCount - saleGoodsCount, false))
								{
									strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										-201,
										roleID,
										goodsDbID,
										site,
										saleMoney,
										saleYuanBao
									});
									client.sendCmd(654, strcmd, false);
									return true;
								}
							}
						}
					}
					else
					{
						if (goodsData.Using > 0)
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-7,
								roleID,
								goodsDbID,
								site,
								saleMoney,
								saleYuanBao
							});
							client.sendCmd(654, strcmd, false);
							return true;
						}
						if (goodsData.Binding > 0)
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-100,
								roleID,
								goodsDbID,
								site,
								saleMoney,
								saleYuanBao
							});
							client.sendCmd(654, strcmd, false);
							return true;
						}
						if (Global.IsTimeLimitGoods(goodsData))
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-101,
								roleID,
								goodsDbID,
								site,
								saleMoney,
								saleYuanBao
							});
							client.sendCmd(654, strcmd, false);
							return true;
						}
						if (Global.GetSaleGoodsDataCount(client) >= SaleManager.MaxSaleNum)
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-110,
								roleID,
								goodsDbID,
								site,
								saleMoney,
								saleYuanBao
							});
							client.sendCmd(654, strcmd, false);
							return true;
						}
						int gridNum = Global.GetGoodsGridNumByID(goodsData.GoodsID);
						if (gridNum > 1 && saleGoodsCount > 0 && saleGoodsCount < goodsData.GCount)
						{
							if (TCPProcessCmdResults.RESULT_OK != Global.SplitGoodsByCmdParams(client, client.ClientSocket, 133, roleID, goodsData.Id, goodsData.Site, goodsData.GoodsID, goodsData.GCount - saleGoodsCount, false))
							{
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-201,
									roleID,
									goodsDbID,
									site,
									saleMoney,
									saleYuanBao
								});
								client.sendCmd(654, strcmd, false);
								return true;
							}
						}
					}
					string[] dbFields = null;
					strcmd = Global.FormatUpdateDBGoodsStr(new object[]
					{
						roleID,
						goodsDbID,
						"*",
						"*",
						"*",
						"*",
						site,
						"*",
						"*",
						"*",
						"*",
						bagIndex,
						saleMoney,
						saleYuanBao,
						saleYinPiao,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*"
					});
					TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(this.tcpClientPool, this.pool, 10006, strcmd, out dbFields, client.ServerId);
					if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-1,
							roleID,
							goodsDbID,
							site,
							saleMoney,
							saleYuanBao,
							saleYinPiao
						});
						client.sendCmd(654, strcmd, false);
						result = true;
					}
					else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-10,
							roleID,
							goodsDbID,
							site,
							saleMoney,
							saleYuanBao,
							saleYinPiao
						});
						client.sendCmd(654, strcmd, false);
						result = true;
					}
					else
					{
						goodsData.BagIndex = bagIndex;
						if (goodsData.Site != site)
						{
							if (goodsData.Site == 0 && site == -1)
							{
								Global.RemoveGoodsData(client, goodsData);
								goodsData.Site = site;
								goodsData.SaleMoney1 = saleMoney;
								goodsData.SaleYuanBao = saleYuanBao;
								goodsData.SaleYinPiao = saleYinPiao;
								Global.AddSaleGoodsData(client, goodsData);
								Global.ModRoleGoodsEvent(client, goodsData, 0, "交易上架", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易上架");
								SaleGoodsItem saleGoodsItem = new SaleGoodsItem
								{
									GoodsDbID = goodsData.Id,
									SalingGoodsData = goodsData,
									Client = client
								};
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (1 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.AddSaleRoleItem(client);
								}
								client.ClientData.AllowMarketBuy = true;
								client.ClientData.OfflineMarketState = 1;
							}
							else if (goodsData.Site == -1 && site == 0)
							{
								SaleGoodsManager.RemoveSaleGoodsItem(goodsData.Id);
								Global.RemoveSaleGoodsData(client, goodsData);
								if (50200 != goodsData.GoodsID)
								{
									goodsData.Site = site;
									goodsData.SaleMoney1 = 0;
									goodsData.SaleYuanBao = 0;
									goodsData.SaleYinPiao = 0;
									Global.AddGoodsData(client, goodsData);
									Global.ModRoleGoodsEvent(client, goodsData, 0, "交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易下架");
								}
								else if (GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
								{
									int addMoney = Math.Max(0, goodsData.Quality);
									if (addMoney > 0)
									{
										GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, addMoney, "金币下架", false);
									}
									Global.ModRoleGoodsEvent(client, goodsData, 0, "铜钱交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "铜钱交易下架");
								}
								if (0 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
								}
							}
							else if (goodsData.Site == 15000 && site == -1)
							{
								RebornEquip.RemoveGoodsData(client, goodsData);
								goodsData.Site = site;
								goodsData.SaleMoney1 = saleMoney;
								goodsData.SaleYuanBao = saleYuanBao;
								goodsData.SaleYinPiao = saleYinPiao;
								Global.AddSaleGoodsData(client, goodsData);
								Global.ModRoleGoodsEvent(client, goodsData, 0, "交易上架", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易上架");
								SaleGoodsItem saleGoodsItem = new SaleGoodsItem
								{
									GoodsDbID = goodsData.Id,
									SalingGoodsData = goodsData,
									Client = client
								};
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (1 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.AddSaleRoleItem(client);
								}
								client.ClientData.AllowMarketBuy = true;
								client.ClientData.OfflineMarketState = 1;
							}
							else if (goodsData.Site == -1 && site == 15000)
							{
								SaleGoodsManager.RemoveSaleGoodsItem(goodsData.Id);
								Global.RemoveSaleGoodsData(client, goodsData);
								if (50200 != goodsData.GoodsID)
								{
									goodsData.Site = site;
									goodsData.SaleMoney1 = 0;
									goodsData.SaleYuanBao = 0;
									goodsData.SaleYinPiao = 0;
									RebornEquip.AddGoodsData(client, goodsData);
									Global.ModRoleGoodsEvent(client, goodsData, 0, "交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易下架");
								}
								else if (GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
								{
									int addMoney = Math.Max(0, goodsData.Quality);
									if (addMoney > 0)
									{
										GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, addMoney, "金币下架", false);
									}
									Global.ModRoleGoodsEvent(client, goodsData, 0, "铜钱交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "铜钱交易下架");
								}
								if (0 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
								}
							}
						}
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							0,
							roleID,
							goodsDbID,
							site,
							saleMoney,
							saleYuanBao,
							saleYinPiao
						});
						client.sendCmd(654, strcmd, false);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x00347388 File Offset: 0x00345588
		private bool SelfSaleGoodsList(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<List<GoodsData>>(655, null, false);
				result = true;
			}
			else
			{
				List<GoodsData> saleGoodsDataList = client.ClientData.SaleGoodsDataList;
				client.sendCmd<List<GoodsData>>(655, saleGoodsDataList, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x003473E4 File Offset: 0x003455E4
		private bool OtherSaleGoodsList(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			int otherRoleID = Convert.ToInt32(fields[1]);
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<List<GoodsData>>(656, null, false);
				result = true;
			}
			else
			{
				List<GoodsData> saleGoodsDataList = new List<GoodsData>();
				GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
				if (null != otherClient)
				{
					saleGoodsDataList = otherClient.ClientData.SaleGoodsDataList;
				}
				else
				{
					saleGoodsDataList = LiXianBaiTanManager.GetLiXianSaleGoodsList(otherRoleID);
				}
				client.sendCmd<List<GoodsData>>(656, saleGoodsDataList, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06003D8D RID: 15757 RVA: 0x00347478 File Offset: 0x00345678
		private bool MarketRoleList(GameClient client, string[] fields)
		{
			List<SaleRoleData> saleRoleDataList = SaleRoleManager.GetSaleRoleDataList();
			client.sendCmd<List<SaleRoleData>>(657, saleRoleDataList, false);
			return true;
		}

		// Token: 0x06003D8E RID: 15758 RVA: 0x003474A0 File Offset: 0x003456A0
		private bool MarketGoodsList(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			int marketSearchType = Convert.ToInt32(fields[1]);
			int startIndex = Convert.ToInt32(fields[2]);
			int maxCount = Convert.ToInt32(fields[3]);
			string marketSearchText = fields[4];
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<SaleGoodsSearchResultData>(658, null, false);
				result = true;
			}
			else if (SingletonTemplate<CreateRoleLimitManager>.Instance().RefreshMarketSlotTicks > 0 && TimeUtil.NOW() - client.ClientData._RefreshMarketTicks < (long)SingletonTemplate<CreateRoleLimitManager>.Instance().RefreshMarketSlotTicks)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(129, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				client.ClientData._RefreshMarketTicks = TimeUtil.NOW();
				SaleGoodsSearchResultData saleGoodsSearchResultData = new SaleGoodsSearchResultData();
				if (0 == marketSearchType)
				{
					saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.GetSaleGoodsDataList();
				}
				else if (2 == marketSearchType)
				{
					Dictionary<int, bool> goodsIDDict = new Dictionary<int, bool>();
					string[] searchFileds = marketSearchText.Split(new char[]
					{
						','
					});
					if (searchFileds != null && searchFileds.Length > 0)
					{
						for (int i = 0; i < searchFileds.Length; i++)
						{
							int searchGoodsID = Global.SafeConvertToInt32(searchFileds[i]);
							goodsIDDict[searchGoodsID] = true;
						}
						saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.FindSaleGoodsDataList(goodsIDDict);
					}
				}
				else if (1 == marketSearchType)
				{
					saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.FindSaleGoodsDataListByRoleName(marketSearchText);
				}
				else if (3 == marketSearchType)
				{
					string[] searchParams = marketSearchText.Split(new char[]
					{
						'$'
					});
					if (searchParams.Length >= 6)
					{
						int type = Global.SafeConvertToInt32(searchParams[0]);
						int id = Global.SafeConvertToInt32(searchParams[1]);
						int moneyFlags = Global.SafeConvertToInt32(searchParams[2]);
						int colorFlags = Global.SafeConvertToInt32(searchParams[3]);
						int orderBy = Global.SafeConvertToInt32(searchParams[4]);
						int orderTypeFlags = 1;
						List<int> goodsIDs;
						if (searchParams.Length >= 7)
						{
							orderTypeFlags = Global.SafeConvertToInt32(searchParams[5]);
							goodsIDs = Global.StringToIntList(searchParams[6], '#');
						}
						else
						{
							goodsIDs = Global.StringToIntList(searchParams[5], '#');
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, false))
						{
							moneyFlags &= 5;
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.MoBi, false))
						{
							moneyFlags &= 3;
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YinLiang, false))
						{
							moneyFlags &= 6;
						}
						saleGoodsSearchResultData.Type = type;
						saleGoodsSearchResultData.ID = id;
						saleGoodsSearchResultData.MoneyFlags = moneyFlags;
						saleGoodsSearchResultData.ColorFlags = colorFlags;
						saleGoodsSearchResultData.OrderBy = orderBy;
						if (moneyFlags <= 0)
						{
							moneyFlags = 7;
						}
						if (colorFlags <= 0)
						{
							colorFlags = 63;
						}
						SearchArgs args = new SearchArgs(id, type, moneyFlags, colorFlags, orderBy, orderTypeFlags);
						if (goodsIDs.IsNullOrEmpty<int>())
						{
							saleGoodsSearchResultData.saleGoodsDataList = SaleManager.GetSaleGoodsDataList(args, null);
							if (null != saleGoodsSearchResultData.saleGoodsDataList)
							{
								saleGoodsSearchResultData.TotalCount = saleGoodsSearchResultData.saleGoodsDataList.Count;
							}
						}
						else
						{
							saleGoodsSearchResultData.saleGoodsDataList = SaleManager.GetSaleGoodsDataList(args, goodsIDs);
							if (saleGoodsSearchResultData.saleGoodsDataList == null || saleGoodsSearchResultData.saleGoodsDataList.Count == 0)
							{
								saleGoodsSearchResultData.TotalCount = -1;
							}
							else
							{
								saleGoodsSearchResultData.TotalCount = saleGoodsSearchResultData.saleGoodsDataList.Count;
							}
						}
						if (saleGoodsSearchResultData.saleGoodsDataList != null && saleGoodsSearchResultData.saleGoodsDataList.Count > 0)
						{
							saleGoodsSearchResultData.StartIndex = startIndex;
							if (startIndex >= saleGoodsSearchResultData.TotalCount)
							{
								saleGoodsSearchResultData.saleGoodsDataList = null;
							}
							else
							{
								startIndex = Global.GMin(startIndex, saleGoodsSearchResultData.saleGoodsDataList.Count - 1);
								maxCount = Global.GMin(maxCount, saleGoodsSearchResultData.saleGoodsDataList.Count - startIndex);
								saleGoodsSearchResultData.saleGoodsDataList = saleGoodsSearchResultData.saleGoodsDataList.GetRange(startIndex, maxCount);
							}
						}
					}
				}
				client.sendCmd<SaleGoodsSearchResultData>(658, saleGoodsSearchResultData, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06003D8F RID: 15759 RVA: 0x00347914 File Offset: 0x00345B14
		private int CalcRealMoneyAfterTax(int money, MoneyTypes moneyType, out int tax)
		{
			tax = 0;
			if (moneyType == MoneyTypes.YinLiang)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiJinBi);
				tax = Global.GMax(tax, 0);
			}
			else if (moneyType == MoneyTypes.YuanBao)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiZuanShi);
				tax = Global.GMax(tax, 0);
			}
			else if (moneyType == MoneyTypes.MoBi)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiMoBi);
				tax = Global.GMax(tax, 0);
			}
			return money - tax;
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x003479AC File Offset: 0x00345BAC
		private bool CheckBuyParams(GoodsData SalingGoodsData, int clientMoneyType, int clientMoneyValue)
		{
			return clientMoneyValue > 0 && ((clientMoneyType == 8 && SalingGoodsData.SaleMoney1 == clientMoneyValue) || (clientMoneyType == 40 && SalingGoodsData.SaleYuanBao == clientMoneyValue) || (clientMoneyType == 141 && SalingGoodsData.SaleYinPiao == clientMoneyValue));
		}

		// Token: 0x06003D91 RID: 15761 RVA: 0x00347A24 File Offset: 0x00345C24
		private bool MarketBuyGoods(GameClient client, string[] fields)
		{
			int roleID = Convert.ToInt32(fields[0]);
			int goodsDbID = Convert.ToInt32(fields[1]);
			int goodsID = Convert.ToInt32(fields[2]);
			int clientMoneyType = Convert.ToInt32(fields[3]);
			int clientMoneyValue = Convert.ToInt32(fields[4]);
			int tax = 0;
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int disableMarket = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (disableMarket > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string tip = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, (MoneyTypes)clientMoneyType, true))
				{
					result = true;
				}
				else
				{
					int otherRID = 0;
					GameClient otherClient = null;
					SaleGoodsItem saleGoodsItem = SaleGoodsManager.RemoveSaleGoodsItem(goodsDbID);
					if (null != saleGoodsItem)
					{
						otherClient = GameManager.ClientMgr.FindClient(saleGoodsItem.Client.ClientData.RoleID);
						if (null != otherClient)
						{
							if (otherClient.ClientData.RoleID == client.ClientData.RoleID)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (GameManager.PlatConfigMgr.GetGameConfigItemStr("CanBuySelfGoods", "1") == "1")
								{
									int newSite = RebornEquip.IsRebornType(saleGoodsItem.SalingGoodsData.GoodsID) ? 15000 : 0;
									int[] array = new int[7];
									array[0] = roleID;
									array[1] = goodsDbID;
									array[2] = newSite;
									int[] saleArgs = array;
									this.SaleGoods(client, Array.ConvertAll<int, string>(saleArgs, (int x) => x.ToString()));
								}
								else
								{
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -30, 0, goodsDbID, goodsID, (int)this.CmdID);
								}
								return true;
							}
							if (!otherClient.ClientData.AllowMarketBuy)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -3, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						otherRID = saleGoodsItem.Client.ClientData.RoleID;
					}
					int salePrice;
					if (saleGoodsItem != null && null != otherClient)
					{
						if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(otherClient.ClientData.RoleID))
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							string tip = GLang.GetLang(582, new object[0]);
							GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return true;
						}
						if (!this.CheckBuyParams(saleGoodsItem.SalingGoodsData, clientMoneyType, clientMoneyValue))
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -40, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						GoodsData goodsData = Global.GetSaleGoodsDataByDbID(otherClient, goodsDbID);
						if (null == goodsData)
						{
							if (null == goodsData)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -3, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						if (goodsData.GoodsID != goodsID || goodsData.Binding != 0)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -1003, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						if (50200 != goodsData.GoodsID)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -5, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -5, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						if (saleGoodsItem.SalingGoodsData.SaleMoney1 > 0 && client.ClientData.YinLiang < saleGoodsItem.SalingGoodsData.SaleMoney1)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -10, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						if (saleGoodsItem.SalingGoodsData.SaleYuanBao > 0 && client.ClientData.UserMoney < saleGoodsItem.SalingGoodsData.SaleYuanBao)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -20, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						int yinPiaoGoodsID = -1;
						if (141 == clientMoneyType)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && client.ClientData.MoBi < saleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -11, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						else if (142 == clientMoneyType)
						{
							yinPiaoGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("YinPiaoGoodsID", -1);
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && Global.GetTotalGoodsCountByID(client, yinPiaoGoodsID) < saleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -21, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && 142 == clientMoneyType)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods2(otherClient, yinPiaoGoodsID, saleGoodsItem.SalingGoodsData.SaleYinPiao, 0, "1900-01-01 12:00:00", true))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -22, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsDataList2(otherClient, yinPiaoGoodsID, saleGoodsItem.SalingGoodsData.SaleYinPiao, 0, "1900-01-01 12:00:00", true))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -22, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						GameManager.logDBCmdMgr.AddMessageLog(-1, "交易日志", "交易市场", otherClient.ClientData.RoleName, client.ClientData.RoleName, "交易", client.ClientData.RoleID, client.ClientData.ZoneID, client.strUserID, otherClient.ClientData.RoleID, GameManager.ServerId, "");
						if (saleGoodsItem.SalingGoodsData.SaleMoney1 > 0)
						{
							if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleGoodsItem.SalingGoodsData.SaleMoney1, "交易市场二", false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -10, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, otherClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleMoney1, MoneyTypes.YinLiang, out tax), "交易市场二", false);
						}
						if (saleGoodsItem.SalingGoodsData.SaleYuanBao > 0)
						{
							if (!Global.CanTrade(client))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -20, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleGoodsItem.SalingGoodsData.SaleYuanBao, "新交易市场购买", false, true, false, DaiBiSySType.None))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -20, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, otherClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYuanBao, MoneyTypes.YuanBao, out tax), "新交易市场出售", ActivityTypes.None, "");
						}
						if (141 == clientMoneyType)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								if (!GameManager.ClientMgr.ModifyMoBiValue(client, -saleGoodsItem.SalingGoodsData.SaleYinPiao, "新交易市场购买", false))
								{
									SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -11, 0, goodsDbID, goodsID, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.ModifyMoBiValue(otherClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.MoBi, out tax), "新交易市场出售", false);
							}
						}
						else if (142 == clientMoneyType)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								bool usedBinding = false;
								bool usedTimeLimited = false;
								if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, yinPiaoGoodsID, saleGoodsItem.SalingGoodsData.SaleYinPiao, false, out usedBinding, out usedTimeLimited, false))
								{
									SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -21, 0, goodsDbID, goodsID, (int)this.CmdID);
									return true;
								}
								Global.BatchAddGoods(otherClient, yinPiaoGoodsID, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.None, out tax), 0, "交易市场购买后批量添加");
							}
						}
						salePrice = saleGoodsItem.SalingGoodsData.SaleYuanBao + saleGoodsItem.SalingGoodsData.SaleYinPiao;
						int saleMoney = goodsData.SaleMoney1;
						int saleYuanBao = goodsData.SaleYuanBao;
						int saleYinPiao = goodsData.SaleYinPiao;
						int site = goodsData.Site;
						GoodsData tradeBlackCopy = new GoodsData(goodsData);
						int saleOutMoney = Math.Max(0, goodsData.Quality);
						goodsData.SaleMoney1 = 0;
						goodsData.SaleYuanBao = 0;
						goodsData.SaleYinPiao = 0;
						if (RebornEquip.IsRebornType(goodsData.GoodsID))
						{
							goodsData.Site = 15000;
						}
						else
						{
							goodsData.Site = 0;
						}
						Global.RemoveSaleGoodsData(otherClient, goodsData);
						bool bMoveToTarget = 50200 != goodsData.GoodsID;
						if (!GameManager.ClientMgr.MoveGoodsDataToOtherRole(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, goodsData, otherClient, client, bMoveToTarget))
						{
							this.GiveBackSaleGoodsMoney(client, otherClient, goodsData, saleMoney, saleYuanBao, site);
							GameManager.SystemServerEvents.AddEvent(string.Format("转移物品时失败, 交易市场购买, FromRole={0}({1}), ToRole={2}({3}), GoodsDbID={4}, GoodsID={5}, GoodsNum={6}", new object[]
							{
								otherClient.ClientData.RoleID,
								otherClient.ClientData.RoleName,
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								goodsData.Id,
								goodsData.GoodsID,
								goodsData.GCount
							}), EventLevels.Important);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -100, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						if (!bMoveToTarget)
						{
							if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场在线购买金币失败, {0}=>{1}", Global.FormatRoleName4(otherClient), Global.FormatRoleName4(client)), null, true);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -1004, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleOutMoney, "摆摊出售金币", false);
						}
						Global.AddRoleSaleEvent(client, goodsData.GoodsID, goodsData.GCount, -saleMoney, -saleYinPiao, -saleYuanBao, yinPiaoGoodsID, -saleOutMoney);
						Global.AddRoleSaleEvent(otherClient, goodsData.GoodsID, -goodsData.GCount, Math.Max(0, saleMoney - tax), Math.Max(0, saleYinPiao - tax), Math.Max(0, saleYuanBao - tax), yinPiaoGoodsID, saleOutMoney);
						GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, otherClient, client, 0, 1, goodsDbID, goodsID, (int)this.CmdID);
						GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, 0, 0, goodsDbID, goodsID, (int)this.CmdID);
						string washPropsStr = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps));
						Global.AddMarketBuyLog(otherClient.ClientData.RoleID, client.ClientData.RoleID, client.ClientData.RoleName, goodsData.GoodsID, goodsData.GCount, goodsData.Forge_level, saleYuanBao, client.ClientData.UserMoney, saleMoney, saleYinPiao, tax, goodsData.ExcellenceInfo, washPropsStr);
						SingletonTemplate<TradeBlackManager>.Instance().OnMarketBuy(client.ClientData.RoleID, otherClient.ClientData.RoleID, tradeBlackCopy);
					}
					else
					{
						LiXianSaleGoodsItem liXianSaleGoodsItem = LiXianBaiTanManager.RemoveLiXianSaleGoodsItem(goodsDbID);
						if (null == liXianSaleGoodsItem)
						{
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, null, -1, 0, goodsDbID, goodsID, (int)this.CmdID);
							return true;
						}
						if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(liXianSaleGoodsItem.RoleID))
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							string tip = GLang.GetLang(582, new object[0]);
							GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return true;
						}
						GoodsData goodsData = liXianSaleGoodsItem.SalingGoodsData;
						if (goodsData.GoodsID != goodsID || goodsData.Binding != 0)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -1003, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (!this.CheckBuyParams(liXianSaleGoodsItem.SalingGoodsData, clientMoneyType, clientMoneyValue))
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -40, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (50200 != goodsData.GoodsID)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -5, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -5, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleMoney1 > 0 && client.ClientData.YinLiang < liXianSaleGoodsItem.SalingGoodsData.SaleMoney1)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -10, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao > 0 && client.ClientData.UserMoney < liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -20, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (141 == clientMoneyType)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && client.ClientData.MoBi < liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -11, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
						}
						GameManager.logDBCmdMgr.AddMessageLog(-1, "交易日志", "交易市场", liXianSaleGoodsItem.RoleName, client.ClientData.RoleName, "交易", liXianSaleGoodsItem.RoleID, client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleID, GameManager.ServerId, "");
						if (50200 != goodsData.GoodsID)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleMoney1 > 0)
							{
								if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, liXianSaleGoodsItem.SalingGoodsData.SaleMoney1, "交易市场三", false))
								{
									LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -10, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.AddOfflineUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleMoney1, MoneyTypes.YinLiang, out tax), "交易市场三", client.ClientData.ZoneID);
							}
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao > 0)
						{
							if (!Global.CanTrade(client))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -20, 0, goodsDbID, goodsID, (int)this.CmdID);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao, "新交易市场购买", false, true, false, DaiBiSySType.None))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -20, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddOfflineUserMoney(this.tcpClientPool, this.pool, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao, MoneyTypes.YuanBao, out tax), "新交易市场出售(离线)", liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.UserID);
						}
						if (141 == clientMoneyType)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								if (!GameManager.ClientMgr.ModifyMoBiValue(client, -liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao, "新交易市场购买", false))
								{
									LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, otherClient, -11, 0, goodsDbID, goodsID, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.ModifyMoBiValueOffline(liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.UserID, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.MoBi, out tax), "新交易市场出售", false);
							}
						}
						salePrice = liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao;
						otherRID = liXianSaleGoodsItem.RoleID;
						int saleMoney = goodsData.SaleMoney1;
						int saleYuanBao = goodsData.SaleYuanBao;
						int saleYinPiao = goodsData.SaleYinPiao;
						int site = goodsData.Site;
						GoodsData tradeBlackCopy = new GoodsData(goodsData);
						int saleOutMoney = Math.Max(0, goodsData.Quality);
						goodsData.SaleMoney1 = 0;
						goodsData.SaleYuanBao = 0;
						goodsData.SaleYinPiao = 0;
						if (RebornEquip.IsRebornType(goodsData.GoodsID))
						{
							goodsData.Site = 15000;
						}
						else
						{
							goodsData.Site = 0;
						}
						string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
						bool bMoveToTarget = 50200 != goodsData.GoodsID;
						if (!GameManager.ClientMgr.MoveGoodsDataToOfflineRole(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, goodsData, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.RoleLevel, userID, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.Level, bMoveToTarget, client.ClientData.ZoneID))
						{
							this.GiveBackSaleGoodsMoneyOffline(client, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, goodsData, saleMoney, saleYuanBao, site);
							GameManager.SystemServerEvents.AddEvent(string.Format("转移物品时失败, 交易市场购买, FromRole={0}({1}), ToRole={2}({3}), GoodsDbID={4}, GoodsID={5}, GoodsNum={6}", new object[]
							{
								liXianSaleGoodsItem.RoleID,
								liXianSaleGoodsItem.RoleName,
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								goodsData.Id,
								goodsData.GoodsID,
								goodsData.GCount
							}), EventLevels.Important);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -100, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (!bMoveToTarget)
						{
							if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买金币失败, {0}=>{1}", liXianSaleGoodsItem.RoleName, Global.FormatRoleName4(client)), null, true);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -1004, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleOutMoney, "摆摊出售物品获取金币", false);
						}
						Global.AddRoleSaleEvent2(userID, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.Level, goodsData.GoodsID, goodsData.GCount, -saleMoney, -saleYinPiao, -saleYuanBao, -saleOutMoney);
						Global.AddRoleSaleEvent2(liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.RoleLevel, goodsData.GoodsID, -goodsData.GCount, Math.Max(0, saleMoney - tax), Math.Max(0, saleYinPiao - tax), Math.Max(0, saleYuanBao - tax), saleOutMoney);
						GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, 0, 0, goodsDbID, goodsID, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
						string washPropsStr = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps));
						Global.AddMarketBuyLog(liXianSaleGoodsItem.RoleID, client.ClientData.RoleID, client.ClientData.RoleName, goodsData.GoodsID, goodsData.GCount, goodsData.Forge_level, saleYuanBao, client.ClientData.UserMoney, saleMoney, saleYinPiao, tax, goodsData.ExcellenceInfo, washPropsStr);
						SingletonTemplate<TradeBlackManager>.Instance().OnMarketBuy(client.ClientData.RoleID, liXianSaleGoodsItem.RoleID, tradeBlackCopy);
					}
					int tradelog_num_minamount = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_num_minamount", 5000);
					if (salePrice >= tradelog_num_minamount)
					{
						GameManager.logDBCmdMgr.AddTradeNumberInfo(2, salePrice, otherRID, client.ClientData.RoleID, client.ServerId);
					}
					int freqNumber = Global.IncreaseTradeCount(client, "SaleTradeDayID", "SaleTradeCount", 1);
					int tradelog_freq_sale = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_freq_sale", 10);
					if (freqNumber >= tradelog_freq_sale)
					{
						GameManager.logDBCmdMgr.AddTradeFreqInfo(2, freqNumber, client.ClientData.RoleID, 0);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x0034971C File Offset: 0x0034791C
		private void GiveBackSaleGoodsMoney(GameClient client, GameClient saller, GoodsData SalingGoodsData, int saleMoney, int saleYuanBao, int site)
		{
			SalingGoodsData.SaleMoney1 = saleMoney;
			SalingGoodsData.SaleYuanBao = saleYuanBao;
			SalingGoodsData.Site = site;
			int tax = 0;
			int backSaleMoney = this.CalcRealMoneyAfterTax(saleMoney, MoneyTypes.YinLiang, out tax);
			int backSaleYuanBao = this.CalcRealMoneyAfterTax(saleYuanBao, MoneyTypes.YuanBao, out tax);
			if (SalingGoodsData.SaleMoney1 > 0)
			{
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleMoney1, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleMoney1), null, true);
				}
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, saller, -backSaleMoney, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), -backSaleMoney), null, true);
				}
			}
			if (SalingGoodsData.SaleYuanBao > 0)
			{
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleYuanBao, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleYuanBao), null, true);
				}
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, saller, -backSaleYuanBao, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), -backSaleYuanBao), null, true);
				}
			}
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x003498E4 File Offset: 0x00347AE4
		private void GiveBackSaleGoodsMoneyOffline(GameClient client, string userID, int sallerRoleID, string sallerName, GoodsData SalingGoodsData, int saleMoney, int saleYuanBao, int site)
		{
			SalingGoodsData.SaleMoney1 = saleMoney;
			SalingGoodsData.SaleYuanBao = saleYuanBao;
			SalingGoodsData.Site = site;
			int tax = 0;
			int backSaleMoney = this.CalcRealMoneyAfterTax(saleMoney, MoneyTypes.YinLiang, out tax);
			int backSaleYuanBao = this.CalcRealMoneyAfterTax(saleYuanBao, MoneyTypes.YuanBao, out tax);
			if (SalingGoodsData.SaleMoney1 > 0)
			{
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleMoney1, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleMoney1), null, true);
				}
				if (!GameManager.ClientMgr.AddOfflineUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, userID, sallerRoleID, sallerName, -backSaleMoney, "新交易市场购买失败退回", client.ClientData.ZoneID))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), -backSaleMoney), null, true);
				}
			}
			if (SalingGoodsData.SaleYuanBao > 0)
			{
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleYuanBao, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleYuanBao), null, true);
				}
				if (!GameManager.ClientMgr.AddUserMoneyOffLine(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, sallerRoleID, -backSaleYuanBao, "新交易市场购买失败退回", client.ClientData.ZoneID, client.strUserID))
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), -backSaleYuanBao), null, true);
				}
			}
		}

		// Token: 0x040047B8 RID: 18360
		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_OPENMARKET2;
	}
}
