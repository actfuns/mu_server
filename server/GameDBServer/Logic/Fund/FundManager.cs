using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Fund
{
	
	public class FundManager : SingletonTemplate<FundManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13116, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13117, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13118, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13119, SingletonTemplate<FundManager>.Instance());
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

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 13116)
			{
				this.FundInfo(client, nID, cmdParams, count);
			}
			else if (nID == 13117)
			{
				this.FundBuy(client, nID, cmdParams, count);
			}
			else if (nID == 13118)
			{
				this.FundAward(client, nID, cmdParams, count);
			}
			else if (nID == 13119)
			{
				this.FundMoney(client, nID, cmdParams, count);
			}
		}

		
		private void FundInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<FundDBItem> list = new List<FundDBItem>();
			MySQLConnection conn = null;
			try
			{
				int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				string cmdText = string.Format("SELECT zoneID,userID,roleID,fundType,fundID,buyTime,awardID,value1,value2,state from t_fund where state>0 and roleID={0}", roleId);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				while (reader.Read())
				{
					list.Add(new FundDBItem
					{
						ZoneID = int.Parse(reader["zoneID"].ToString()),
						UserID = reader["userID"].ToString(),
						RoleID = int.Parse(reader["roleID"].ToString()),
						FundType = int.Parse(reader["fundType"].ToString()),
						FundID = int.Parse(reader["fundID"].ToString()),
						BuyTime = DateTime.Parse(reader["buyTime"].ToString()),
						AwardID = int.Parse(reader["awardID"].ToString()),
						Value1 = int.Parse(reader["value1"].ToString()),
						Value2 = int.Parse(reader["value2"].ToString()),
						State = int.Parse(reader["state"].ToString())
					});
				}
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<List<FundDBItem>>(nID, list);
		}

		
		private void FundBuy(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			MySQLConnection conn = null;
			try
			{
				FundDBItem item = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				string cmdText = string.Format("UPDATE t_fund SET state='0' where zoneID='{0}' and userID='{1}' and roleID='{2}' and fundType='{3}' and state='1'", new object[]
				{
					item.ZoneID,
					item.UserID,
					item.RoleID,
					item.FundType
				});
				string cmdText2 = string.Format("INSERT INTO t_fund(zoneID,userID,roleID,fundType,fundID,buyTime) VALUE('{0}','{1}','{2}','{3}','{4}','{5}')", new object[]
				{
					item.ZoneID,
					item.UserID,
					item.RoleID,
					item.FundType,
					item.FundID,
					item.BuyTime
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText2), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				bResult = (cmd.ExecuteNonQuery() > 0);
				cmd = new MySQLCommand(cmdText2, conn);
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				bResult = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				bResult = false;
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<bool>(nID, bResult);
		}

		
		private void FundAward(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				FundDBItem item = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				string cmdText = string.Format("UPDATE t_fund SET awardID='{0}',state='{1}' where zoneID='{2}' and userID='{3}' and roleID='{4}' and fundType='{5}' and fundID='{6}'", new object[]
				{
					item.AwardID,
					item.State,
					item.ZoneID,
					item.UserID,
					item.RoleID,
					item.FundType,
					item.FundID
				});
				bResult = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			client.sendCmd<bool>(nID, bResult);
		}

		
		private void FundMoney(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			try
			{
				FundDBItem item = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				bResult = this.FundAddMoney(item.UserID, item.Value1, item.RoleID, item.Value2);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			client.sendCmd<bool>(nID, bResult);
		}

		
		public bool FundAddMoney(string userID, int moneyAdd, int roleID, int moneyCost = 0)
		{
			bool result = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_fund SET value1=value1+{0},value2=value2+{1} where userID='{2}' and fundType=3 and state=1", moneyAdd, moneyCost, userID);
				DBRoleInfo roleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
				if (roleInfo == null)
				{
					return false;
				}
				cmdText = string.Format("{0} and zoneID={1}", cmdText, roleInfo.ZoneID);
				if (moneyCost > 0 && roleID > 0)
				{
					cmdText = string.Format("{0} and roleID={1}", cmdText, roleID);
				}
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}
	}
}
