using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	// Token: 0x02000105 RID: 261
	internal class JieriLianXuChargeActHandler : SingletonTemplate<JieriLianXuChargeActHandler>
	{
		// Token: 0x0600045F RID: 1119 RVA: 0x00022A5C File Offset: 0x00020C5C
		private JieriLianXuChargeActHandler()
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00022A68 File Offset: 0x00020C68
		public TCPProcessCmdResults ProcQueryActInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int zoneID = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace('$', ':');
				string toDate = fields[3].Replace('$', ':');
				List<int> awardIdList = new List<int>();
				string[] szIds = fields[4].Split(new char[]
				{
					'_'
				});
				foreach (string szId in szIds)
				{
					if (!string.IsNullOrEmpty(szId))
					{
						awardIdList.Add(Convert.ToInt32(szId));
					}
				}
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				StringBuilder sb = new StringBuilder(256);
				lock (userInfo)
				{
					Dictionary<string, int> eachDayChargeYB = this._QueryEachDayChargeYB(dbMgr, fromDate, toDate, userInfo.UserID, zoneID);
					if (eachDayChargeYB != null && eachDayChargeYB.Count > 0)
					{
						foreach (KeyValuePair<string, int> kvp in eachDayChargeYB)
						{
							sb.Append(kvp.Key).Append(',').Append(kvp.Value).Append('$');
						}
					}
					sb.Append(':');
					Dictionary<int, int> eachAwardIdFlag = this._QueryEachAwardIdFlag(dbMgr, fromDate, toDate, userInfo.UserID, roleInfo.ZoneID, fields[4].Split(new char[]
					{
						'_'
					}));
					if (eachAwardIdFlag != null && eachAwardIdFlag.Count > 0)
					{
						foreach (KeyValuePair<int, int> kvp2 in eachAwardIdFlag)
						{
							sb.Append(kvp2.Key).Append(',').Append(kvp2.Value).Append('$');
						}
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, sb.ToString(), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00022E40 File Offset: 0x00021040
		public TCPProcessCmdResults ProcUpdateAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int awardId = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace('$', ':');
				string toDate = fields[3].Replace('$', ':');
				int awardFlag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				int result = 1;
				lock (userInfo)
				{
					result = this._UpdateAwardFlag(dbMgr, roleInfo.UserID, fromDate, toDate, roleInfo.ZoneID, awardId, awardFlag);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", result), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00023060 File Offset: 0x00021260
		private string _GetActAwardKey_NoZoneID(string fromDate, string toDate, int zoneId, int awardId)
		{
			return Global.GetHuoDongKeyString(fromDate, toDate) + "_" + awardId.ToString();
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0002308C File Offset: 0x0002128C
		private string _GetActAwardKey_Ex(string fromDate, string toDate, int zoneId, int awardId)
		{
			return string.Concat(new string[]
			{
				Global.GetHuoDongKeyString(fromDate, toDate),
				"_",
				zoneId.ToString(),
				"_",
				awardId.ToString()
			});
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x000230DC File Offset: 0x000212DC
		private bool _GetAwardIdByExtKey(string extKey, out int awardId)
		{
			awardId = 0;
			bool result;
			if (string.IsNullOrEmpty(extKey))
			{
				result = false;
			}
			else
			{
				string[] szFields = extKey.Split(new char[]
				{
					'_'
				});
				result = (szFields != null && szFields.Length >= 3 && int.TryParse(szFields[szFields.Length - 1], out awardId));
			}
			return result;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0002313C File Offset: 0x0002133C
		private Dictionary<int, int> _QueryEachAwardIdFlag(DBManager dbMgr, string fromDate, string toDate, string userid, int zoneId, string[] AwardIdArray)
		{
			Dictionary<int, int> result;
			if (dbMgr == null || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userid) || AwardIdArray == null || AwardIdArray.Length == 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, int> eachAwardIdFlag = new Dictionary<int, int>();
				MySQLConnection conn = null;
				try
				{
					string cmdText = string.Format("SELECT keystr,hasgettimes FROM t_huodongawarduserhist WHERE userid='{0}' AND activitytype={1} AND keystr LIKE '{2}%'", userid, 61, Global.GetHuoDongKeyString(fromDate, toDate));
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					conn = dbMgr.DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					try
					{
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						while (reader.Read())
						{
							string extKey = reader["keystr"].ToString();
							int awardFlag = Convert.ToInt32(reader["hasgettimes"].ToString());
							int awardId = 0;
							if (this._GetAwardIdByExtKey(extKey, out awardId))
							{
								int flag;
								if (!eachAwardIdFlag.TryGetValue(awardId, out flag))
								{
									eachAwardIdFlag[awardId] = awardFlag;
								}
								else
								{
									eachAwardIdFlag[awardId] = (flag | awardFlag);
								}
							}
						}
					}
					catch (MySQLException)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("读取数据库失败: {0}", cmdText), null, true);
					}
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if (null != conn)
					{
						dbMgr.DBConns.PushDBConnection(conn);
					}
				}
				result = eachAwardIdFlag;
			}
			return result;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x000232D0 File Offset: 0x000214D0
		private Dictionary<string, int> _QueryEachDayChargeYB(DBManager dbMgr, string fromDate, string toDate, string userid, int zoneid)
		{
			Dictionary<string, int> result;
			if (dbMgr == null || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userid))
			{
				result = null;
			}
			else
			{
				Dictionary<string, int> eachDayChargeYB = new Dictionary<string, int>();
				MySQLConnection conn = null;
				try
				{
					string cmdText = string.Format("SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND result='success' GROUP BY inputdate UNION ALL  SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog2 WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND result='success' GROUP BY inputdate", userid, fromDate, toDate);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					conn = dbMgr.DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					try
					{
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						while (reader.Read())
						{
							string inputDate = reader["inputdate"].ToString();
							int inputMoney = Convert.ToInt32(reader["total"].ToString());
							int inputYuanBao = Global.TransMoneyToYuanBao(inputMoney);
							if (!eachDayChargeYB.ContainsKey(inputDate))
							{
								eachDayChargeYB.Add(inputDate, inputYuanBao);
							}
							else
							{
								Dictionary<string, int> dictionary;
								string key;
								(dictionary = eachDayChargeYB)[key = inputDate] = dictionary[key] + inputYuanBao;
							}
						}
					}
					catch (MySQLException)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("读取数据库失败: {0}", cmdText), null, true);
					}
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if (null != conn)
					{
						dbMgr.DBConns.PushDBConnection(conn);
					}
				}
				result = eachDayChargeYB;
			}
			return result;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00023444 File Offset: 0x00021644
		private int _UpdateAwardFlag(DBManager dbMgr, string userid, string fromDate, string toDate, int zoneId, int awardId, int awardFlag)
		{
			string lastgettime_just_placeholder = string.Empty;
			int hasgettimes_just_placeholder = 0;
			int result = 1;
			string actKey = this._GetActAwardKey_NoZoneID(fromDate, toDate, zoneId, awardId);
			if (DBQuery.GetAwardHistoryForUser(dbMgr, userid, 61, actKey, out hasgettimes_just_placeholder, out lastgettime_just_placeholder) < 0)
			{
				if (DBWriter.AddHongDongAwardRecordForUser(dbMgr, userid, 61, actKey, (long)awardFlag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) < 0)
				{
					result = -1008;
				}
			}
			else if (DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, userid, 61, actKey, (long)awardFlag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) < 0)
			{
			}
			return result;
		}
	}
}
