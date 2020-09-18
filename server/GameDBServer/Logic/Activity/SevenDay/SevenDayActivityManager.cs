using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Activity.SevenDay
{
	
	public class SevenDayActivityManager : SingletonTemplate<SevenDayActivityManager>, IManager, ICmdProcessor
	{
		
		private SevenDayActivityManager()
		{
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13220, SingletonTemplate<SevenDayActivityManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13221, SingletonTemplate<SevenDayActivityManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13222, SingletonTemplate<SevenDayActivityManager>.Instance());
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
			if (nID == 13220)
			{
				this.HandleUpdate(client, nID, cmdParams, count);
			}
			else if (nID == 13221)
			{
				this.HandleClear(client, nID, cmdParams, count);
			}
			else if (nID == 13222)
			{
				this.HandleQueryCharge(client, nID, cmdParams, count);
			}
		}

		
		private void HandleQueryCharge(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 3)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int roleId = Convert.ToInt32(fields[0]);
				string szStartTime = fields[1].Replace('$', ':');
				string szEndTime = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleId);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("七日充值，找不到玩家 roleid={0}", roleId), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					Dictionary<string, int> result = this._QueryEachDayChargeYB(DBManager.getInstance(), szStartTime, szEndTime, roleInfo.UserID, roleInfo.ZoneID);
					client.sendCmd<Dictionary<string, int>>(nID, result);
				}
			}
		}

		
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
					string cmdText = string.Format("SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND zoneid={3} AND result='success' GROUP BY inputdate UNION ALL  SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog2 WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND zoneid={3} AND result='success' GROUP BY inputdate", new object[]
					{
						userid,
						fromDate,
						toDate,
						zoneid
					});
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
							if (!eachDayChargeYB.ContainsKey(inputDate))
							{
								eachDayChargeYB.Add(inputDate, inputMoney);
							}
							else
							{
								Dictionary<string, int> dictionary;
								string key;
								(dictionary = eachDayChargeYB)[key = inputDate] = dictionary[key] + inputMoney;
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

		
		private void HandleClear(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			try
			{
				int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				DBRoleInfo dbRole = DBManager.getInstance().GetDBRoleInfo(ref roleId);
				if (dbRole == null)
				{
					throw new Exception("SevenDayActivityManager.HandleClear not Find DBRoleInfo, roleid=" + roleId);
				}
				string sql = string.Format("DELETE FROM t_seven_day_act where roleid={0}", roleId);
				if (!this.ExecNonQuery(sql))
				{
					throw new Exception("SevenDayActivityManager.HandleClear ExecSql Failed, sql= " + sql);
				}
				lock (dbRole.SevenDayActDict)
				{
					dbRole.SevenDayActDict.Clear();
				}
				bResult = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message, null, true);
				bResult = false;
			}
			client.sendCmd<bool>(nID, bResult);
		}

		
		private void HandleUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			try
			{
				SevenDayUpdateDbData data = DataHelper.BytesToObject<SevenDayUpdateDbData>(cmdParams, 0, count);
				DBRoleInfo dbRole = DBManager.getInstance().GetDBRoleInfo(ref data.RoleId);
				if (dbRole == null)
				{
					throw new Exception("SevenDayActivityManager.HandleUpdate not Find DBRoleInfo, roleid=" + data.RoleId);
				}
				string sql = string.Format("REPLACE INTO t_seven_day_act(roleid,act_type,id,award_flag,param1,param2) VALUES({0},{1},{2},{3},{4},{5})", new object[]
				{
					data.RoleId,
					data.ActivityType,
					data.Id,
					data.Data.AwardFlag,
					data.Data.Params1,
					data.Data.Params2
				});
				if (!this.ExecNonQuery(sql))
				{
					throw new Exception("SevenDayActivityManager.HandleUpdate ExecSql Failed, sql= " + sql);
				}
				lock (dbRole.SevenDayActDict)
				{
					Dictionary<int, SevenDayItemData> itemDict = null;
					if (!dbRole.SevenDayActDict.TryGetValue(data.ActivityType, out itemDict))
					{
						itemDict = new Dictionary<int, SevenDayItemData>();
						dbRole.SevenDayActDict[data.ActivityType] = itemDict;
					}
					itemDict[data.Id] = data.Data;
				}
				bResult = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message, null, true);
				bResult = false;
			}
			client.sendCmd<bool>(nID, bResult);
		}

		
		private bool ExecNonQuery(string sql)
		{
			bool bResult = false;
			MySQLConnection conn = null;
			try
			{
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				bResult = true;
				return true;
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
			return bResult;
		}
	}
}
