using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	// Token: 0x02000104 RID: 260
	public class JieriGiveKingActHandler : SingletonTemplate<JieriGiveKingActHandler>
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x0002214A File Offset: 0x0002034A
		private JieriGiveKingActHandler()
		{
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00022158 File Offset: 0x00020358
		public TCPProcessCmdResults ProcLoadJieriGiveKingRank(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string fromDate = fields[0].Replace('$', ':');
				string toDate = fields[1].Replace('$', ':');
				int RankCnt = Convert.ToInt32(fields[2]);
				List<JieriGiveKingItemData> rankItems = JieriGiveKingActHandler.QueryJieriGiveKingRank(dbMgr, fromDate, toDate, RankCnt);
				string actKey = Global.GetHuoDongKeyString(fromDate, toDate);
				foreach (JieriGiveKingItemData item in rankItems)
				{
					int hasgettimes = 0;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, item.RoleID, item.ZoneID, 51, actKey, out hasgettimes, out lastgettime);
					item.GetAwardTimes = hasgettimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JieriGiveKingItemData>>(rankItems, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00022348 File Offset: 0x00020548
		public TCPProcessCmdResults ProcLoadRoleJieriGiveKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				JieriGiveKingItemData rankItem = this.QueryRoleJieriGiveKing(dbMgr, fromDate, toDate, roleID);
				if (rankItem == null)
				{
					rankItem = new JieriGiveKingItemData();
					rankItem.RoleID = roleID;
					rankItem.Rolename = roleInfo.RoleName;
					rankItem.TotalGive = 0;
					rankItem.ZoneID = roleInfo.ZoneID;
					rankItem.Rank = -1;
					rankItem.GetAwardTimes = 0;
				}
				else
				{
					int hasgettimes = 0;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, rankItem.RoleID, rankItem.ZoneID, 51, Global.GetHuoDongKeyString(fromDate, toDate), out hasgettimes, out lastgettime);
					rankItem.GetAwardTimes = hasgettimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriGiveKingItemData>(rankItem, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00022574 File Offset: 0x00020774
		public TCPProcessCmdResults ProcGetJieriGiveKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				lock (roleInfo)
				{
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 51, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1008), nID);
						return TCPProcessCmdResults.RESULT_DATA;
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

		// Token: 0x0600045D RID: 1117 RVA: 0x000227C8 File Offset: 0x000209C8
		public static List<JieriGiveKingItemData> QueryJieriGiveKingRank(DBManager dbMgr, string fromDate, string toDate, int rankCnt)
		{
			List<JieriGiveKingItemData> result = new List<JieriGiveKingItemData>();
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT sender, rname, zoneid, SUM(goodscnt) AS totalsend FROM t_jierizengsong, t_roles WHERE sender=rid AND isdel=0 AND sendtime>= '{0}' AND sendtime<='{1}' GROUP BY sender ORDER BY totalsend DESC, sender ASC LIMIT {2}", fromDate, toDate, rankCnt);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int count = 0;
				while (reader.Read() && count < rankCnt)
				{
					JieriGiveKingItemData item = new JieriGiveKingItemData
					{
						RoleID = Convert.ToInt32(reader["sender"].ToString()),
						Rolename = reader["rname"].ToString(),
						ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
						TotalGive = Convert.ToInt32(reader["totalsend"].ToString()),
						Rank = count + 1
					};
					result.Add(item);
					count++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				cmd.Dispose();
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00022928 File Offset: 0x00020B28
		private JieriGiveKingItemData QueryRoleJieriGiveKing(DBManager dbMgr, string fromDate, string toDate, int roleID)
		{
			JieriGiveKingItemData result = null;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = "SELECT t_roles.rid, t_roles.rname, t_roles.zoneid, x.totalsend from t_roles,  (SELECT t_jierizengsong.sender, SUM(t_jierizengsong.goodscnt) AS totalsend " + string.Format(" FROM t_jierizengsong WHERE t_jierizengsong.sender={0} AND sendtime>= '{1}' AND sendtime<='{2}') x ", roleID, fromDate, toDate) + " where t_roles.isdel=0 and t_roles.rid = x.sender ";
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				if (reader.Read())
				{
					result = new JieriGiveKingItemData();
					result.RoleID = Convert.ToInt32(reader["rid"].ToString());
					result.Rolename = reader["rname"].ToString();
					result.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
					result.TotalGive = Convert.ToInt32(reader["totalsend"].ToString());
					result.Rank = -1;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				cmd.Dispose();
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}
	}
}
