using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	
	internal class GlodAuctionProcessCmdEx : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static GlodAuctionProcessCmdEx getInstance()
		{
			return GlodAuctionProcessCmdEx.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool startup()
		{
			lock (this.AuctionMsgMutex)
			{
				this.GoldAuctionMgr.InitData();
				TCPCmdDispatcher.getInstance().registerProcessorEx(2080, 8, 8, GlodAuctionProcessCmdEx.getInstance(), TCPCmdFlags.IsStringArrayParams);
				TCPCmdDispatcher.getInstance().registerProcessorEx(2081, 4, 4, GlodAuctionProcessCmdEx.getInstance(), TCPCmdFlags.IsStringArrayParams);
			}
			return true;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					if (nID == 2080)
					{
						GoldAuctionS2C msgData = new GoldAuctionS2C();
						this.GetGlodAuction(client, nID, bytes, cmdParams, ref msgData);
						client.sendCmd<GoldAuctionS2C>(nID, msgData, false);
					}
					else if (nID == 2081)
					{
						int info = 0;
						this.SetGlodAuction(client, nID, bytes, cmdParams, ref info);
						client.sendCmd(nID, info.ToString(), false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		
		private void GetGlodAuction(GameClient client, int nID, byte[] bytes, string[] cmdParams, ref GoldAuctionS2C msgData)
		{
			try
			{
				int Auctiontype = Convert.ToInt32(cmdParams[1]);
				int ordeType = Convert.ToInt32(cmdParams[2]);
				int sortNum = Convert.ToInt32(cmdParams[3]);
				int startPage = Convert.ToInt32(cmdParams[4]);
				int maxNum = Convert.ToInt32(cmdParams[5]);
				string Seach = cmdParams[6];
				int Color = Convert.ToInt32(cmdParams[7]);
				if (!GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)Auctiontype))
				{
					msgData.Info = 2;
				}
				else if (Auctiontype == 1 && client.ClientData.Faction < 1)
				{
					msgData.Info = 1;
				}
				else
				{
					this.GoldAuctionMgr.GetGoldAuctionS2C(Auctiontype, ordeType, sortNum, startPage, maxNum, Seach, Color, ref msgData);
					msgData.Info = 0;
				}
			}
			catch (Exception ex)
			{
				msgData.Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private void SetGlodAuction(GameClient client, int nID, byte[] bytes, string[] cmdParams, ref int Info)
		{
			GoldAuctionItem item = null;
			Info = 0;
			try
			{
				int type = Convert.ToInt32(cmdParams[1]);
				string[] itemKey = cmdParams[2].Split(new char[]
				{
					'|'
				});
				int Price = Convert.ToInt32(cmdParams[3]);
				if (itemKey == null || itemKey.Length != 2)
				{
					Info = 3;
				}
				else
				{
					string ProductionTime = itemKey[0].Replace(',', ':');
					int AuctionSource = Convert.ToInt32(itemKey[1]);
					if (type == 1 && client.ClientData.Faction < 1)
					{
						Info = 1;
					}
					AuctionConfig AuctionCfg = GoldAuctionConfigModel.GetAuctionConfig(AuctionSource);
					if (null == AuctionCfg)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]SetGlodAuction ({0}) null == config", AuctionSource), null, true);
						Info = 4;
					}
					else
					{
						item = this.GoldAuctionMgr.GetGoldAuctionItem(type, ProductionTime, AuctionSource, true);
						if (item == null || item.AuctionType != type)
						{
							Info = 9;
						}
						else if (client.ClientData.RoleID == item.BuyerData.m_RoleID)
						{
							Info = 11;
						}
						else if (item.BuyerData.m_RoleID == 0 && Price < AuctionCfg.OriginPrice)
						{
							Info = 5;
						}
						else if (Price < AuctionCfg.UnitPrice)
						{
							Info = 6;
						}
						else if ((long)Price <= item.BuyerData.Value)
						{
							Info = 10;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(client, Price, "金团拍卖购买", false, false, false, false, DaiBiSySType.None))
						{
							Info = 7;
						}
						if (Info != 0)
						{
							this.GoldAuctionMgr.UnLock(item.ProductionTime, item.AuctionSource);
						}
						else
						{
							AuctionRoleData oldBuyerData = new AuctionRoleData();
							CopyData.Copy<AuctionRoleData>(item.BuyerData, ref oldBuyerData);
							bool upFlag = false;
							int newBuyer = client.ClientData.RoleID;
							item.BuyerData.Value = (long)Price;
							item.BuyerData.m_RoleID = client.ClientData.RoleID;
							item.BuyerData.m_RoleName = client.ClientData.RoleName;
							item.BuyerData.ZoneID = client.ClientData.ZoneID;
							item.BuyerData.strUserID = client.strUserID;
							item.BuyerData.ServerId = client.ServerId;
							if (Price >= AuctionCfg.MaxPrice && AuctionCfg.MaxPrice > 0)
							{
								if (!this.GoldAuctionMgr.DisposeAward(item))
								{
									Info = 8;
									LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]一口价 RoleId={0}, Price={1},itemKey={2} fail", newBuyer, Price, cmdParams[1]), null, true);
								}
								else
								{
									this.GoldAuctionMgr.DelGoldAuction(item, "金团拍卖购买su");
									upFlag = true;
								}
							}
							else if (!this.GoldAuctionMgr.UpdatePrice(item))
							{
								Info = 8;
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]更新价格 RoleId={0}, Price={1},itemKey={2} fail", newBuyer, Price, cmdParams[1]), null, true);
							}
							else
							{
								upFlag = true;
							}
							if (upFlag)
							{
								this.GoldAuctionMgr.ReturnOldAuctionMoney(oldBuyerData, item.StrGoods);
							}
							else
							{
								this.GoldAuctionMgr.UnLock(item.ProductionTime, item.AuctionSource);
								GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "金团购买失败扣除钻石", client.ClientData.RoleName, client.ClientData.RoleName, "减少", Price, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (null != item)
				{
					this.GoldAuctionMgr.UnLock(item.ProductionTime, item.AuctionSource);
				}
				Info = 100;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		public void KillBossAddAuction(int KillBossRole, long BossLife, List<AuctionRoleData> PointInfoList, AuctionEnum AuctionSource)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					AuctionAwardConfig AuctionAwardCfg = GoldAuctionConfigModel.RandAuctionAwardConfig();
					if (null == AuctionAwardCfg)
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl]KillBossAddAuction RandAuctionAwardConfig null == config", null, true);
					}
					else
					{
						AuctionConfig AuctionCfg = GoldAuctionConfigModel.GetAuctionConfig((int)AuctionSource);
						if (null == AuctionCfg)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GoldAuctionConfigModel.GetAuctionConfig({0}) null == config", AuctionSource), null, true);
						}
						else
						{
							GoldAuctionItem item = new GoldAuctionItem();
							item.AuctionSource = (int)AuctionSource;
							item.KillBossRoleID = KillBossRole;
							item.UpDBWay = 2;
							item.ProductionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss.fff");
							item.BuyerData = new AuctionRoleData();
							item.BuyerData.m_RoleID = 0;
							item.BuyerData.Value = (long)AuctionCfg.OriginPrice;
							int index = Global.GetRandomNumber(AuctionAwardCfg.StartValues, AuctionAwardCfg.EndValues) - 1;
							index = Math.Min(index, AuctionAwardCfg.strGoodsList.Count - 1);
							item.StrGoods = AuctionAwardCfg.strGoodsList[index];
							if (null == GlobalNew.ParseGoodsData(AuctionAwardCfg.strGoodsList[index]))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]null == item.Goods index={0}  ,  GoodsList.Count = {1}", index, AuctionAwardCfg.strGoodsList.Count), null, true);
							}
							else
							{
								item.BossLife = 0L;
								foreach (AuctionRoleData PointInfo in PointInfoList)
								{
									item.BossLife += PointInfo.Value;
									if (PointInfo.Value >= GameManager.systemParamsList.GetParamValueIntByName("AngelTempleAuction", -1))
									{
										AuctionRoleData RoleData = new AuctionRoleData();
										CopyData.Copy<AuctionRoleData>(PointInfo, ref RoleData);
										item.RoleList.Add(RoleData);
									}
								}
								if (item.BossLife < 1L)
								{
									item.BossLife = BossLife;
								}
								bool addFlag = false;
								for (int i = 0; i < AuctionCfg.OrderList.Count; i++)
								{
									if (GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)AuctionCfg.OrderList[i]))
									{
										item.AuctionType = AuctionCfg.OrderList[i];
										item.LifeTime = AuctionCfg.TimeList[i];
										if (item.AuctionType > 0 && item.AuctionType < 3)
										{
											if (this.GoldAuctionMgr.SendUpdate2DB(item))
											{
												this.GoldAuctionMgr.AddNewAuctionItem(item);
											}
											else
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]新拍卖物品存库失败 未加入 time={0}，AuctionSource={1}", item.ProductionTime, AuctionSource), null, true);
											}
											addFlag = true;
											break;
										}
										LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]AuctionType ={1} err，AuctionSource={0}", AuctionSource, item.AuctionType), null, true);
									}
								}
								if (!addFlag)
								{
									int roleID = item.GetMaxmDamageID();
									this.GoldAuctionMgr.SendItem(roleID, item.RoleList.Find((AuctionRoleData x) => x.m_RoleID == roleID), item, AuctionCfg);
									LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]新拍卖物品未加入 直接邮件发送给玩家第一名 time={0}，AuctionSource={1}", item.ProductionTime, AuctionSource), null, true);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		
		private static GlodAuctionProcessCmdEx instance = new GlodAuctionProcessCmdEx();

		
		private object AuctionMsgMutex = new object();

		
		private GoldAuctionManager GoldAuctionMgr = new GoldAuctionManager();
	}
}
