using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Reborn;
using GameServer.Logic.UserReturn;
using GameServer.Logic.YueKa;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.UserMoneyCharge
{
	
	public class UserMoneyMgr
	{
		
		public static UserMoneyMgr getInstance()
		{
			return UserMoneyMgr.instance;
		}

		
		public bool InitConfig()
		{
			this.InitFirstChargeConfigData();
			this.InitItemChargeConfigData();
			Dictionary<int, int> OpenStateDict = new Dictionary<int, int>();
			string strPlatformOpen = GameManager.systemParamsList.GetParamValueByName("JieRiChongZhiQiangGou");
			if (!string.IsNullOrEmpty(strPlatformOpen))
			{
				string[] Fields = strPlatformOpen.Split(new char[]
				{
					'|'
				});
				foreach (string dat in Fields)
				{
					string[] State = dat.Split(new char[]
					{
						','
					});
					if (State.Length == 2)
					{
						OpenStateDict[Global.SafeConvertToInt32(State[0])] = Global.SafeConvertToInt32(State[1]);
					}
				}
			}
			OpenStateDict.TryGetValue(this.GetActivityPlatformType(), out this.PlatformOpenStateVavle);
			GameManager.ClientMgr.NotifyAllActivityState(6, this.PlatformOpenStateVavle, "", "", 0);
			return true;
		}

		
		public void InitItemChargeConfigData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ZhiGou.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ZhiGou.xml"));
				if (xml != null)
				{
					Dictionary<int, ChargeItemData> ChargeItemDict = new Dictionary<int, ChargeItemData>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							ChargeItemData chargeItem = new ChargeItemData();
							chargeItem.ChargeItemID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhiGouID");
							chargeItem.ChargeDangID = (int)Global.GetSafeAttributeLong(xmlItem, "ChongZhiID");
							chargeItem.SinglePurchase = (int)Global.GetSafeAttributeLong(xmlItem, "SinglePurchase");
							chargeItem.DayPurchase = (int)Global.GetSafeAttributeLong(xmlItem, "EverydayPurchase");
							string ThemePurchase = Global.GetDefAttributeStr(xmlItem, "ThemePurchase", "");
							chargeItem.ThemePurchase = Global.SafeConvertToInt32(ThemePurchase);
							chargeItem.FromDate = Global.GetSafeAttributeStr(xmlItem, "FromDate");
							chargeItem.ToDate = Global.GetSafeAttributeStr(xmlItem, "ToDate");
							if (0 == chargeItem.FromDate.CompareTo("-1"))
							{
								chargeItem.FromDate = "2008-08-08 08:08:08";
							}
							if (0 == chargeItem.ToDate.CompareTo("-1"))
							{
								chargeItem.ToDate = "2028-08-08 08:08:08";
							}
							chargeItem.GoodsStringOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (!string.IsNullOrEmpty(chargeItem.GoodsStringOne))
							{
								string[] fields = chargeItem.GoodsStringOne.Split(new char[]
								{
									'|'
								});
								chargeItem.GoodsDataOne = HuodongCachingMgr.ParseGoodsDataList(fields, "ZhiGou.xml励配置文件1");
							}
							chargeItem.GoodsStringTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(chargeItem.GoodsStringTwo))
							{
								string[] fields = chargeItem.GoodsStringTwo.Split(new char[]
								{
									'|'
								});
								chargeItem.GoodsDataTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "ZhiGou.xml励配置文件1");
							}
							ChargeItemDict[chargeItem.ChargeItemID] = chargeItem;
						}
					}
					Data.ChargeItemDict = ChargeItemDict;
				}
				else
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("丢失充值直购配置文件{0}", "Config/ZhiGou.xml"), null, true);
				}
			}
			catch (Exception e)
			{
				LogManager.WriteLog(LogTypes.Fatal, "读取Config/ZhiGou.xml错误:" + e.ToString(), e, true);
			}
		}

		
		public void InitFirstChargeConfigData()
		{
			try
			{
				string strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
				strCmd = strCmd.ToLower();
				string sectionKey = string.Empty;
				ChargePlatformType cpt;
				if (strCmd == "app")
				{
					sectionKey = "dl_app";
					cpt = ChargePlatformType.CPT_App;
				}
				else if (strCmd == "yueyu")
				{
					sectionKey = "dl_yueyu";
					cpt = ChargePlatformType.CPT_YueYu;
				}
				else if (strCmd == "andrid" || strCmd == "android" || strCmd == "yyb")
				{
					sectionKey = "dl_android";
					cpt = ChargePlatformType.CPT_Android;
				}
				else
				{
					sectionKey = "dl_app";
					cpt = ChargePlatformType.CPT_App;
				}
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/MU_ChongZhi.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/MU_ChongZhi.xml"));
				if (xml != null)
				{
					IEnumerable<XElement> xmlItems = xml.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey).Elements();
					SingleChargeData chargeData = new SingleChargeData();
					chargeData.YueKaBangZuan = GameManager.PlatConfigMgr.GetGameConfigItemInt("YueKaBangZuan", 0);
					chargeData.ChargePlatType = (int)cpt;
					foreach (XElement item in xmlItems)
					{
						if (null != item)
						{
							int money = (int)Global.GetSafeAttributeLong(item, "RMB");
							int bindmoney = (int)Global.GetSafeAttributeLong(item, "FirstBindZuanShi");
							int id = (int)Global.GetSafeAttributeLong(item, "ID");
							if (id == YueKaManager.YUE_KA_MONEY_ID_IN_CHARGE_FILE)
							{
								chargeData.YueKaMoney = money;
							}
							chargeData.ChargeIDVsMoneyDict[id] = money;
							chargeData.MoneyVsChargeIDDict[money] = id;
							if (!chargeData.singleData.ContainsKey(money))
							{
								chargeData.singleData[money] = bindmoney;
							}
						}
					}
					JieriSuperInputActivity jsiAct = HuodongCachingMgr.GetJieRiSuperInputActivity();
					if (null != jsiAct)
					{
						jsiAct.FilterSingleChargeData(chargeData);
					}
					if (1 == Global.sendToDB<int, byte[]>(10171, DataHelper.ObjectToBytes<SingleChargeData>(chargeData), 0))
					{
						Data.ChargeData = chargeData;
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("丢失平台充值配置文件{0}", "Config/MU_ChongZhi.xml"), null, true);
				}
			}
			catch (Exception e)
			{
				LogManager.WriteException("读取充值xml错误：" + e.ToString());
			}
		}

		
		public void NotifyActivityState(GameClient client)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				6,
				this.PlatformOpenStateVavle,
				"",
				0,
				0
			});
			client.sendCmd(770, strcmd, false);
		}

		
		public int GetActivityPlatformType()
		{
			string strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
			strCmd = strCmd.ToLower();
			ZhiGouActPlatformType platform;
			if (strCmd == "app")
			{
				platform = ZhiGouActPlatformType.CZQG_App;
			}
			else if (strCmd == "yueyu")
			{
				platform = ZhiGouActPlatformType.CZQG_YueYu;
			}
			else if (strCmd == "andrid" || strCmd == "android")
			{
				platform = ZhiGouActPlatformType.CZQG_Android;
			}
			else if (strCmd == "yyb")
			{
				platform = ZhiGouActPlatformType.CZQG_YYB;
			}
			else
			{
				platform = ZhiGouActPlatformType.CZQG_App;
			}
			return (int)platform;
		}

		
		private void GiveClientChargeItem(GameClient client, List<GoodsData> awardList)
		{
			int outBag;
			if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, awardList, out outBag))
			{
				Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(553, new object[0]), GLang.GetLang(554, new object[0]), 0, 0, 0);
			}
			else
			{
				for (int i = 0; i < awardList.Count; i++)
				{
					GoodsData goodsData = awardList[i];
					if (null != goodsData)
					{
						goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "充值直购", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, 0, 0, null, null, 0, true);
					}
				}
			}
		}

		
		public int GetChargeItemPurchaseNum(GameClient client, int ZhiGouID)
		{
			int PurchaseNum = 0;
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				Dictionary<int, int> PurchaseDict = client.ClientData.ChargeItemPurchaseDict;
				PurchaseDict.TryGetValue(ZhiGouID, out PurchaseNum);
			}
			return PurchaseNum;
		}

		
		private void AddChargeItemPurchaseNum(GameClient client, int ZhiGouID)
		{
			int PurchaseNum = 0;
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				Dictionary<int, int> PurchaseDict = client.ClientData.ChargeItemPurchaseDict;
				PurchaseDict.TryGetValue(ZhiGouID, out PurchaseNum);
				PurchaseNum = (PurchaseDict[ZhiGouID] = PurchaseNum + 1);
			}
		}

		
		public int GetChargeItemDayPurchaseNum(GameClient client, int ZhiGouID)
		{
			int PurchaseNum = 0;
			lock (client.ClientData.ChargeItemDayPurchaseDict)
			{
				int ChargeDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				if (client.ClientData.ChargeItemDayPurchaseDayID != ChargeDayID)
				{
					client.ClientData.ChargeItemDayPurchaseDict.Clear();
					client.ClientData.ChargeItemDayPurchaseDayID = ChargeDayID;
				}
				Dictionary<int, int> PurchaseDict = client.ClientData.ChargeItemDayPurchaseDict;
				PurchaseDict.TryGetValue(ZhiGouID, out PurchaseNum);
			}
			return PurchaseNum;
		}

		
		private void AddChargeItemDayPurchaseNum(GameClient client, int ZhiGouID, string chargeTime)
		{
			DateTime ChargeTm;
			DateTime.TryParse(chargeTime, out ChargeTm);
			int ChargeDayID = TimeUtil.GetOffsetDay(ChargeTm);
			if (client.ClientData.ChargeItemDayPurchaseDayID == ChargeDayID)
			{
				int PurchaseNum = 0;
				lock (client.ClientData.ChargeItemDayPurchaseDict)
				{
					Dictionary<int, int> PurchaseDict = client.ClientData.ChargeItemDayPurchaseDict;
					PurchaseDict.TryGetValue(ZhiGouID, out PurchaseNum);
					PurchaseNum = (PurchaseDict[ZhiGouID] = PurchaseNum + 1);
				}
			}
		}

		
		public bool CheckChargeItemConfigLogic(int zhigouID, int PurchaseNum, string GoodsOne, string GoodsTwo, string FromSystem)
		{
			Dictionary<int, ChargeItemData> ChargeItemDict = Data.ChargeItemDict;
			bool result;
			if (null == ChargeItemDict)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：充值直购文件尚未成功加载", new object[0]), null, true);
				result = false;
			}
			else
			{
				ChargeItemData configdata = null;
				if (!ChargeItemDict.TryGetValue(zhigouID, out configdata))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：充值直购配置错误 zhigouID={0} fromSys={1}", zhigouID, FromSystem), null, true);
					result = false;
				}
				else if (configdata.SinglePurchase != PurchaseNum && configdata.DayPurchase != PurchaseNum && configdata.ThemePurchase != PurchaseNum)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：充值直购配置错误 Purchase={0} fromSys={1}", PurchaseNum, FromSystem), null, true);
					result = false;
				}
				else if (configdata.GoodsStringOne != GoodsOne || configdata.GoodsStringTwo != GoodsTwo)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：充值直购配置错误 GoodsOne={0} GoodsTwo={1} fromSys={2}", GoodsOne, GoodsTwo, FromSystem), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		private void ComputeClientChargeItem(GameClient client, List<TempItemChargeInfo> list)
		{
			lock (client.ClientData.ChargeItemPurchaseDict)
			{
				client.ClientData.ChargeItemPurchaseDict.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					TempItemChargeInfo tempitem = list[i];
					if (tempitem.isDel != 0 && tempitem.isDel != 2)
					{
						int PurchaseNum = 0;
						client.ClientData.ChargeItemPurchaseDict.TryGetValue(tempitem.zhigouID, out PurchaseNum);
						PurchaseNum = (client.ClientData.ChargeItemPurchaseDict[tempitem.zhigouID] = PurchaseNum + 1);
					}
				}
			}
			lock (client.ClientData.ChargeItemDayPurchaseDict)
			{
				client.ClientData.ChargeItemDayPurchaseDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				client.ClientData.ChargeItemDayPurchaseDict.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					TempItemChargeInfo tempitem = list[i];
					if (tempitem.isDel != 0 && tempitem.isDel != 2)
					{
						int PurchaseNum = 0;
						DateTime ChargeTm;
						DateTime.TryParse(tempitem.chargeTime, out ChargeTm);
						int ChargeDayID = TimeUtil.GetOffsetDay(ChargeTm);
						if (ChargeDayID == client.ClientData.ChargeItemDayPurchaseDayID)
						{
							client.ClientData.ChargeItemDayPurchaseDict.TryGetValue(tempitem.zhigouID, out PurchaseNum);
							PurchaseNum = (client.ClientData.ChargeItemDayPurchaseDict[tempitem.zhigouID] = PurchaseNum + 1);
						}
					}
				}
			}
		}

		
		public void HandleSystemChargeMoney(string userID, int addMoney)
		{
			TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
			if (null != clientSocket)
			{
				LogManager.WriteLog(LogTypes.SQL, string.Format("通知账户ID{0}的角色更新元宝数量", userID), null, true);
				GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
				if (null != otherClient)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, 0, "", ActivityTypes.None, "");
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "GM命令强迫更新", "系统", otherClient.ClientData.RoleName, "增加", addMoney, otherClient.ClientData.ZoneID, otherClient.strUserID, otherClient.ClientData.UserMoney, otherClient.ServerId, null);
				}
			}
		}

		
		public void HandleClientChargeMoney(string userID, int rid, int addMoney, bool transmit, int superInputFanLi = 0)
		{
			JieriIPointsExchgActivity act = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			if (null != act)
			{
				act.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			EverydayActivity everyAct = HuodongCachingMgr.GetEverydayActivity();
			if (null != everyAct)
			{
				everyAct.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
			if (null != everyAct)
			{
				spAct.OnMoneyChargeEvent(userID, rid, addMoney);
			}
			if (superInputFanLi > 0)
			{
				JieriSuperInputActivity siAct = HuodongCachingMgr.GetJieRiSuperInputActivity();
				if (null != siAct)
				{
					siAct.OnMoneyChargeEvent(userID, rid, addMoney, superInputFanLi);
				}
			}
			TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
			if (null == clientSocket)
			{
				SpecialActivity specAct = HuodongCachingMgr.GetSpecialActivity();
				if (null != specAct)
				{
					specAct.OnMoneyChargeEventOffLine(userID, rid, addMoney);
				}
			}
			else
			{
				if (transmit)
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("通知账户ID{0}的角色更新元宝数量", userID), null, true);
				}
				GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
				if (null != otherClient)
				{
					SpecialActivity specAct = HuodongCachingMgr.GetSpecialActivity();
					if (null != specAct)
					{
						specAct.OnMoneyChargeEventOnLine(otherClient, addMoney);
					}
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, 0, "", ActivityTypes.None, "");
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "GM命令强迫更新", "系统", otherClient.ClientData.RoleName, "增加", addMoney, otherClient.ClientData.ZoneID, otherClient.strUserID, otherClient.ClientData.UserMoney, otherClient.ServerId, null);
					otherClient._IconStateMgr.FlushChongZhiIconState(otherClient);
					otherClient._IconStateMgr.CheckJieRiActivity(otherClient, false);
					otherClient._IconStateMgr.SendIconStateToClient(otherClient);
					UserReturnManager.getInstance().initUserReturnData(otherClient);
					SingletonTemplate<SevenDayActivityMgr>.Instance().OnCharge(otherClient);
					FundManager.initFundData(otherClient);
				}
				else
				{
					SpecialActivity specAct = HuodongCachingMgr.GetSpecialActivity();
					if (null != specAct)
					{
						specAct.OnMoneyChargeEventOffLine(userID, rid, addMoney);
					}
				}
			}
		}

		
		public void HandleClientChargeItem(GameClient client, byte HandleDel = 0)
		{
			if (client != null && !GameManager.IsKuaFuServer)
			{
				lock (client.ClientData.ChargeItemMutex)
				{
					List<TempItemChargeInfo> list = new List<TempItemChargeInfo>();
					string cmd = string.Format("{0}:{1}:{2}", client.strUserID, client.ClientData.RoleID, HandleDel);
					list = Global.sendToDB<List<TempItemChargeInfo>, string>(13320, cmd, client.ServerId);
					if (list != null && list.Count != 0)
					{
						if (HandleDel == 1)
						{
							this.ComputeClientChargeItem(client, list);
						}
						Dictionary<int, ChargeItemData> ChargeItemDict = Data.ChargeItemDict;
						for (int i = 0; i < list.Count; i++)
						{
							TempItemChargeInfo tempitem = list[i];
							if (tempitem.isDel == 0)
							{
								int ProcessChargeMoney = 0;
								int ReturnUserMoney = 0;
								ChargeItemData configdata = null;
								if (!ChargeItemDict.TryGetValue(tempitem.zhigouID, out configdata))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("充值直购zhigouID错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
									{
										tempitem.userID,
										tempitem.chargeRoleID,
										tempitem.addUserMoney,
										tempitem.zhigouID,
										tempitem.chargeTime
									}), null, true);
									ProcessChargeMoney = 1;
								}
								else if (string.Compare(tempitem.chargeTime, configdata.FromDate) < 0 || string.Compare(tempitem.chargeTime, configdata.ToDate) > 0)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("充值直购Time错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
									{
										tempitem.userID,
										tempitem.chargeRoleID,
										tempitem.addUserMoney,
										tempitem.zhigouID,
										tempitem.chargeTime
									}), null, true);
									ProcessChargeMoney = 1;
								}
								else
								{
									int Money = 0;
									if (!Data.ChargeData.ChargeIDVsMoneyDict.TryGetValue(configdata.ChargeDangID, out Money))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("充值直购Dang错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
										{
											tempitem.userID,
											tempitem.chargeRoleID,
											tempitem.addUserMoney,
											tempitem.zhigouID,
											tempitem.chargeTime
										}), null, true);
										ProcessChargeMoney = 1;
									}
									else
									{
										int ZhiGouImplicit = GameManager.PlatConfigMgr.GetGameConfigItemInt("zhigou_implicit", 0);
										if (ZhiGouImplicit > 0)
										{
											if (tempitem.addUserMoney < Money)
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("充值直购Money错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
												{
													tempitem.userID,
													tempitem.chargeRoleID,
													tempitem.addUserMoney,
													tempitem.zhigouID,
													tempitem.chargeTime
												}), null, true);
												ProcessChargeMoney = 1;
											}
											else
											{
												ReturnUserMoney = Math.Max(tempitem.addUserMoney - Money, 0);
											}
										}
										else if (tempitem.addUserMoney != Money)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("充值直购Money错误 uid={0}，rid={1}，money={2}，itemid={3}，chargeTm={4}", new object[]
											{
												tempitem.userID,
												tempitem.chargeRoleID,
												tempitem.addUserMoney,
												tempitem.zhigouID,
												tempitem.chargeTime
											}), null, true);
											ProcessChargeMoney = 1;
										}
									}
								}
								int PurchaseNum = this.GetChargeItemPurchaseNum(client, tempitem.zhigouID);
								int DayPurchaseNum = this.GetChargeItemDayPurchaseNum(client, tempitem.zhigouID);
								if (configdata != null && configdata.SinglePurchase > 0 && PurchaseNum >= configdata.SinglePurchase)
								{
									ProcessChargeMoney = 1;
								}
								if (configdata != null && configdata.DayPurchase > 0)
								{
									EverydayActivity everyAct = HuodongCachingMgr.GetEverydayActivity();
									if (everyAct == null || !everyAct.CheckValidChargeItem(tempitem.zhigouID))
									{
										ProcessChargeMoney = 1;
									}
									if (DayPurchaseNum >= configdata.DayPurchase)
									{
										ProcessChargeMoney = 1;
									}
								}
								if (configdata != null && configdata.ThemePurchase > 0)
								{
									ThemeZhiGouActivity zhigouAct = HuodongCachingMgr.GetThemeZhiGouActivity();
									if (zhigouAct == null || !zhigouAct.CheckValidChargeItem(tempitem.zhigouID))
									{
										ProcessChargeMoney = 1;
									}
									if (DayPurchaseNum >= configdata.ThemePurchase)
									{
										ProcessChargeMoney = 1;
									}
								}
								SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
								if (spAct != null && !spAct.CheckValidChargeItem(tempitem.zhigouID))
								{
									ProcessChargeMoney = 1;
								}
								RegressActiveDayBuy radbAct = HuodongCachingMgr.GetRegressActiveDayBuy();
								if (radbAct != null && !radbAct.CheckValidChargeItem(client, tempitem.zhigouID))
								{
									ProcessChargeMoney = 1;
								}
								cmd = string.Format("{0}:{1}:{2}", tempitem.ID, ProcessChargeMoney, ReturnUserMoney);
								string[] dbFields = null;
								TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 13321, cmd, out dbFields, client.ServerId);
								if (dbRequestResult != TCPProcessCmdResults.RESULT_FAILED && dbFields.Length > 0)
								{
									if (ProcessChargeMoney == 0)
									{
										this.AddChargeItemPurchaseNum(client, tempitem.zhigouID);
										this.AddChargeItemDayPurchaseNum(client, tempitem.zhigouID, tempitem.chargeTime);
									}
									if (configdata != null && ProcessChargeMoney == 0)
									{
										ChargeItemBaseEventObject evObj_ChargeItem = new ChargeItemBaseEventObject(client, configdata);
										GlobalEventSource.getInstance().fireEvent(evObj_ChargeItem);
									}
									if (ProcessChargeMoney != 1)
									{
										if (null != configdata)
										{
											List<GoodsData> awardList = new List<GoodsData>();
											if (configdata.GoodsDataOne != null)
											{
												awardList.AddRange(configdata.GoodsDataOne);
											}
											List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, configdata.GoodsDataTwo);
											if (proGoods != null)
											{
												awardList.AddRange(proGoods);
											}
											this.GiveClientChargeItem(client, awardList);
											this.HandleClientChargeMoney(client.strUserID, client.ClientData.RoleID, tempitem.addUserMoney - ReturnUserMoney, false, 0);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void ProcessSendBindGold(GameClient client, int bindMoney, string userId, int rid, string firstbindData)
		{
			if (client != null)
			{
				GameManager.ClientMgr.SendToClient(client, firstbindData, 671);
				GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindMoney, "首次充值送绑钻(在线)");
			}
			else
			{
				GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, rid, bindMoney, "首次充值送绑钻(离线)", userId);
			}
		}

		
		public TCPProcessCmdResults ProcessGetFirstChargeInfoCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string[] dbFields = null;
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 671, client.strUserID, out dbFields, client.ServerId);
				if (null == dbFields)
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string returndata = dbFields[0];
				if (returndata == "-1")
				{
					returndata = "";
				}
				string strcmd = string.Format("{0}", returndata);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetFirstChargeInfoCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static UserMoneyMgr instance = new UserMoneyMgr();

		
		public int PlatformOpenStateVavle = 0;
	}
}
