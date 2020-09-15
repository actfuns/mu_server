using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	// Token: 0x02000103 RID: 259
	public class JieriGiveActHandler : SingletonTemplate<JieriGiveActHandler>
	{
		// Token: 0x06000452 RID: 1106 RVA: 0x00021837 File Offset: 0x0001FA37
		private JieriGiveActHandler()
		{
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00021844 File Offset: 0x0001FA44
		public static bool GetTotalGiveAndRecv(DBManager dbMgr, int roleid, string fromDate, string toDate, out int totalGive, out int totalRecv)
		{
			totalGive = 0;
			totalRecv = 0;
			bool bSuccess = false;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string queryGive = string.Format("SELECT IFNULL(SUM(goodscnt), 0) AS totalcnt FROM t_jierizengsong WHERE sender={0} AND sendtime>='{1}' AND sendtime <= '{2}'", roleid, fromDate, toDate);
				string queryRecv = string.Format("SELECT IFNULL(SUM(goodscnt), 0) AS totalcnt FROM t_jierizengsong WHERE receiver={0} AND sendtime>='{1}' AND sendtime <= '{2}'", roleid, fromDate, toDate);
				string cmdText = queryGive + " UNION ALL " + queryRecv;
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int idx = 0;
				while (reader.Read() && idx < 2)
				{
					if (idx == 0)
					{
						totalGive = Convert.ToInt32(reader["totalcnt"].ToString());
					}
					else if (idx == 1)
					{
						totalRecv = Convert.ToInt32(reader["totalcnt"].ToString());
					}
					idx++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				cmd.Dispose();
				bSuccess = true;
			}
			finally
			{
				if (conn != null)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return bSuccess;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00021984 File Offset: 0x0001FB84
		public bool AddJieriGiveRecord(DBManager dbMgr, int sender, int receiver, int goods, int cnt)
		{
			bool bSuccess = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_jierizengsong (sender, receiver, goodsid, goodscnt, sendtime) VALUES({0}, {1}, {2}, {3}, '{4}')", new object[]
				{
					sender,
					receiver,
					goods,
					cnt,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				if (conn.ExecuteNonQuery(cmdText, 0) > 0)
				{
					bSuccess = true;
				}
			}
			return bSuccess;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00021A38 File Offset: 0x0001FC38
		public TCPProcessCmdResults ProcQueryJieriGiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int actType = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace('$', ':');
				string toDate = fields[3].Replace('$', ':');
				int todayIdxInActPeriod = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int awardBitFlag = 0;
				int totalGive = 0;
				int totalRecv = 0;
				string lastgettime_just_placeholder = string.Empty;
				JieriGiveActHandler.GetTotalGiveAndRecv(dbMgr, roleID, fromDate, toDate, out totalGive, out totalRecv);
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, actType, this._GetAwardKey_Ext_DayIdxInPeriod(fromDate, toDate, todayIdxInActPeriod), out awardBitFlag, out lastgettime_just_placeholder);
				string strcmd = string.Format("{0}:{1}:{2}", totalGive, totalRecv, awardBitFlag);
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

		// Token: 0x06000456 RID: 1110 RVA: 0x00021C38 File Offset: 0x0001FE38
		public TCPProcessCmdResults ProcRoleJieriGiveToOther(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string receiverRolename = fields[1];
				int goodsID = Convert.ToInt32(fields[2]);
				int goodsCnt = Convert.ToInt32(fields[3]);
				DBRoleInfo receiverRole = dbMgr.GetDBRoleInfo(receiverRolename);
				if (receiverRole == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!this.AddJieriGiveRecord(dbMgr, roleID, receiverRole.RoleID, goodsID, goodsCnt))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-2", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", receiverRole.RoleID), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00021E04 File Offset: 0x00020004
		public TCPProcessCmdResults ProcessGetJieriGiveAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int actType = Convert.ToInt32(fields[3]);
				int todayIdx = Convert.ToInt32(fields[4]);
				int awardBitFlags = Convert.ToInt32(fields[5]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string lastgettime_just_placeholder = string.Empty;
				int hasgettimes_just_placeholder = 0;
				lock (roleInfo)
				{
					string actKey = this._GetAwardKey_Ext_DayIdxInPeriod(fromDate, toDate, todayIdx);
					if (DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, actType, actKey, out hasgettimes_just_placeholder, out lastgettime_just_placeholder) < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, actType, actKey, awardBitFlags, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							string strcmd = string.Format("{0}", -1008);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, actType, actKey, awardBitFlags, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							string strcmd = string.Format("{0}", -1008);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00022120 File Offset: 0x00020320
		private string _GetAwardKey_Ext_DayIdxInPeriod(string fromDate, string toDate, int todayIdxInActPeriod)
		{
			return Global.GetHuoDongKeyString(fromDate, toDate) + "_" + todayIdxInActPeriod.ToString();
		}
	}
}
