using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic.Fund;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class UserMoneyMgr
	{
		
		public static void UpdateUsersMoney(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - UserMoneyMgr.LastUpdateUserMoneyTicks >= 2000L)
			{
				UserMoneyMgr.LastUpdateUserMoneyTicks = nowTicks;
				SingleChargeData chargeData = CFirstChargeMgr.ChargeData;
				if (chargeData == null)
				{
					if (!UserMoneyMgr.ChargeDataLogState)
					{
						UserMoneyMgr.ChargeDataLogState = true;
						LogManager.WriteLog(LogTypes.Error, "处理充值时找不到ChargeData, " + DateTime.Now.ToString(), null, true);
					}
				}
				else
				{
					if (UserMoneyMgr.ChargeDataLogState)
					{
						UserMoneyMgr.ChargeDataLogState = false;
						LogManager.WriteLog(LogTypes.Error, "处理充值时已经获取到了ChargeData, " + DateTime.Now.ToString(), null, true);
					}
					List<TempMoneyInfo> tempMoneyInfoList = new List<TempMoneyInfo>();
					DBQuery.QueryTempMoney(dbMgr, tempMoneyInfoList);
					if (tempMoneyInfoList.Count > 0)
					{
						for (int i = 0; i < tempMoneyInfoList.Count; i++)
						{
							string userID = tempMoneyInfoList[i].userID;
							int chargeRoleID = tempMoneyInfoList[i].chargeRoleID;
							int addUserMoney = tempMoneyInfoList[i].addUserMoney;
							int zhigouID = tempMoneyInfoList[i].addUserItem;
							string chargeTm = tempMoneyInfoList[i].chargeTm;
							LogManager.WriteLog(LogTypes.Error, string.Format("正在处理充值 UID={0}，money={1}，itemid={2}", userID, addUserMoney, zhigouID), null, true);
							DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
							if (dbUserInfo == null)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("处理充值时找不到user, UID={0}，money={1}，itemid={2}", userID, addUserMoney, zhigouID), null, true);
							}
							else if (zhigouID != 0 && !dbUserInfo.ListRoleIDs.Contains(chargeRoleID))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("处理充值直购时user内找不到rid, UID={0}，rid={1}，money={2}，itemid={3}", new object[]
								{
									userID,
									chargeRoleID,
									addUserMoney,
									zhigouID
								}), null, true);
							}
							else
							{
								UserMoneyMgr._ProcessCharge(dbMgr, dbUserInfo, chargeRoleID, addUserMoney, zhigouID, chargeTm, chargeData, false);
								if (zhigouID == 0 && addUserMoney == chargeData.YueKaMoney && chargeData.YueKaMoney > 0)
								{
									UserMoneyMgr._ProcessBuyYueKa(dbMgr, dbUserInfo);
								}
							}
						}
					}
				}
			}
		}

		
		private static void _ProcessBuyItem(DBManager dbMgr, DBUserInfo dbUserInfo, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm)
		{
			DBWriter.InsertChargeTempItem(dbMgr, dbUserInfo.UserID, chargeRoleID, addUserMoney, zhigouID, chargeTm);
		}

		
		private static JieriSuperInputData GetJieriSuperInputDataByChargeTm(SingleChargeData chargeData, string chargeTm)
		{
			JieriSuperInputData configData = null;
			DateTime chargeTime = DateTime.Parse(chargeTm);
			foreach (JieriSuperInputData config in chargeData.SuperInputFanLiDict.Values)
			{
				if (chargeTime >= config.BeginTime && chargeTime <= config.EndTime)
				{
					configData = config;
					break;
				}
			}
			return configData;
		}

		
		private static int _ProcessSuperInputFanLi(DBManager dbMgr, DBUserInfo dbUserInfo, SingleChargeData chargeData, int addUserMoney, int ChargeID, string chargeTm)
		{
			try
			{
				string SuperInputFanLiKey = chargeData.SuperInputFanLiKey;
				if (string.IsNullOrEmpty(SuperInputFanLiKey))
				{
					return 0;
				}
				string[] KeyFileds = SuperInputFanLiKey.Split(new char[]
				{
					'_'
				});
				if (KeyFileds.Length != 2)
				{
					return 0;
				}
				DateTime startTime = DateTime.Parse(KeyFileds[0]);
				DateTime endTime = DateTime.Parse(KeyFileds[1]);
				if (TimeUtil.NowDateTime() < startTime || TimeUtil.NowDateTime() > endTime)
				{
					return 0;
				}
				JieriSuperInputData configData = UserMoneyMgr.GetJieriSuperInputDataByChargeTm(chargeData, chargeTm);
				if (null == configData)
				{
					return 0;
				}
				string beginStr = configData.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string endStr = configData.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keyStr = string.Format("res_{0}_{1}_{2}", beginStr, endStr, configData.ID);
				long reservetimes = 0L;
				string lastgettime = "";
				int ret = DBQuery.GetAwardHistoryForUser(dbMgr, dbUserInfo.UserID, 71, keyStr, out reservetimes, out lastgettime);
				if (reservetimes <= 0L)
				{
					return 0;
				}
				reservetimes -= 1L;
				lastgettime = chargeTm;
				ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbUserInfo.UserID, 71, keyStr, reservetimes, lastgettime);
				if (ret < 0)
				{
					ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbUserInfo.UserID, 71, keyStr, reservetimes, lastgettime);
				}
				return configData.ID;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("_ProcessSuperInputFanLi:" + ex.ToString());
			}
			return 0;
		}

		
		private static void _ProcessBuyYueKa(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			int rid = DBQuery.LastLoginRole(dbMgr, dbUserInfo.UserID);
			string gmCmdData = string.Format("-buyyueka {0} {1}", dbUserInfo.UserID, rid);
			LogManager.WriteLog(LogTypes.Error, string.Format("处理玩家购买月卡，userid={0}, roleid={1}", dbUserInfo.UserID, rid), null, true);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
		}

		
		private static void _ProcessCharge(DBManager dbMgr, DBUserInfo dbUserInfo, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm, SingleChargeData chargeData, bool bZhiGouFail = false)
		{
			int currentGiftID = GameDBManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0);
			int moneyToYuanBao = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
			int moneyToJiFen = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-jifen", 1);
			int ChargeID = 0;
			chargeData.MoneyVsChargeIDDict.TryGetValue(addUserMoney, out ChargeID);
			bool bWillGiveZuanShi = zhigouID == 0 && (chargeData.ChargePlatType == 1 || addUserMoney != chargeData.YueKaMoney);
			bool bSystemProcessCharge = chargeRoleID == -1;
			bool bProcessBuyItem = zhigouID != 0 && chargeRoleID > 0;
			lock (dbUserInfo)
			{
				if (bWillGiveZuanShi)
				{
					dbUserInfo.Money += addUserMoney * moneyToYuanBao;
					if (dbUserInfo.Money < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("充值后玩家元宝变负数修正为0, UserID={0}, Money={1}, AddMoney={2}", dbUserInfo.UserID, dbUserInfo.Money, addUserMoney), null, true);
						dbUserInfo.Money = 0;
					}
				}
				if (!bSystemProcessCharge && !bZhiGouFail)
				{
					dbUserInfo.RealMoney += addUserMoney;
					if (dbUserInfo.RealMoney < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("充值后玩家realmoney变负数修正为0, UserID={0}, Money={1}, AddMoney={2}", dbUserInfo.UserID, dbUserInfo.RealMoney, addUserMoney), null, true);
						dbUserInfo.RealMoney = 0;
					}
					if (currentGiftID != dbUserInfo.GiftID)
					{
						dbUserInfo.GiftJiFen = 0;
						dbUserInfo.GiftID = currentGiftID;
					}
					if (dbUserInfo.GiftID > 0)
					{
						dbUserInfo.GiftJiFen += addUserMoney * moneyToJiFen;
					}
				}
				int userMoney = dbUserInfo.Money;
				int realMoney = dbUserInfo.RealMoney;
				int giftID = dbUserInfo.GiftID;
				int giftJiFen = dbUserInfo.GiftJiFen;
				DBWriter.UpdateUserInfo(dbMgr, dbUserInfo);
			}
			DBRoleInfo dbRoleInfo = Global.FindOnlineRoleInfoByUserInfo(dbMgr, dbUserInfo);
			if (dbRoleInfo != null && !bZhiGouFail)
			{
				DBWriter.UpdateCityInfoItem(dbMgr, dbRoleInfo.LastIP, dbRoleInfo.UserID, "inputmoney", addUserMoney * moneyToYuanBao);
			}
			int rid = chargeRoleID;
			if (!bSystemProcessCharge && !bProcessBuyItem)
			{
				rid = DBQuery.LastLoginRole(dbMgr, dbUserInfo.UserID);
			}
			int addUserYuanBao = Global.TransMoneyToYuanBao(addUserMoney);
			if (bProcessBuyItem)
			{
				UserMoneyMgr._ProcessBuyItem(dbMgr, dbUserInfo, chargeRoleID, addUserMoney, zhigouID, chargeTm);
			}
			int superInputFanLi = 0;
			if (!bProcessBuyItem && addUserMoney != chargeData.YueKaMoney && !bZhiGouFail)
			{
				superInputFanLi = UserMoneyMgr._ProcessSuperInputFanLi(dbMgr, dbUserInfo, chargeData, addUserMoney, ChargeID, chargeTm);
			}
			if (!bSystemProcessCharge && bWillGiveZuanShi)
			{
				CFirstChargeMgr.SendToRolebindgold(dbMgr, dbUserInfo.UserID, rid, addUserMoney, chargeData);
			}
			if (!bSystemProcessCharge && !bZhiGouFail)
			{
				SingletonTemplate<FundManager>.Instance().FundAddMoney(dbUserInfo.UserID, addUserYuanBao, rid, 0);
				GameDBManager.RankCacheMgr.OnUserDoSomething(rid, RankType.Charge, addUserYuanBao);
			}
			string gmCmdData = string.Format("-updateyb {0} {1} {2} {3} {4}", new object[]
			{
				dbUserInfo.UserID,
				rid,
				addUserMoney,
				superInputFanLi,
				zhigouID
			});
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
			LogManager.WriteLog(LogTypes.Error, string.Format("处理充值成功 UID={0}，money={1}，itemid={2}", dbUserInfo.UserID, addUserMoney, zhigouID), null, true);
		}

		
		public static void ScanInputLogToDBLog(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - UserMoneyMgr.LastScanInputLogTicks >= 30000L)
			{
				UserMoneyMgr.LastScanInputLogTicks = nowTicks;
				if (UserMoneyMgr.LastScanID < 0)
				{
					UserMoneyMgr.LastScanID = DBQuery.QueryLastScanInputLogID(dbMgr);
				}
				int newLastScanID = DBQuery.ScanInputLogFromTable(dbMgr, UserMoneyMgr.LastScanID);
				if (newLastScanID != UserMoneyMgr.LastScanID)
				{
					UserMoneyMgr.LastScanID = newLastScanID;
					DBWriter.UpdateLastScanInputLogID(dbMgr, UserMoneyMgr.LastScanID);
				}
			}
		}

		
		public static void GMAddCharge(string userid, string money, string rid, string itemid, string time)
		{
			if (string.IsNullOrEmpty(time))
			{
				time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			}
			Random random = new Random();
			string rs = random.Next().ToString("X8");
			string cc = Global.GCC(4, new object[]
			{
				rs,
				userid,
				money,
				time
			});
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("insert into t_tempmoney values(null,'{0}','{1}','{2}',{3},{4}, '{5}');", new object[]
				{
					cc.Substring(0, 24) + rs,
					userid,
					rid,
					money,
					itemid,
					time
				});
				conn.ExecuteNonQuery(sql, 0);
				sql = string.Format("INSERT INTO `t_inputlog` VALUES (null, '{2}', '{0}', '{1}', '{5}', '{0}{5}', '1470151268', '{6}', '{4}', 'success', '54', {3}, '{4}');", new object[]
				{
					userid,
					rid,
					money,
					itemid,
					time,
					rs,
					cc
				});
				conn.ExecuteNonQuery(sql, 0);
			}
		}

		
		public static void QueryTotalUserMoney()
		{
			if (GameDBManager.Flag_Query_Total_UserMoney_Minute >= 5)
			{
				DateTime now = DateTime.Now;
				bool bInCheckTime = false;
				long elapsedMilliseconds = (now.Ticks - UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime.Ticks) / 10000L;
				if (elapsedMilliseconds >= (long)(GameDBManager.Flag_Query_Total_UserMoney_Minute * 60 * 1000))
				{
					bInCheckTime = true;
				}
				else if (now.Day != UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime.Day)
				{
					bInCheckTime = true;
				}
				if (bInCheckTime)
				{
					UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime = now;
					LogManager.WriteLog(LogTypes.TotalUserMoney, string.Format("{0}\t{1}\t{2}", 10000, GameDBManager.ZoneID, DBQuery.QueryServerTotalUserMoney()), null, true);
				}
			}
		}

		
		public static TCPProcessCmdResults ProcessGetChargeItemData(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string uid = fields[0];
				int roleID = Convert.ToInt32(fields[1]);
				byte HandleDel = Convert.ToByte(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<TempItemChargeInfo> tempItemInfoList = DBQuery.QueryTempItemChargeInfo(dbMgr, roleID, 0, HandleDel);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<TempItemChargeInfo>>(tempItemInfoList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException("ProcessGetChargeItemData:" + e.ToString());
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessDelChargeItemData(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int SerialID = Convert.ToInt32(fields[0]);
				int ChargeMoney = Convert.ToInt32(fields[1]);
				int ReturnUserMoney = Convert.ToInt32(fields[2]);
				SingleChargeData chargeData = CFirstChargeMgr.ChargeData;
				if (chargeData == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<TempItemChargeInfo> tempItemInfoList = DBQuery.QueryTempItemChargeInfo(dbMgr, 0, SerialID, 0);
				if (tempItemInfoList.Count == 0)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = tempItemInfoList[0].userID;
				int chargeRoleID = tempItemInfoList[0].chargeRoleID;
				int addUserMoney = tempItemInfoList[0].addUserMoney;
				int zhigouID = tempItemInfoList[0].zhigouID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (dbUserInfo == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				byte DelState = (byte)((ChargeMoney == 1) ? 2 : 1);
				if (!DBWriter.DeleteChargeItemInfo(dbMgr, SerialID, DelState))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (ChargeMoney == 1)
				{
					UserMoneyMgr._ProcessCharge(dbMgr, dbUserInfo, chargeRoleID, addUserMoney, 0, "", chargeData, true);
				}
				else if (ReturnUserMoney > 0)
				{
					UserMoneyMgr._ProcessCharge(dbMgr, dbUserInfo, chargeRoleID, ReturnUserMoney, 0, "", chargeData, true);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException("ProcessDelChargeItemData:" + e.ToString());
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static long LastUpdateUserMoneyTicks = 0L;

		
		private static bool ChargeDataLogState = false;

		
		private static long LastScanInputLogTicks = DateTime.Now.Ticks / 10000L;

		
		private static int LastScanID = -1;

		
		private static DateTime LastLastQueryServerTotalUserMoneyTime = DateTime.MinValue;
	}
}
