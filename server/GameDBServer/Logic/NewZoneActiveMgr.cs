using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000157 RID: 343
	internal class NewZoneActiveMgr
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x00033058 File Offset: 0x00031258
		private static List<PaiHangItemData> GetActiveKingTypeRanklist(DBManager dbMgr, List<int> minGateValueList, int activityType, string midDate, int maxPaiHang = 10)
		{
			List<HuoDongPaiHangData> listPaiHangReal = DBQuery.GetActivityPaiHangListNearMidTime(dbMgr, activityType, midDate, maxPaiHang);
			List<PaiHangItemData> listPaiHang = new List<PaiHangItemData>();
			int preUserPaiHang = 0;
			int preValueid = 0;
			bool bFirst = true;
			for (int i = 0; i < listPaiHangReal.Count; i++)
			{
				HuoDongPaiHangData phData = listPaiHangReal[i];
				phData.PaiHang = -1;
				for (int j = 0; j < minGateValueList.Count; j++)
				{
					if (phData.PaiHangValue >= minGateValueList[j])
					{
						PaiHangItemData item = new PaiHangItemData();
						if (bFirst)
						{
							phData.PaiHang = j + 1;
						}
						else if (j == preValueid)
						{
							phData.PaiHang = preUserPaiHang + 1;
						}
						else if (j + 1 > preUserPaiHang)
						{
							phData.PaiHang = j + 1;
						}
						else
						{
							phData.PaiHang = preUserPaiHang + 1;
						}
						item.RoleID = phData.RoleID;
						item.RoleName = phData.RoleName;
						item.Val2 = phData.PaiHang;
						item.Val1 = phData.PaiHangValue;
						listPaiHang.Add(item);
						preValueid = j;
						preUserPaiHang = phData.PaiHang;
						bFirst = false;
						break;
					}
				}
				if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			return listPaiHang;
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x000331D8 File Offset: 0x000313D8
		private static List<PaiHangItemData> GetRankListByActiveLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int activeId, int maxPaiHang = 3)
		{
			List<InputKingPaiHangData> listPaiHangReal = new List<InputKingPaiHangData>();
			List<PaiHangItemData> ranklist = new List<PaiHangItemData>();
			switch (activeId)
			{
			case 34:
				listPaiHangReal = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, null, maxPaiHang);
				break;
			case 35:
				listPaiHangReal = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, null, maxPaiHang);
				break;
			case 36:
			{
				string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, toDate))
				{
					paiHangDate = toDate;
				}
				return NewZoneActiveMgr.GetActiveKingTypeRanklist(dbMgr, minGateValueList, activeId, paiHangDate, maxPaiHang);
			}
			case 37:
			{
				DateTime now = DateTime.Now;
				DateTime huodongStartTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(fromDate, out huodongStartTime);
				DateTime sub1DayDateTime = Global.GetAddDaysDataTime(now, -1, true);
				string startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				listPaiHangReal = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, startTime, endTime, null, maxPaiHang);
				break;
			}
			}
			int preUserPaiHang = 0;
			int preValueid = 0;
			string uid = "";
			bool bFirst = true;
			for (int i = 0; i < listPaiHangReal.Count; i++)
			{
				InputKingPaiHangData phData = listPaiHangReal[i];
				phData.PaiHang = -1;
				if (activeId != 35)
				{
					Global.GetUserMaxLevelRole(dbMgr, phData.UserID, out phData.MaxLevelRoleName, out phData.MaxLevelRoleZoneID);
				}
				else
				{
					Global.GetRoleNameAndUserID(dbMgr, Global.SafeConvertToInt32(phData.UserID, 10), out phData.MaxLevelRoleName, out uid);
				}
				for (int j = 0; j < minGateValueList.Count; j++)
				{
					int values = phData.PaiHangValue;
					if (activeId == 35)
					{
						values = phData.PaiHangValue;
					}
					if (values >= minGateValueList[j])
					{
						if (bFirst)
						{
							phData.PaiHang = j + 1;
						}
						else if (j == preValueid)
						{
							phData.PaiHang = preUserPaiHang + 1;
						}
						else if (j + 1 > preUserPaiHang)
						{
							phData.PaiHang = j + 1;
						}
						else
						{
							phData.PaiHang = preUserPaiHang + 1;
						}
						PaiHangItemData item = new PaiHangItemData();
						item.Val1 = values;
						if (activeId == 35)
						{
							item.RoleID = Convert.ToInt32(phData.UserID);
						}
						item.RoleName = phData.MaxLevelRoleName;
						item.Val2 = phData.PaiHang;
						item.uid = phData.UserID;
						ranklist.Add(item);
						preValueid = j;
						preUserPaiHang = phData.PaiHang;
						bFirst = false;
						break;
					}
				}
				if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			if (activeId == 37)
			{
				for (int j = 0; j < ranklist.Count; j++)
				{
					int rank = j + minGateValueList.Count - ranklist.Count;
					ranklist[j].Val1 = ranklist[j].Val1 * minGateValueList[rank] / 100;
				}
			}
			return ranklist;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0003356C File Offset: 0x0003176C
		public static TCPProcessCmdResults ProcessQueryActiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int activeid = Global.SafeConvertToInt32(fields[4], 10);
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				List<PaiHangItemData> listPaiHang = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, toDate, minGateValueList, activeid, minGateValueList.Count);
				int inputMoneyInPeriod = 0;
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				switch (activeid)
				{
				case 34:
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 34, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						hasgettimes = 1;
					}
					break;
				case 35:
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 35, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						hasgettimes = 1;
					}
					break;
				case 36:
					DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, activeid, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						hasgettimes = 1;
					}
					break;
				case 37:
					inputMoneyInPeriod = NewZoneActiveMgr.ComputTotalFanliValue(dbMgr, roleInfo, activeid, fromDate, toDate, minGateValueList);
					if (inputMoneyInPeriod == 0)
					{
						hasgettimes = 1;
					}
					break;
				}
				NewZoneActiveData consumedata = new NewZoneActiveData
				{
					YuanBao = inputMoneyInPeriod,
					ActiveId = activeid,
					Ranklist = listPaiHang,
					GetState = hasgettimes
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<NewZoneActiveData>(consumedata, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000338A8 File Offset: 0x00031AA8
		private static TCPProcessCmdResults GetBossKillAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, toDate))
				{
					paiHangDate = toDate;
				}
				List<HuoDongPaiHangData> listPaiHang = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, 36, paiHangDate, 10);
				int paiHang = -1;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (listPaiHang[i] != null && roleInfo.RoleID == listPaiHang[i].RoleID)
					{
						paiHang = listPaiHang[i].PaiHang;
						break;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, activeid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 36, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, activeid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 36, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, activeid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, paiHang, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00033B68 File Offset: 0x00031D68
		private static TCPProcessCmdResults GetConsumeKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PaiHangItemData> listPaiHang = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, toDate, minGateValueList, activeid, minGateValueList.Count);
				int paiHang = -1;
				int inputMoneyInPeriod = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.RoleID == listPaiHang[i].RoleID)
					{
						paiHang = listPaiHang[i].Val2;
						inputMoneyInPeriod = listPaiHang[i].Val1;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 35, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 35, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, paiHang, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00033E4C File Offset: 0x0003204C
		private static TCPProcessCmdResults GetRechargeKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PaiHangItemData> listPaiHang = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, toDate, minGateValueList, activeid, minGateValueList.Count);
				int paiHang = -1;
				int inputMoneyInPeriod = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.UserID == listPaiHang[i].uid)
					{
						paiHang = listPaiHang[i].Val2;
						inputMoneyInPeriod = listPaiHang[i].Val1;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 34, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 34, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, paiHang, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x0003416C File Offset: 0x0003236C
		private static TCPProcessCmdResults GetNewFanliAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string todate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1001, roleID, activeid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleYuanBaoInPeriod = NewZoneActiveMgr.ComputTotalFanliValue(dbMgr, roleInfo, activeid, fromDate, todate, minGateValueList);
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					if (roleYuanBaoInPeriod > 0)
					{
						DateTime sub1DayDateTime = Global.GetAddDaysDataTime(DateTime.Now, -1, true);
						DateTime startTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 0, 0, 0);
						DateTime endTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 23, 59, 59);
						string huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 37, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:{2}", -1008, roleID, activeid);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleYuanBaoInPeriod, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00034398 File Offset: 0x00032598
		private static int ComputNewFanLiValue(DBManager dbMgr, DBRoleInfo roleInfo, int activeid, string fromdate, string todate, List<int> minGateValueList)
		{
			int retvalue = 0;
			List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromdate, todate, minGateValueList, minGateValueList.Count);
			RankDataKey key = new RankDataKey(RankType.Charge, fromdate, todate, null);
			int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
			if (inputMoneyInPeriod < 0)
			{
				inputMoneyInPeriod = 0;
			}
			for (int i = 0; i < listPaiHang.Count; i++)
			{
				if (listPaiHang[i].UserID == roleInfo.UserID)
				{
					inputMoneyInPeriod = inputMoneyInPeriod * minGateValueList[listPaiHang[i].PaiHang - 1] / 100;
					retvalue = inputMoneyInPeriod;
					break;
				}
			}
			return retvalue;
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0003444C File Offset: 0x0003264C
		private static int ComputTotalFanliValue(DBManager dbMgr, DBRoleInfo roleInfo, int activeid, string fromDate, string todate, List<int> minGateValueList)
		{
			DateTime now = DateTime.Now;
			DateTime huodongStartTime = new DateTime(2000, 1, 1, 0, 0, 0);
			DateTime huodongEndTime = default(DateTime);
			DateTime.TryParse(fromDate, out huodongStartTime);
			DateTime.TryParse(todate, out huodongEndTime);
			int retvalue = 0;
			int result;
			if (now.Ticks <= huodongStartTime.Ticks + 864000000000L)
			{
				result = 0;
			}
			else
			{
				for (int i = 1; i <= 7; i++)
				{
					DateTime sub1DayDateTime = Global.GetAddDaysDataTime(now, -i, true);
					DateTime startTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 0, 0, 0);
					DateTime endTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 23, 59, 59);
					string huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
					if (startTime < huodongStartTime)
					{
						break;
					}
					int hasgettimes = 0;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						break;
					}
					retvalue += NewZoneActiveMgr.ComputNewFanLiValue(dbMgr, roleInfo, activeid, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), minGateValueList);
				}
				result = retvalue;
			}
			return result;
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000345B8 File Offset: 0x000327B8
		public static TCPProcessCmdResults ProcessGetNewzoneActiveAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
			TCPProcessCmdResults ret = TCPProcessCmdResults.RESULT_FAILED;
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int activetype = Global.SafeConvertToInt32(fields[4], 10);
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				switch (activetype)
				{
				case 34:
					ret = NewZoneActiveMgr.GetRechargeKingAward(dbMgr, pool, nID, roleID, activetype, fromDate, toDate, minGateValueList, out tcpOutPacket);
					break;
				case 35:
					ret = NewZoneActiveMgr.GetConsumeKingAward(dbMgr, pool, nID, roleID, activetype, fromDate, toDate, minGateValueList, out tcpOutPacket);
					break;
				case 36:
					ret = NewZoneActiveMgr.GetBossKillAward(dbMgr, pool, nID, roleID, activetype, fromDate, toDate, minGateValueList, out tcpOutPacket);
					break;
				case 37:
					ret = NewZoneActiveMgr.GetNewFanliAward(dbMgr, pool, nID, roleID, activetype, fromDate, toDate, minGateValueList, out tcpOutPacket);
					break;
				}
			}
			catch (Exception ex)
			{
			}
			return ret;
		}

		// Token: 0x04000854 RID: 2132
		public static NewZoneActiveData NewZoneFanli = null;
	}
}
