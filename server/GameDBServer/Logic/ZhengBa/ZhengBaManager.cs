using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.ZhengBa
{
	
	public class ZhengBaManager : SingletonTemplate<ZhengBaManager>, IManager, ICmdProcessor
	{
		
		private ZhengBaManager()
		{
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14014, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14013, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14015, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14012, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14011, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14016, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14017, SingletonTemplate<ZhengBaManager>.Instance());
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

		
		private bool ExecNonQuery(string sql)
		{
			bool bResult = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				bResult = conn.ExecuteNonQueryBool(sql, 0);
			}
			return bResult;
		}

		
		private MySQLDataReader ExecSelect(string sql)
		{
			MySQLConnection conn = null;
			MySQLDataReader result;
			try
			{
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				cmd.Dispose();
				result = reader;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = null;
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 14014)
			{
				this.HandleLoadPkLog(client, nID, cmdParams, count);
			}
			else if (nID == 14013)
			{
				this.HandleLoadSupportLog(client, nID, cmdParams, count);
			}
			else if (nID == 14015)
			{
				this.HandleLoadSupportFlag(client, nID, cmdParams, count);
			}
			else if (nID == 14012)
			{
				this.HandleSavePkLog(client, nID, cmdParams, count);
			}
			else if (nID == 14011)
			{
				this.HandleSaveSupportLog(client, nID, cmdParams, count);
			}
			else if (nID == 14016)
			{
				this.HandleLoadWaitAwardYaZhu(client, nID, cmdParams, count);
			}
			else if (nID == 14017)
			{
				this.HandleSetYaZhuAward(client, nID, cmdParams, count);
			}
		}

		
		private void HandleSetYaZhuAward(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
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
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int month = Convert.ToInt32(fields[0]);
					int roleId = Convert.ToInt32(fields[1]);
					int unionGroup = Convert.ToInt32(fields[2]);
					int group = Convert.ToInt32(fields[3]);
					string logSql = string.Format("UPDATE t_zhengba_support_flag SET is_award=1 WHERE month={0} AND from_rid={1} AND to_union_group={2} AND to_group={3} AND support_type={4};", new object[]
					{
						month,
						roleId,
						unionGroup,
						group,
						3
					});
					if (!this.ExecNonQuery(logSql))
					{
					}
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		
		private void HandleLoadWaitAwardYaZhu(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
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
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int month = Convert.ToInt32(fields[0]);
					string querySql = string.Format("SELECT to_union_group,to_group,rank_of_day,is_award,from_rid FROM t_zhengba_support_flag WHERE support_type={0} AND month={1} AND is_award=0", 3, month);
					MySQLDataReader reader = this.ExecSelect(querySql);
					List<ZhengBaWaitYaZhuAwardData> waitAwardYaZhuList = new List<ZhengBaWaitYaZhuAwardData>();
					while (reader != null && reader.Read())
					{
						waitAwardYaZhuList.Add(new ZhengBaWaitYaZhuAwardData
						{
							FromRoleId = Convert.ToInt32(reader["from_rid"].ToString()),
							Month = month,
							RankOfDay = Convert.ToInt32(reader["rank_of_day"].ToString()),
							UnionGroup = Convert.ToInt32(reader["to_union_group"].ToString()),
							Group = Convert.ToInt32(reader["to_group"].ToString())
						});
					}
					client.sendCmd<List<ZhengBaWaitYaZhuAwardData>>(nID, waitAwardYaZhuList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<List<ZhengBaWaitYaZhuAwardData>>(nID, null);
			}
		}

		
		private void HandleSaveSupportLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ZhengBaSupportLogData data = DataHelper.BytesToObject<ZhengBaSupportLogData>(cmdParams, 0, count);
				if (data.FromServerId == GameDBManager.ZoneID)
				{
					string flagSql = string.Format("INSERT INTO t_zhengba_support_flag(month,rank_of_day,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,from_serverid) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9})", new object[]
					{
						data.Month,
						data.RankOfDay,
						data.FromRoleId,
						data.FromZoneId,
						data.FromRolename,
						data.SupportType,
						data.ToUnionGroup,
						data.ToGroup,
						data.Time.ToString("yyyy-MM-dd HH:mm:ss"),
						data.FromServerId
					});
					if (!this.ExecNonQuery(flagSql))
					{
						client.sendCmd<bool>(nID, false);
						return;
					}
				}
				string updateSql = string.Format("INSERT INTO t_zhengba_support_log(month,rank_of_day,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,from_serverid) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9})", new object[]
				{
					data.Month,
					data.RankOfDay,
					data.FromRoleId,
					data.FromZoneId,
					data.FromRolename,
					data.SupportType,
					data.ToUnionGroup,
					data.ToGroup,
					data.Time.ToString("yyyy-MM-dd HH:mm:ss"),
					data.FromServerId
				});
				if (!this.ExecNonQuery(updateSql))
				{
				}
				client.sendCmd<bool>(nID, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		
		private void HandleSavePkLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ZhengBaPkLogData data = DataHelper.BytesToObject<ZhengBaPkLogData>(cmdParams, 0, count);
				string logSql = string.Format("INSERT INTO t_zhengba_pk_log(month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9},{10},{11},'{12}','{13}')", new object[]
				{
					data.Month,
					data.Day,
					data.RoleID1,
					data.ZoneID1,
					data.RoleName1,
					data.IsMirror1 ? 1 : 0,
					data.RoleID2,
					data.ZoneID2,
					data.RoleName2,
					data.IsMirror2 ? 1 : 0,
					data.PkResult,
					data.UpGrade ? 1 : 0,
					data.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
					data.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				if (!this.ExecNonQuery(logSql))
				{
					client.sendCmd<bool>(nID, false);
				}
				else
				{
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		
		private void HandleLoadSupportFlag(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int roleId = Convert.ToInt32(fields[0]);
				int month = Convert.ToInt32(fields[1]);
				DBRoleInfo roleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleId);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("HandleLoadSupportFlag，找不到玩家 roleid={0}", roleId), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string sql = string.Format("SELECT support_type,to_union_group,to_group,rank_of_day FROM t_zhengba_support_flag WHERE from_rid={0} AND month={1}", roleId, month);
					MySQLDataReader reader = this.ExecSelect(sql);
					List<ZhengBaSupportFlagData> data = new List<ZhengBaSupportFlagData>();
					while (reader != null && reader.Read())
					{
						int toUnionGroup = Convert.ToInt32(reader["to_union_group"].ToString());
						int toGroup = Convert.ToInt32(reader["to_group"].ToString());
						int supportDay = Convert.ToInt32(reader["rank_of_day"].ToString());
						int supportType = Convert.ToInt32(reader["support_type"].ToString());
						ZhengBaSupportFlagData flag = data.Find((ZhengBaSupportFlagData _f) => _f.UnionGroup == toUnionGroup && _f.Group == toGroup);
						if (flag == null)
						{
							data.Add(flag = new ZhengBaSupportFlagData
							{
								UnionGroup = toUnionGroup,
								Group = toGroup,
								SupportDay = supportDay
							});
						}
						if (supportType == 2)
						{
							flag.IsOppose = true;
						}
						else if (supportType == 1)
						{
							flag.IsSupport = true;
						}
						else if (supportType == 3)
						{
							flag.IsYaZhu = true;
						}
					}
					client.sendCmd<List<ZhengBaSupportFlagData>>(nID, data);
				}
			}
		}

		
		private void HandleLoadSupportLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int month = Convert.ToInt32(fields[0]);
				int maxNum = Convert.ToInt32(fields[1]);
				List<int> toUnionGropuList = new List<int>();
				string sql = string.Format("SELECT DISTINCT(to_union_group) FROM t_zhengba_support_log WHERE `month`={0}", month);
				MySQLDataReader reader = this.ExecSelect(sql);
				while (reader != null && reader.Read())
				{
					int toUnionGroup = Convert.ToInt32(reader["to_union_group"].ToString());
					toUnionGropuList.Add(toUnionGroup);
				}
				Dictionary<int, List<ZhengBaSupportLogData>> supportLogs = new Dictionary<int, List<ZhengBaSupportLogData>>();
				foreach (int g in toUnionGropuList)
				{
					sql = string.Format("SELECT from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,month,rank_of_day,from_serverid FROM t_zhengba_support_log WHERE month={0} AND `to_union_group`={1} ORDER BY `time` DESC limit {2};", month, g, maxNum);
					reader = this.ExecSelect(sql);
					while (reader != null && reader.Read())
					{
						int toUnionGroup = Convert.ToInt32(reader["to_union_group"].ToString());
						List<ZhengBaSupportLogData> LogList = null;
						if (!supportLogs.TryGetValue(toUnionGroup, out LogList))
						{
							LogList = (supportLogs[toUnionGroup] = new List<ZhengBaSupportLogData>());
						}
						ZhengBaSupportLogData log = new ZhengBaSupportLogData();
						log.FromRoleId = Convert.ToInt32(reader["from_rid"].ToString());
						log.FromZoneId = Convert.ToInt32(reader["from_zoneid"].ToString());
						log.FromRolename = reader["from_rolename"].ToString();
						log.SupportType = Convert.ToInt32(reader["support_type"].ToString());
						log.ToUnionGroup = toUnionGroup;
						log.ToGroup = Convert.ToInt32(reader["to_group"].ToString());
						log.Month = Convert.ToInt32(reader["month"].ToString());
						log.RankOfDay = Convert.ToInt32(reader["rank_of_day"].ToString());
						log.FromServerId = Convert.ToInt32(reader["from_serverid"].ToString());
						log.Time = DateTime.Parse(reader["time"].ToString());
						LogList.Add(log);
					}
				}
				foreach (List<ZhengBaSupportLogData> logList in supportLogs.Values)
				{
					logList.Reverse();
				}
				client.sendCmd<Dictionary<int, List<ZhengBaSupportLogData>>>(nID, supportLogs);
			}
		}

		
		private void HandleLoadPkLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int month = Convert.ToInt32(fields[0]);
				int maxNum = Convert.ToInt32(fields[1]);
				string sql = string.Format("SELECT month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime FROM t_zhengba_pk_log WHERE month={0} ORDER BY endtime DESC LIMIT {1};", month, maxNum);
				MySQLDataReader reader = this.ExecSelect(sql);
				List<ZhengBaPkLogData> logList = new List<ZhengBaPkLogData>();
				while (reader != null && reader.Read())
				{
					logList.Add(new ZhengBaPkLogData
					{
						Day = Convert.ToInt32(reader["day"].ToString()),
						Month = Convert.ToInt32(reader["month"].ToString()),
						RoleID1 = Convert.ToInt32(reader["rid1"].ToString()),
						ZoneID1 = Convert.ToInt32(reader["zoneid1"].ToString()),
						RoleName1 = reader["rname1"].ToString(),
						IsMirror1 = (Convert.ToInt32(reader["ismirror1"].ToString()) == 1),
						RoleID2 = Convert.ToInt32(reader["rid2"].ToString()),
						ZoneID2 = Convert.ToInt32(reader["zoneid2"].ToString()),
						RoleName2 = reader["rname2"].ToString(),
						IsMirror2 = (Convert.ToInt32(reader["ismirror2"].ToString()) == 1),
						PkResult = Convert.ToInt32(reader["result"].ToString()),
						UpGrade = (Convert.ToInt32(reader["upgrade"].ToString()) == 1),
						StartTime = DateTime.Parse(reader["starttime"].ToString()),
						EndTime = DateTime.Parse(reader["endtime"].ToString())
					});
				}
				logList.Reverse();
				client.sendCmd<List<ZhengBaPkLogData>>(nID, logList);
			}
		}

		
		private enum EZhengBaSupport
		{
			
			Invalid,
			
			Support,
			
			Oppose,
			
			YaZhu
		}
	}
}
