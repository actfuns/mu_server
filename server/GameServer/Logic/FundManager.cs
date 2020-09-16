using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class FundManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static FundManager getInstance()
		{
			return FundManager.instance;
		}

		
		public bool initialize()
		{
			FundManager.InitConfig();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1032, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1033, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1034, 1, 1, FundManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1032:
				result = this.ProcessFundInfoCmd(client, nID, bytes, cmdParams);
				break;
			case 1033:
				result = this.ProcessFundBuyCmd(client, nID, bytes, cmdParams);
				break;
			case 1034:
				result = this.ProcessFundAwardCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		public bool ProcessFundInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				FundData data = FundManager.FundGetData(client);
				client.sendCmd<FundData>(1032, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessFundBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int fundType = int.Parse(cmdParams[0]);
				FundData data = FundManager.FundBuy(client, fundType);
				client.sendCmd<FundData>(1033, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessFundAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int fundType = int.Parse(cmdParams[0]);
				FundData data = FundManager.FundAward(client, fundType);
				client.sendCmd<FundData>(1034, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private static FundData FundGetData(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			bool isOpen = FundManager.IsGongNengOpened(client, false);
			if (fundData.IsOpen != isOpen)
			{
				FundManager.initFundData(client);
			}
			return FundManager.GetFundData(client);
		}

		
		private static FundData FundBuy(GameClient client, int fundType)
		{
			FundData myData = FundManager.GetFundData(client);
			FundData result;
			if (!myData.IsOpen)
			{
				result = myData;
			}
			else
			{
				myData.FundType = fundType;
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot9))
				{
					myData.State = -2;
					result = myData;
				}
				else if (!myData.FundDic.ContainsKey(fundType))
				{
					myData.State = -1;
					result = myData;
				}
				else
				{
					FundItem myItem = myData.FundDic[fundType];
					if (myItem.BuyType == 1)
					{
						myData.State = -4;
						result = myData;
					}
					else if (myItem.BuyType == 3)
					{
						myData.State = -5;
						result = myData;
					}
					else
					{
						FundInfo fundInfo = FundManager._fundDic[myItem.FundID];
						if (fundInfo.Price > client.ClientData.UserMoney)
						{
							myData.State = -3;
							result = myData;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, fundInfo.Price, "基金购买", true, false, false, DaiBiSySType.None))
						{
							myData.State = -1;
							result = myData;
						}
						else
						{
							DateTime buyTime = TimeUtil.NowDateTime();
							if (!FundManager.DBFundBuy(client, new FundDBItem
							{
								zoneID = client.ClientData.ZoneID,
								UserID = client.strUserID,
								RoleID = client.ClientData.RoleID,
								FundType = myData.FundType,
								FundID = myItem.FundID,
								BuyTime = buyTime,
								State = 1
							}))
							{
								myData.State = -1;
								result = myData;
							}
							else
							{
								myItem.BuyType = 1;
								myItem.BuyTime = buyTime;
								if (myItem.FundType == 2)
								{
									myItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(myItem.BuyTime) + 1;
								}
								FundAwardInfo awardInfo = FundManager._fundAwardDic[myItem.AwardID];
								if (myItem.Value1 >= awardInfo.Value1 && myItem.Value2 >= awardInfo.Value2)
								{
									myItem.AwardType = 2;
								}
								else
								{
									myItem.AwardType = 3;
								}
								myData.State = 1;
								myData.FundType = fundType;
								FundManager.CheckActivityTip(client);
								result = myData;
							}
						}
					}
				}
			}
			return result;
		}

		
		private static FundData FundAward(GameClient client, int fundType)
		{
			FundData myData = FundManager.GetFundData(client);
			FundData result;
			if (!myData.IsOpen)
			{
				result = myData;
			}
			else
			{
				myData.FundType = fundType;
				if (!myData.FundDic.ContainsKey(fundType))
				{
					myData.State = -1;
					result = myData;
				}
				else
				{
					FundItem myItem = myData.FundDic[fundType];
					if (myItem.BuyType != 1)
					{
						myData.State = -8;
						result = myData;
					}
					else if (myItem.AwardType == 3)
					{
						myData.State = -6;
						result = myData;
					}
					else if (myItem.AwardType == 1)
					{
						myData.State = -7;
						result = myData;
					}
					else
					{
						DateTime buyTime = TimeUtil.NowDateTime();
						FundDBItem dbItem = new FundDBItem();
						dbItem.zoneID = client.ClientData.ZoneID;
						dbItem.UserID = client.strUserID;
						dbItem.RoleID = client.ClientData.RoleID;
						dbItem.FundType = myData.FundType;
						dbItem.FundID = myItem.FundID;
						dbItem.BuyTime = buyTime;
						dbItem.AwardID = myItem.AwardID;
						int fundState = 1;
						bool isAwardMax = (from info in FundManager._fundAwardDic.Values
						where info.FundType == myItem.FundType && info.FundID == myItem.FundID && info.AwardID > myItem.AwardID
						select info).Any<FundAwardInfo>();
						bool isFundMax = (from info in FundManager._fundDic.Values
						where info.FundType == myItem.FundType && info.FundID > myItem.FundID
						select info).Any<FundInfo>();
						if (!isAwardMax && !isFundMax)
						{
							fundState = 2;
						}
						dbItem.State = fundState;
						if (!FundManager.DBFundAward(client, dbItem))
						{
							myData.State = -1;
							result = myData;
						}
						else
						{
							FundAwardInfo awardInfo = FundManager._fundAwardDic[myItem.AwardID];
							if (!FundManager.AddDiamone(client, awardInfo.AwardIsBind, awardInfo.AwardCount))
							{
								myData.State = -1;
								result = myData;
							}
							else
							{
								myData.State = 1;
								if (dbItem.State == 2)
								{
									myItem.AwardType = 1;
									FundManager.CheckActivityTip(client);
									result = myData;
								}
								else
								{
									FundManager.initFundAwardNext(client, myItem);
									FundManager.CheckActivityTip(client);
									myData.FundType = fundType;
									result = myData;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		private static bool AddDiamone(GameClient client, bool isBind, int diamond)
		{
			bool result;
			if (isBind)
			{
				result = GameManager.ClientMgr.AddUserGold(client, diamond, "基金绑钻");
			}
			else
			{
				result = GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, diamond, "基金钻石", ActivityTypes.None, "");
			}
			return result;
		}

		
		public static FundData GetFundData(GameClient client)
		{
			FundData myFundData;
			lock (client.ClientData.LockFund)
			{
				myFundData = client.ClientData.MyFundData;
			}
			return myFundData;
		}

		
		public static void initFundData(GameClient client)
		{
			lock (client.ClientData.LockFund)
			{
				FundData fundData = new FundData();
				if (!FundManager.IsGongNengOpened(client, false))
				{
					client.ClientData.MyFundData = fundData;
				}
				else
				{
					fundData.IsOpen = true;
					fundData.FundDic.Add(1, FundManager.initFundItem(client, EFund.ChangeLife));
					fundData.FundDic.Add(2, FundManager.initFundItem(client, EFund.Login));
					fundData.FundDic.Add(3, FundManager.initFundItem(client, EFund.Money));
					List<FundDBItem> dbItemList = FundManager.DBFundInfo(client);
					if (dbItemList == null)
					{
						client.ClientData.MyFundData = fundData;
					}
					else
					{
						foreach (FundDBItem dbItem in dbItemList)
						{
							if (fundData.FundDic.ContainsKey(dbItem.FundType) && dbItem.State > 0)
							{
								FundItem fundItem = fundData.FundDic[dbItem.FundType];
								fundItem.BuyType = 1;
								fundItem.BuyTime = dbItem.BuyTime;
								fundItem.FundID = dbItem.FundID;
								fundItem.AwardID = dbItem.AwardID;
								fundItem.AwardType = 1;
								if (fundItem.FundType == 3)
								{
									fundItem.Value1 = dbItem.Value1;
									fundItem.Value2 = dbItem.Value2;
								}
								if (fundItem.FundType == 2 && fundItem.BuyTime > DateTime.MinValue)
								{
									fundItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(fundItem.BuyTime) + 1;
								}
								if (dbItem.State == 1)
								{
									FundManager.initFundAwardNext(client, fundItem);
								}
							}
						}
						client.ClientData.MyFundData = fundData;
						FundManager.CheckActivityTip(client);
					}
				}
			}
		}

		
		private static FundItem initFundItem(GameClient client, EFund fundType)
		{
            FundItem result;
			lock(client.ClientData.LockFund)
			{
                FundInfo fundInfo = (from info in FundManager._fundDic.Values
				where info.FundType == (int)fundType
				orderby info.FundID
				select info).First<FundInfo>();
				FundItem item = new FundItem();
				item.FundID = fundInfo.FundID;
				item.FundType = (int)fundType;
				item.BuyType = 3;
				if (client.ClientData.VipLevel >= fundInfo.MinVip)
				{
					item.BuyType = 2;
				}
				FundAwardInfo awardInfo = (from info in FundManager._fundAwardDic.Values
				where info.FundType == (int)fundType && info.FundID == fundInfo.FundID
				orderby info.AwardID
				select info).First<FundAwardInfo>();
				item.AwardID = awardInfo.AwardID;
				item.AwardType = 3;
				FundManager.checkFundItemValue(client, item);
				result = item;
			}
            return result;
		}

		
		private static void checkFundItemValue(GameClient client, FundItem fundItem)
		{
			lock (client.ClientData.LockFund)
			{
				switch (fundItem.FundType)
				{
				case 1:
					fundItem.Value1 = client.ClientData.ChangeLifeCount;
					fundItem.Value2 = client.ClientData.Level;
					break;
				case 2:
					if (fundItem.BuyTime > DateTime.MinValue)
					{
						fundItem.Value1 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(fundItem.BuyTime) + 1;
					}
					break;
				}
			}
		}

		
		private static void initFundAwardNext(GameClient client, FundItem fundItem)
		{
			lock (client.ClientData.LockFund)
			{
				IOrderedEnumerable<FundAwardInfo> tempAwardList = from info in FundManager._fundAwardDic.Values
				where info.FundType == fundItem.FundType && info.FundID == fundItem.FundID && info.AwardID > fundItem.AwardID
				orderby info.AwardID
				select info;
				if (tempAwardList.Any<FundAwardInfo>())
				{
					FundAwardInfo awardInfo = tempAwardList.First<FundAwardInfo>();
					fundItem.AwardID = awardInfo.AwardID;
					if (fundItem.Value1 >= awardInfo.Value1 && fundItem.Value2 >= awardInfo.Value2)
					{
						fundItem.AwardType = 2;
					}
					else
					{
						fundItem.AwardType = 3;
					}
					return;
				}
			}
			IOrderedEnumerable<FundInfo> tempFundList = from info in FundManager._fundDic.Values
			where info.FundType == fundItem.FundType && info.FundID > fundItem.FundID
			orderby info.FundID
			select info;
			if (tempFundList.Any<FundInfo>())
			{
				FundInfo fundInfo = tempFundList.First<FundInfo>();
				fundItem.FundID = fundInfo.FundID;
				fundItem.BuyTime = DateTime.MinValue;
				fundItem.BuyType = 3;
				if (client.ClientData.VipLevel >= fundInfo.MinVip)
				{
					fundItem.BuyType = 2;
				}
				FundAwardInfo awardInfo = (from award in FundManager._fundAwardDic.Values
				where award.FundType == fundItem.FundType && award.FundID == fundItem.FundID && award.AwardID > fundItem.AwardID
				orderby award.AwardID
				select award).First<FundAwardInfo>();
				fundItem.AwardID = awardInfo.AwardID;
				fundItem.AwardType = 3;
				fundItem.Value1 = 0;
				fundItem.Value2 = 0;
			}
		}

		
		private static List<FundDBItem> DBFundInfo(GameClient client)
		{
			List<FundDBItem> list = new List<FundDBItem>();
			return Global.sendToDB<List<FundDBItem>, int>(13116, client.ClientData.RoleID, client.ServerId);
		}

		
		private static bool DBFundBuy(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13117, item, client.ServerId);
		}

		
		private static bool DBFundAward(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13118, item, client.ServerId);
		}

		
		private static bool DBFundMoney(GameClient client, FundDBItem item)
		{
			return Global.sendToDB<bool, FundDBItem>(13119, item, client.ServerId);
		}

		
		public static bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Fund") && GlobalNew.IsGongNengOpened(client, GongNengIDs.Fund, hint);
		}

		
		public static void FundChangeLife(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				if (fundData.FundDic.ContainsKey(1))
				{
					FundItem fundItem = fundData.FundDic[1];
					fundItem.Value1 = client.ClientData.ChangeLifeCount;
					fundItem.Value2 = client.ClientData.Level;
					if (fundItem.BuyType == 1 && fundItem.AwardType == 3)
					{
						FundAwardInfo awardInfo = FundManager._fundAwardDic[fundItem.AwardID];
						if (fundItem.Value1 >= awardInfo.Value1 && fundItem.Value2 >= awardInfo.Value2)
						{
							fundItem.AwardType = 2;
							FundManager.CheckActivityTip(client);
						}
					}
				}
			}
		}

		
		public static void FundVip(GameClient client)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				bool isTip = false;
				foreach (FundItem item in fundData.FundDic.Values)
				{
					if (item.BuyType == 3)
					{
						FundInfo info = FundManager._fundDic[item.FundID];
						if (client.ClientData.VipLevel >= info.MinVip)
						{
							item.BuyType = 2;
							isTip = true;
						}
					}
				}
				if (isTip)
				{
					FundManager.CheckActivityTip(client);
				}
			}
		}

		
		public static void FundMoneyCost(GameClient client, int moneyCost)
		{
			FundData fundData = FundManager.GetFundData(client);
			if (fundData != null && fundData.IsOpen)
			{
				if (fundData.FundDic.ContainsKey(3))
				{
					FundItem fundItem = fundData.FundDic[3];
					if (fundItem.BuyType == 1)
					{
						if (FundManager.DBFundMoney(client, new FundDBItem
						{
							UserID = client.strUserID,
							RoleID = client.ClientData.RoleID,
							Value1 = 0,
							Value2 = moneyCost
						}))
						{
							fundItem.Value2 += moneyCost;
							FundAwardInfo awardInfo = FundManager._fundAwardDic[fundItem.AwardID];
							if (fundItem.AwardType == 3 && fundItem.Value1 >= awardInfo.Value1 && fundItem.Value2 >= awardInfo.Value2)
							{
								fundItem.AwardType = 2;
								FundManager.CheckActivityTip(client);
							}
						}
					}
				}
			}
		}

		
		private static bool InitConfig()
		{
			string fileName = "";
			try
			{
				FundManager._fundDic.Clear();
				fileName = Global.GameResPath("Config/Fund/FundSet.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						FundInfo config = new FundInfo();
						config.FundID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MainID", "0"));
						config.FundType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "PageID", "0"));
						config.MinVip = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinVip", "0"));
						config.NextID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NextID", "0"));
						config.Price = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Price", "0"));
						FundManager._fundDic.Add(config.FundID, config);
					}
				}
				FundManager._fundAwardDic.Clear();
				fileName = Global.GameResPath("Config/Fund/Fund.xml");
				xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						FundAwardInfo awardConfig = new FundAwardInfo();
						awardConfig.AwardID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
						awardConfig.FundID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MainID", "0"));
						awardConfig.FundType = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoalType", "0"));
						awardConfig.AwardIsBind = (Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RewardType", "0")) > 0);
						awardConfig.AwardCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RewardCount", "0"));
						string[] numArr = Global.GetDefAttributeStr(xmlItem, "GoalNum", "0,0").Split(new char[]
						{
							','
						});
						switch (awardConfig.FundType)
						{
						case 1:
							awardConfig.Value1 = int.Parse(numArr[0]);
							awardConfig.Value2 = int.Parse(numArr[1]);
							break;
						case 2:
							awardConfig.Value1 = int.Parse(numArr[0]);
							break;
						case 3:
							awardConfig.Value1 = int.Parse(numArr[0]);
							awardConfig.Value2 = int.Parse(numArr[1]);
							break;
						}
						FundManager._fundAwardDic.Add(awardConfig.AwardID, awardConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				return false;
			}
			return true;
		}

		
		private static void CheckActivityTip(GameClient client)
		{
			lock (client.ClientData.LockFund)
			{
				FundData fundData = FundManager.GetFundData(client);
				if (fundData.IsOpen)
				{
					bool isChange = false;
					bool isAll = false;
					List<int> tipTypeList = new List<int>();
					foreach (FundItem item in fundData.FundDic.Values)
					{
						bool tip = item.BuyType == 2 || item.AwardType == 2;
						isAll = (isAll || tip);
						switch (item.FundType)
						{
						case 1:
							isChange |= client._IconStateMgr.AddFlushIconState(14106, tip);
							break;
						case 2:
							isChange |= client._IconStateMgr.AddFlushIconState(14107, tip);
							break;
						case 3:
							isChange |= client._IconStateMgr.AddFlushIconState(14108, tip);
							break;
						}
					}
					isChange |= client._IconStateMgr.AddFlushIconState(14109, isAll);
					if (isChange)
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		
		private static FundManager instance = new FundManager();

		
		private static Dictionary<int, FundInfo> _fundDic = new Dictionary<int, FundInfo>();

		
		private static Dictionary<int, FundAwardInfo> _fundAwardDic = new Dictionary<int, FundAwardInfo>();
	}
}
