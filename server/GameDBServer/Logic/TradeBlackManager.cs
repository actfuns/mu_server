using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000180 RID: 384
	public class TradeBlackManager : SingletonTemplate<TradeBlackManager>, IManager, ICmdProcessor
	{
		// Token: 0x060006B7 RID: 1719 RVA: 0x0003DAEA File Offset: 0x0003BCEA
		private TradeBlackManager()
		{
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0003DAF8 File Offset: 0x0003BCF8
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0003DB0C File Offset: 0x0003BD0C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14007, SingletonTemplate<TradeBlackManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14008, SingletonTemplate<TradeBlackManager>.Instance());
			return true;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0003DB4C File Offset: 0x0003BD4C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0003DB60 File Offset: 0x0003BD60
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0003DB74 File Offset: 0x0003BD74
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 14007)
			{
				this.HandleLoad(client, nID, cmdParams, count);
			}
			else if (nID == 14008)
			{
				this.handleSave(client, nID, cmdParams, count);
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0003DBC0 File Offset: 0x0003BDC0
		private void HandleLoad(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<TradeBlackHourItem> items = null;
			MySQLConnection conn = null;
			try
			{
				string reqCmd = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] fields = reqCmd.Split(new char[]
				{
					':'
				});
				int roleid = Convert.ToInt32(fields[0]);
				string currDay = fields[1].Trim();
				int currHour = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRole = DBManager.getInstance().GetDBRoleInfo(ref roleid);
				if (dbRole == null)
				{
					throw new Exception("TradeBlackManager.HandleLoad not Find DBRoleInfo, roleid=" + roleid);
				}
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				string sql = string.Format("SELECT day,hour,distinct_roles,market_times,market_in_price,market_out_price,trade_times,trade_in_price,trade_out_price FROM t_ban_trade WHERE rid={0} AND ((TO_DAYS('{1}') - TO_DAYS(day) = 0 AND hour <= {2}) or (TO_DAYS('{1}') - TO_DAYS(day) = 1 AND hour > {2}))", roleid, currDay, currHour);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				items = new List<TradeBlackHourItem>();
				while (reader.Read())
				{
					items.Add(new TradeBlackHourItem
					{
						RoleId = roleid,
						Day = reader["day"].ToString().Trim(),
						Hour = Convert.ToInt32(reader["hour"].ToString()),
						TradeDistinctRoleCount = Convert.ToInt32(reader["distinct_roles"].ToString()),
						MarketInPrice = Convert.ToInt64(reader["market_in_price"]),
						MarketTimes = Convert.ToInt32(reader["market_times"]),
						MarketOutPrice = Convert.ToInt64(reader["market_out_price"]),
						TradeInPrice = Convert.ToInt64(reader["Trade_in_price"]),
						TradeTimes = Convert.ToInt32(reader["Trade_times"]),
						TradeOutPrice = Convert.ToInt64(reader["Trade_out_price"])
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException("TradeBlackManager.HandleLoad " + ex.Message);
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<List<TradeBlackHourItem>>(nID, items);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0003DE40 File Offset: 0x0003C040
		private void handleSave(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			MySQLConnection conn = null;
			try
			{
				TradeBlackHourItem data = DataHelper.BytesToObject<TradeBlackHourItem>(cmdParams, 0, count);
				DBRoleInfo dbRole = DBManager.getInstance().GetDBRoleInfo(ref data.RoleId);
				if (dbRole == null)
				{
					throw new Exception("TradeBlackManager.handleSave not Find DBRoleInfo, roleid=" + data.RoleId);
				}
				string sql = string.Format("REPLACE INTO t_ban_trade(rid,day,hour,market_in_price,market_times,market_out_price,Trade_in_price,Trade_times,Trade_out_price,distinct_roles)  VALUES({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9})", new object[]
				{
					data.RoleId,
					data.Day,
					data.Hour,
					data.MarketInPrice,
					data.MarketTimes,
					data.MarketOutPrice,
					data.TradeInPrice,
					data.TradeTimes,
					data.TradeOutPrice,
					data.TradeDistinctRoleCount
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				bResult = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("TradeBlackManager.handleSave " + ex.Message);
				bResult = false;
			}
			finally
			{
				if (conn != null)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<bool>(nID, bResult);
		}
	}
}
