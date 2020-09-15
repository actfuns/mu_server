using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A8 RID: 168
	public class GoldAuctionManager
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0002C0EC File Offset: 0x0002A2EC
		public void InitData()
		{
			try
			{
				lock (this.AuctionMutex)
				{
					this.AuctionItemList = new List<GoldAuctionItem>();
					this.S2CCache = new Dictionary<string, AuctionS2CCache>();
					this.InitFromDB();
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("GoldAuctionManager.TimerProc", new EventHandler(this.TimerProc)), 0, 1000);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0002C1A8 File Offset: 0x0002A3A8
		private void InitFromDB()
		{
			try
			{
				for (int i = 1; i < 3; i++)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]InitFromDB AuctionID={0}", i), null, true);
					GetAuctionDBData DBData = Global.sendToDB<GetAuctionDBData, string>(2080, i.ToString(), 0);
					if (DBData == null || !DBData.Flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GoldAuctionManager.InitFromDB DBData={0}, type={1}", null == DBData, i), null, true);
					}
					else
					{
						foreach (GoldAuctionDBItem item in DBData.ItemList)
						{
							GoldAuctionItem temp = new GoldAuctionItem();
							CopyData.CopyAuctionDB2Item(item, out temp);
							AuctionConfig AuctionCfg = GoldAuctionConfigModel.GetAuctionConfig(item.AuctionSource);
							if (null == AuctionCfg)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GetAuctionConfig null == config AuctionSource ={0}", ((AuctionEnum)item.AuctionSource).ToString()), null, true);
							}
							else
							{
								temp.LifeTime = AuctionCfg.GetTimeByAuction(i);
								if (temp.LifeTime == -1)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GetTimeByAuction =-1 AuctionOrderEnum={0}", i), null, true);
								}
								else
								{
									this.AddNewAuctionItem(temp);
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

		// Token: 0x06000296 RID: 662 RVA: 0x0002C368 File Offset: 0x0002A568
		public bool SendUpdate2DB(GoldAuctionItem AuctionItem)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					if (AuctionItem == null)
					{
						return false;
					}
					GoldAuctionDBItem dbinfo;
					CopyData.CopyAuctionItem2DB(AuctionItem, out dbinfo);
					string msg = Global.Send2DB<GoldAuctionDBItem>(2081, dbinfo, 0);
					if ("True".Equals(msg))
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]AuctionItem SendUpdate2DB true up type = {0},ProductionTime={1}, AuctionSource={2}", (DBAuctionUpEnum)dbinfo.UpDBWay, dbinfo.ProductionTime, dbinfo.AuctionSource), null, true);
						return true;
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]AuctionItem SendUpdate2DB false up type = {0},ProductionTime={1}, AuctionSource={2}", (DBAuctionUpEnum)dbinfo.UpDBWay, dbinfo.ProductionTime, dbinfo.AuctionSource), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0002C490 File Offset: 0x0002A690
		public void UnLock(string ProductionTime, int AuctionSource)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem item = this.getGoldAuctionItem(ProductionTime, AuctionSource);
					if (null != item)
					{
						item.Lock = false;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0002C568 File Offset: 0x0002A768
		private GoldAuctionItem getGoldAuctionItem(string ProductionTime, int AuctionSource)
		{
			GoldAuctionItem item = null;
			try
			{
				item = this.AuctionItemList.Find((GoldAuctionItem x) => x.ProductionTime == ProductionTime && x.AuctionSource == AuctionSource);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return item;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0002C5E8 File Offset: 0x0002A7E8
		public void AddNewAuctionItem(GoldAuctionItem AuctionItem)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem item = this.getGoldAuctionItem(AuctionItem.ProductionTime, AuctionItem.AuctionSource);
					if (null != item)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]AddNewGoldAuction same time={0}, AuctionSource={1}", AuctionItem.ProductionTime, (AuctionEnum)AuctionItem.AuctionSource), null, true);
					}
					else
					{
						this.AuctionItemList.Add(AuctionItem);
						this.S2CCache.Clear();
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]AddNewGoldAuction su time={0}, AuctionSource={1}", AuctionItem.ProductionTime, (AuctionEnum)AuctionItem.AuctionSource), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0002C72C File Offset: 0x0002A92C
		public void DelGoldAuction(GoldAuctionItem Item, string info)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => x.ProductionTime == Item.ProductionTime && x.AuctionSource == Item.AuctionSource) > 0)
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]{0} DelGoldAuction time={1}, AuctionSource={2}", info, Item.ProductionTime, (AuctionEnum)Item.AuctionSource), null, true);
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0002C820 File Offset: 0x0002AA20
		public bool UpdatePrice(GoldAuctionItem _item)
		{
			bool flag = false;
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem item = this.getGoldAuctionItem(_item.ProductionTime, _item.AuctionSource);
					if (item == null || item.AuctionType != _item.AuctionType)
					{
						return flag;
					}
					CopyData.Copy<AuctionRoleData>(_item.BuyerData, ref item.BuyerData);
					item.Lock = false;
					item.UpDBWay = 3;
					item.OldAuctionType = item.AuctionType;
					flag = this.SendUpdate2DB(item);
					if (flag)
					{
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return flag;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0002C928 File Offset: 0x0002AB28
		public GoldAuctionItem GetGoldAuctionItem(int type, string ProductionTime, int AuctionSource, bool isLock)
		{
			GoldAuctionItem temp = null;
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem item = this.getGoldAuctionItem(ProductionTime, AuctionSource);
					if (item == null || item.AuctionType != type)
					{
						return null;
					}
					item.Lock = isLock;
					CopyData.CopyGoldAuctionItem(item, ref temp);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return temp;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0002C9E4 File Offset: 0x0002ABE4
		public void GetGoldAuctionS2C(int Auctiontype, int ordeType, int sortNum, int startNum, int maxNum, string Seach, int Color, ref GoldAuctionS2C clientData)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					string memKey = string.Format("{0}|{1}|{2}|{3}|{4}", new object[]
					{
						Auctiontype,
						ordeType,
						sortNum,
						Seach,
						Color
					});
					int allNum = 0;
					List<AuctionItemS2C> dList = new List<AuctionItemS2C>();
					AuctionS2CCache Cache;
					if (!this.S2CCache.TryGetValue(memKey, out Cache))
					{
						Cache = new AuctionS2CCache();
						foreach (GoldAuctionItem item in this.AuctionItemList)
						{
							if (item.AuctionType == Auctiontype)
							{
								allNum++;
								AuctionItemS2C temp;
								if (CopyData.Copy2AuctionItemS2C(item, out temp, Seach, Color))
								{
									dList.Add(temp);
								}
							}
						}
						dList.Sort(new GlodAuctionIComparer(ordeType, sortNum > 0));
						Cache.MaxNum = allNum;
						Cache.dList.AddRange(dList);
						this.S2CCache.Add(memKey, Cache);
					}
					else
					{
						allNum = Cache.MaxNum;
						foreach (AuctionItemS2C item2 in Cache.dList)
						{
							string[] itemKey = item2.AuctionItemKey.Split(new char[]
							{
								'|'
							});
							GoldAuctionItem AuctionItem = this.getGoldAuctionItem(itemKey[0].Replace(',', ':'), Convert.ToInt32(itemKey[1]));
							AuctionItemS2C temp;
							if (CopyData.Copy2AuctionItemS2C(item2, out temp, AuctionItem))
							{
								dList.Add(temp);
							}
						}
					}
					int len = maxNum;
					int starIndex = (startNum - 1) * maxNum;
					clientData.CurrentPage = startNum;
					if (starIndex < 1 || starIndex >= dList.Count)
					{
						clientData.CurrentPage = 1;
						len = Math.Min(maxNum, dList.Count);
					}
					else if (starIndex + maxNum > dList.Count)
					{
						len = dList.Count - starIndex;
					}
					clientData.TotalCount = allNum;
					clientData.ItemList.AddRange(dList.GetRange(starIndex, len));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0002CCF0 File Offset: 0x0002AEF0
		public static bool IsOpenAuction(AuctionOrderEnum type)
		{
			try
			{
				if (type == AuctionOrderEnum.AuctionZhanMeng)
				{
					return GameManager.systemParamsList.GetParamValueIntByName("AuctionZhanMengOpen", -1) > 0L;
				}
				if (type == AuctionOrderEnum.AuctionWorld)
				{
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0002CDC8 File Offset: 0x0002AFC8
		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					foreach (GoldAuctionItem item in this.AuctionItemList)
					{
						this.SetOutTimeItem(item);
					}
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => DateTime.Parse(x.AuctionTime).AddHours((double)x.LifeTime) <= TimeUtil.NowDateTime() && !x.Lock) > 0)
					{
						this.S2CCache.Clear();
					}
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => x.UpDBWay == 1) > 0)
					{
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0002CF14 File Offset: 0x0002B114
		private bool SetOutTimeItem(GoldAuctionItem AuctionItem)
		{
			try
			{
				if (AuctionItem.Lock || DateTime.Parse(AuctionItem.AuctionTime).AddHours((double)AuctionItem.LifeTime) > TimeUtil.NowDateTime())
				{
					return true;
				}
				if (AuctionItem.BuyerData.m_RoleID != 0)
				{
					return this.DisposeAward(AuctionItem);
				}
				AuctionConfig AuctionCfg = GoldAuctionConfigModel.GetAuctionConfig(AuctionItem.AuctionSource);
				if (null == AuctionCfg)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]null == config AuctionSource = {0}", AuctionItem.AuctionSource), null, true);
					return false;
				}
				int NextAuctiontype = AuctionCfg.GetNextAuction(AuctionItem.AuctionType);
				if (NextAuctiontype <= -1)
				{
					return this.DisposeAward(AuctionItem);
				}
				AuctionItem.OldAuctionType = AuctionItem.AuctionType;
				AuctionItem.AuctionType = NextAuctiontype;
				AuctionItem.AuctionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss");
				AuctionItem.UpDBWay = 3;
				AuctionItem.LifeTime = AuctionCfg.GetTimeByAuction(NextAuctiontype);
				this.S2CCache.Clear();
				if (this.SendUpdate2DB(AuctionItem))
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]超时换拍卖行ProductionTime = {0}, AuctionSource={1},AuctionType={2}", AuctionItem.ProductionTime, AuctionItem.AuctionSource, AuctionItem.AuctionType), null, true);
					return true;
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]超时换拍卖行 db失败 ProductionTime = {0}, AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
				if (AuctionItem.LifeTime == -1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GetTimeByAuction =-1 AuctionOrderEnum={0}", NextAuctiontype), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0002D13C File Offset: 0x0002B33C
		public bool DisposeAward(GoldAuctionItem Item)
		{
			try
			{
				bool flag = false;
				try
				{
					object auctionMutex;
					Monitor.Enter(auctionMutex = this.AuctionMutex, ref flag);
					if (null == Item)
					{
						return false;
					}
					AuctionConfig AuctionCfg = GoldAuctionConfigModel.GetAuctionConfig(Item.AuctionSource);
					if (null == AuctionCfg)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]GoldAuctionConfigModel.GetAuctionConfig({0}) null == config", (AuctionEnum)Item.AuctionSource), null, true);
						return false;
					}
					long averageNum = 0L;
					int minNum = (int)GameManager.systemParamsList.GetParamValueIntByName("AngelTempleAuctionMin", 0);
					if (minNum < 0)
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl]AngelTempleAuctionMin < 0", null, true);
						minNum = 1;
					}
					long delNum = 0L;
					if (0 == Item.BuyerData.m_RoleID)
					{
						averageNum = 0L;
						delNum = Item.BuyerData.Value;
					}
					else if (Item.RoleList.Count < 1)
					{
						averageNum = 0L;
					}
					else if ((long)(minNum * Item.RoleList.Count) < Item.BuyerData.Value)
					{
						averageNum = (long)minNum;
						delNum = (long)Item.RoleList.Count * averageNum;
					}
					else if ((long)(minNum * Item.RoleList.Count) >= Item.BuyerData.Value)
					{
						averageNum = Item.BuyerData.Value / (long)Item.RoleList.Count;
						delNum = Item.BuyerData.Value;
					}
					Item.UpDBWay = 1;
					Item.OldAuctionType = Item.AuctionType;
					this.S2CCache.Clear();
					if (!this.SendUpdate2DB(Item))
					{
						Item.UpDBWay = 3;
						return false;
					}
					Item.BuyerData.Value -= delNum;
					int AwardRole = Item.BuyerData.m_RoleID;
					if (AwardRole > 0)
					{
						this.SendMoney(Item, averageNum, AuctionCfg);
						this.SendItem(AwardRole, Item.BuyerData, Item, AuctionCfg);
					}
					else
					{
						AwardRole = Item.GetMaxmDamageID();
						this.SendItem(AwardRole, Item.RoleList.Find((AuctionRoleData x) => x.m_RoleID == AwardRole), Item, AuctionCfg);
					}
				}
				finally
				{
					if (flag)
					{
						object auctionMutex;
						Monitor.Exit(auctionMutex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0002D41C File Offset: 0x0002B61C
		private void SendMoney(GoldAuctionItem AuctionItem, long averageNum, AuctionConfig AuctionCfg)
		{
			try
			{
				Dictionary<AuctionRoleData, long> sendMsg = new Dictionary<AuctionRoleData, long>();
				if (averageNum >= 1L)
				{
					foreach (AuctionRoleData temp in AuctionItem.RoleList)
					{
						long num = averageNum;
						if (AuctionItem.BuyerData.Value > 0L && AuctionItem.BossLife > 0L && temp.Value <= AuctionItem.BossLife)
						{
							num += (long)((double)temp.Value / (double)AuctionItem.BossLife * (double)AuctionItem.BuyerData.Value);
						}
						sendMsg.Add(temp, num);
					}
					foreach (KeyValuePair<AuctionRoleData, long> temp2 in sendMsg)
					{
						AuctionRoleData roleData = temp2.Key;
						int sendNum = (int)temp2.Value;
						try
						{
							if (sendNum > 0)
							{
								if (!Global.UseMailGivePlayerAward3(roleData.m_RoleID, null, AuctionCfg.SuccessTitle, string.Format(AuctionCfg.SuccessIntro, this.GetItemName(AuctionItem.StrGoods), sendNum), sendNum, 0, 0))
								{
									GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "邮件发放金团结算失败", roleData.m_RoleName, roleData.m_RoleName, "增加", sendNum, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, "");
									LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]邮件发放金团结算失败 send email roleid={0}, money={1}", roleData.m_RoleID, sendNum), null, true);
								}
								else
								{
									GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "邮件发放金团结算", "系统", roleData.m_RoleName, "增加", sendNum, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, null);
								}
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]发放奖励 send email roleid={0}, money={1}, {2}", roleData.m_RoleID, sendNum, ex.ToString()), null, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0002D6F0 File Offset: 0x0002B8F0
		public void SendItem(int roleID, AuctionRoleData roleData, GoldAuctionItem AuctionItem, AuctionConfig AuctionCfg)
		{
			lock (this.AuctionMutex)
			{
				try
				{
					if (roleID <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]SendItem roleid=0,ProductionTime={0},AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
					}
					else
					{
						List<GoodsData> goodsData = new List<GoodsData>();
						GoodsData goods = GlobalNew.ParseGoodsData(AuctionItem.StrGoods);
						if (null == goods)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]SendItem null == goods,ProductionTime={0},AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
						}
						else
						{
							goodsData.Add(goods);
							string ItemName = this.GetItemName(AuctionItem.StrGoods);
							if (null == roleData)
							{
								roleData = new AuctionRoleData();
								GameClient client = GameManager.ClientMgr.FindClient(roleID);
								if (client != null)
								{
									roleData.m_RoleID = roleID;
									roleData.strUserID = client.strUserID;
									roleData.m_RoleName = client.ClientData.RoleName;
									roleData.ZoneID = client.ClientData.ZoneID;
									roleData.ServerId = client.ServerId;
								}
							}
							string strTitle;
							string strIntro;
							if (roleID == AuctionItem.BuyerData.m_RoleID)
							{
								strTitle = "购买成功";
								strIntro = "在金团拍卖购买成功";
							}
							else
							{
								strTitle = AuctionCfg.FailTitle;
								strIntro = string.Format(AuctionCfg.FailIntro, ItemName);
							}
							if (!Global.UseMailGivePlayerAward3(roleID, goodsData, strTitle, strIntro, 0, 0, 0))
							{
								GameManager.logDBCmdMgr.AddMessageLog(-1, ItemName, "邮件发放金团结算失败", roleData.m_RoleName, roleData.m_RoleName, "增加", goods.GCount, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, AuctionItem.StrGoods);
								LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]SendItem 邮件发放金团结算失败 roleid={0}, item = {1}", roleID, AuctionItem.StrGoods), null, true);
							}
							else
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, ItemName, "邮件发放金团结算", "系统", roleData.m_RoleName, "增加", goods.GCount, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, null);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]SendItem send email roleid={0}, item = {1}", roleID, AuctionItem.StrGoods, ex.ToString()), null, true);
				}
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0002D990 File Offset: 0x0002BB90
		public void ReturnOldAuctionMoney(AuctionRoleData BuyerData, string StrGoods)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					int addMoney = (int)BuyerData.Value;
					if (!Global.UseMailGivePlayerAward3(BuyerData.m_RoleID, null, "竞拍失败", string.Format("您参与的活动奖励{0}，在竞价中被超过,返还您{1}钻石", this.GetItemName(StrGoods), addMoney), addMoney, 0, 0))
					{
						GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "邮件发放金团结算失败", BuyerData.m_RoleName, BuyerData.m_RoleName, "增加", addMoney, BuyerData.ZoneID, BuyerData.strUserID, -1, BuyerData.ServerId, "");
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]邮件发放,有新的竞价者,把钱返回给之前竞价者,失败 send email roleid={0}, money={1}", BuyerData.m_RoleID, addMoney), null, true);
					}
					else
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "邮件发放金团结算", "系统", BuyerData.m_RoleName, "增加", addMoney, BuyerData.ZoneID, BuyerData.strUserID, -1, BuyerData.ServerId, null);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0002DB00 File Offset: 0x0002BD00
		private string GetItemName(string strGoods)
		{
			try
			{
				return Global.GetGoodsName(GlobalNew.ParseGoodsData(strGoods));
			}
			catch
			{
			}
			return "道具";
		}

		// Token: 0x040003F0 RID: 1008
		private const int PageSetNum = 10;

		// Token: 0x040003F1 RID: 1009
		private const string msgFlag = "True";

		// Token: 0x040003F2 RID: 1010
		private object AuctionMutex = new object();

		// Token: 0x040003F3 RID: 1011
		private List<GoldAuctionItem> AuctionItemList;

		// Token: 0x040003F4 RID: 1012
		private Dictionary<string, AuctionS2CCache> S2CCache;
	}
}
