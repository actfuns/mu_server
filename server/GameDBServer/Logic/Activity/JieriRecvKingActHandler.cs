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
	
	public class JieriRecvKingActHandler : SingletonTemplate<JieriRecvKingActHandler>
	{
		
		private JieriRecvKingActHandler()
		{
		}

		
		public TCPProcessCmdResults ProcLoadJieriRecvKingRank(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				List<JieriRecvKingItemData> rankItems = JieriRecvKingActHandler.QueryJieriRecvKingRank(dbMgr, fromDate, toDate, RankCnt);
				string actKey = Global.GetHuoDongKeyString(fromDate, toDate);
				foreach (JieriRecvKingItemData item in rankItems)
				{
					int hasgettimes = 0;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, item.RoleID, item.ZoneID, 52, actKey, out hasgettimes, out lastgettime);
					item.GetAwardTimes = hasgettimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JieriRecvKingItemData>>(rankItems, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public TCPProcessCmdResults ProcLoadRoleJieriRecvKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				JieriRecvKingItemData rankItem = this.QueryRoleJieriRecvKing(dbMgr, fromDate, toDate, roleID);
				if (rankItem == null)
				{
					rankItem = new JieriRecvKingItemData();
					rankItem.RoleID = roleID;
					rankItem.Rolename = roleInfo.RoleName;
					rankItem.TotalRecv = 0;
					rankItem.ZoneID = roleInfo.ZoneID;
					rankItem.Rank = -1;
					rankItem.GetAwardTimes = 0;
				}
				else
				{
					int hasgettimes = 0;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, rankItem.RoleID, rankItem.ZoneID, 52, Global.GetHuoDongKeyString(fromDate, toDate), out hasgettimes, out lastgettime);
					rankItem.GetAwardTimes = hasgettimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriRecvKingItemData>(rankItem, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public TCPProcessCmdResults ProcGetJieriRecvKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				lock (roleInfo)
				{
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 52, Global.GetHuoDongKeyString(fromDate, toDate), 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

		
		public static List<JieriRecvKingItemData> QueryJieriRecvKingRank(DBManager dbMgr, string fromDate, string toDate, int rankCnt)
		{
			List<JieriRecvKingItemData> result = new List<JieriRecvKingItemData>();
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT receiver, rname, zoneid, SUM(goodscnt) AS totalrecv FROM t_jierizengsong, t_roles WHERE receiver=rid AND isdel=0 AND sendtime>= '{0}' AND sendtime<='{1}' GROUP BY receiver ORDER BY totalrecv DESC, receiver ASC LIMIT {2}", fromDate, toDate, rankCnt);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int count = 0;
				while (reader.Read() && count < rankCnt)
				{
					JieriRecvKingItemData item = new JieriRecvKingItemData
					{
						RoleID = Convert.ToInt32(reader["receiver"].ToString()),
						Rolename = reader["rname"].ToString(),
						ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
						TotalRecv = Convert.ToInt32(reader["totalrecv"].ToString()),
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

		
		private JieriRecvKingItemData QueryRoleJieriRecvKing(DBManager dbMgr, string fromDate, string toDate, int roleID)
		{
			JieriRecvKingItemData result = null;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = "SELECT t_roles.rid, t_roles.rname, t_roles.zoneid, x.totalrecv from t_roles,  (SELECT t_jierizengsong.receiver, SUM(t_jierizengsong.goodscnt) AS totalrecv " + string.Format(" FROM t_jierizengsong WHERE t_jierizengsong.receiver={0} AND sendtime>= '{1}' AND sendtime<='{2}') x ", roleID, fromDate, toDate) + " where t_roles.isdel=0 and t_roles.rid = x.receiver ";
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				if (reader.Read())
				{
					result = new JieriRecvKingItemData();
					result.RoleID = Convert.ToInt32(reader["rid"].ToString());
					result.Rolename = reader["rname"].ToString();
					result.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
					result.TotalRecv = Convert.ToInt32(reader["totalrecv"].ToString());
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
