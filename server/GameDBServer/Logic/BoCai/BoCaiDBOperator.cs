using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x02000119 RID: 281
	public class BoCaiDBOperator
	{
		// Token: 0x060004AE RID: 1198 RVA: 0x000262E4 File Offset: 0x000244E4
		public static bool ReplaceOpenLottery(OpenLottery data)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_open_lottery(DataPeriods, AllBalance, SurplusBalance, XiaoHaoDaiBi, BocaiType, strWinNum, WinInfo, IsAward) VALUES({0},{1},{2},{3},{4},'{5}','{6}', {7});", new object[]
					{
						data.DataPeriods,
						data.AllBalance,
						data.SurplusBalance,
						data.XiaoHaoDaiBi,
						data.BocaiType,
						data.strWinNum,
						data.WinInfo,
						data.IsAward
					});
					ret = conn.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return ret > -1;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x000263E8 File Offset: 0x000245E8
		public static long GetMaxData(int BoCaiType)
		{
			long ret = -1L;
			MySQLConnection conn = null;
			DBManager dbMgr = DBManager.getInstance();
			long result;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string sql = string.Format("SELECT MAX(DataPeriods) as DataPeriods from t_bocai_open_lottery WHERE `BocaiType`={0}", BoCaiType);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				if (reader.Read())
				{
					string data = reader["DataPeriods"].ToString();
					if (string.IsNullOrEmpty(data))
					{
						ret = (result = 0L);
						return result;
					}
					ret = Convert.ToInt64(data);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			result = ret;
			return result;
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00026504 File Offset: 0x00024704
		public static void SelectOpenLottery(int bocaiType, out List<OpenLottery> dList)
		{
			MySQLConnection conn = null;
			DBManager dbMgr = DBManager.getInstance();
			dList = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string sql = string.Format("SELECT `DataPeriods`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo` ,`AllBalance`,`SurplusBalance` FROM t_bocai_open_lottery WHERE `IsAward` < 1 AND `BocaiType`={0};", bocaiType);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				dList = new List<OpenLottery>();
				while (reader.Read())
				{
					OpenLottery Item = new OpenLottery
					{
						strWinNum = reader["strWinNum"].ToString(),
						XiaoHaoDaiBi = Convert.ToInt32(reader["XiaoHaoDaiBi"].ToString()),
						WinInfo = reader["WinInfo"].ToString(),
						DataPeriods = Convert.ToInt64(reader["DataPeriods"].ToString()),
						AllBalance = Convert.ToInt64(reader["AllBalance"].ToString()),
						SurplusBalance = Convert.ToInt64(reader["SurplusBalance"].ToString()),
						BocaiType = bocaiType
					};
					dList.Add(Item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x000266C4 File Offset: 0x000248C4
		public static bool ReplaceBuyBoCai(BuyBoCai2SDB data)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_buy_history(rid, RoleName, ZoneID, UserID, ServerID, BuyNum, BuyValue, IsSend, IsWin, BocaiType, DataPeriods, UpdateTime)VALUES({0},'{1}',{2},'{3}',{4}, {5},'{6}', {7}, {8}, {9}, {10}, '{11}');", new object[]
					{
						data.m_RoleID,
						data.m_RoleName,
						data.ZoneID,
						data.strUserID,
						data.ServerId,
						data.BuyNum,
						data.strBuyValue,
						data.IsSend,
						data.IsWin,
						data.BocaiType,
						data.DataPeriods,
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
					});
					ret = conn.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return ret > -1;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0002680C File Offset: 0x00024A0C
		public static void SelectBuyBoCai(int bocaiType, long DataPeriods, out List<BuyBoCai2SDB> dList, bool isNoSend)
		{
			MySQLConnection conn = null;
			DBManager dbMgr = DBManager.getInstance();
			dList = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string sql;
				if (isNoSend)
				{
					sql = string.Format("SELECT `rid`,`RoleName`,`ZoneID`,`UserID`,`ServerID`,`BuyNum`,`BuyValue`,`IsSend`,`IsWin`  FROM t_bocai_buy_history WHERE `BocaiType`={0} AND `DataPeriods`={1} AND `IsSend`={2};", bocaiType, DataPeriods, 0);
				}
				else
				{
					sql = string.Format("SELECT `rid`,`RoleName`,`ZoneID`,`UserID`,`ServerID`,`BuyNum`,`BuyValue`,`IsSend`,`IsWin`  FROM t_bocai_buy_history WHERE `BocaiType`={0} AND `DataPeriods`={1};", bocaiType, DataPeriods);
				}
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				dList = new List<BuyBoCai2SDB>();
				while (reader.Read())
				{
					BuyBoCai2SDB Item = new BuyBoCai2SDB
					{
						m_RoleName = reader["RoleName"].ToString(),
						strUserID = reader["UserID"].ToString(),
						strBuyValue = reader["BuyValue"].ToString(),
						m_RoleID = Convert.ToInt32(reader["rid"].ToString()),
						ZoneID = Convert.ToInt32(reader["ZoneID"].ToString()),
						ServerId = Convert.ToInt32(reader["ServerID"].ToString()),
						BuyNum = Convert.ToInt32(reader["BuyNum"].ToString()),
						IsSend = (Convert.ToInt32(reader["IsSend"].ToString()) > 0),
						IsWin = (Convert.ToInt32(reader["IsWin"].ToString()) > 0),
						BocaiType = bocaiType,
						DataPeriods = DataPeriods
					};
					dList.Add(Item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00026A64 File Offset: 0x00024C64
		public static bool ReplaceBoCaiShop(BoCaiShopDB data)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_shop(rid, ID, BuyNum, Periods, WuPinID) VALUES({0},{1},{2},'{3}','{4}');", new object[]
					{
						data.RoleID,
						data.ID,
						data.BuyNum,
						TimeUtil.NowDataTimeString("yyMMdd"),
						data.WuPinID
					});
					ret = conn.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return ret > -1;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00026B40 File Offset: 0x00024D40
		public static void SelectBoCaiShop(string Periods, out List<BoCaiShopDB> dList)
		{
			dList = null;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT rid, ID, BuyNum, WuPinID from t_bocai_shop where Periods={0}", Periods);
					MySQLDataReader reader = conn.ExecuteReader(sql, new MySQLParameter[0]);
					dList = new List<BoCaiShopDB>();
					while (reader.Read())
					{
						BoCaiShopDB temp = new BoCaiShopDB();
						temp.RoleID = Convert.ToInt32(reader["rid"].ToString());
						temp.ID = Convert.ToInt32(reader["ID"].ToString());
						temp.BuyNum = Convert.ToInt32(reader["BuyNum"].ToString());
						temp.WuPinID = reader["WuPinID"].ToString();
						temp.Periods = Convert.ToInt32(Periods);
						dList.Add(temp);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00026C6C File Offset: 0x00024E6C
		public static bool DelData(string table, string Sql)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					ret = conn.ExecuteNonQuery(string.Format("delete from {1} where {0}", Sql, table), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return ret > -1;
		}
	}
}
