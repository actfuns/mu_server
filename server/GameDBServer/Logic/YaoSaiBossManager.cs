using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class YaoSaiBossManager : SingletonTemplate<YaoSaiBossManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20306, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20307, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20308, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20309, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20310, SingletonTemplate<YaoSaiBossManager>.Instance());
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
			switch (nID)
			{
			case 20306:
				this.GetYaoSaiBossData(client, nID, cmdParams, count);
				break;
			case 20307:
				this.SetYaoSaiBossData(client, nID, cmdParams, count);
				break;
			case 20308:
				this.GetYaoSaiBossFightLog(client, nID, cmdParams, count);
				break;
			case 20309:
				this.SetYaoSaiBossFightLog(client, nID, cmdParams, count);
				break;
			case 20310:
				this.CleanYaoSaiBossFightData(client, nID, cmdParams, count);
				break;
			}
		}

		
		public void GetYaoSaiBossData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, YaoSaiBossData> roleBossDataDict = new Dictionary<int, YaoSaiBossData>();
				MySQLConnection conn = null;
				try
				{
					RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
					string cmdText = "select * from t_yaosaiboss";
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					conn = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					while (reader.Read())
					{
						int rid = int.Parse(reader["rid"].ToString());
						int bossID = int.Parse(reader["bossID"].ToString());
						int bossLife = int.Parse(reader["bosslife"].ToString());
						DateTime deadTime = DateTime.Parse(reader["deadtime"].ToString());
						roleBossDataDict[rid] = new YaoSaiBossData
						{
							OwnerID = rid,
							BossID = bossID,
							LifeV = (double)bossLife,
							DeadTime = deadTime
						};
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
				client.sendCmd<Dictionary<int, YaoSaiBossData>>(nID, roleBossDataDict);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void GetYaoSaiBossFightLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiBossFightLog>> bossFightDict = new Dictionary<int, List<YaoSaiBossFightLog>>();
				MySQLConnection conn = null;
				try
				{
					RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
					string cmdText = "select * from t_yaosaiboss_fight";
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					conn = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					while (reader.Read())
					{
						int rid = int.Parse(reader["rid"].ToString());
						int otherrid = int.Parse(reader["otherrid"].ToString());
						string otherrname = reader["otherrname"].ToString();
						int invitetype = int.Parse(reader["invitetype"].ToString());
						int fightlife = int.Parse(reader["fightlife"].ToString());
						List<YaoSaiBossFightLog> fightLogList = null;
						if (!bossFightDict.TryGetValue(rid, out fightLogList))
						{
							fightLogList = new List<YaoSaiBossFightLog>();
							bossFightDict[rid] = fightLogList;
						}
						fightLogList.Add(new YaoSaiBossFightLog
						{
							OtherRid = otherrid,
							OtherRname = otherrname,
							InviteType = invitetype,
							FightLife = fightlife
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
				client.sendCmd<Dictionary<int, List<YaoSaiBossFightLog>>>(nID, bossFightDict);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void SetYaoSaiBossData(GameServerClient client, int nID, byte[] cmdParams, int count)
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
					string cmdText = string.Format("insert into t_yaosaiboss (rid,bossID,bosslife,deadtime) values ({0}, {1}, {2}, '{3}') on duplicate key update bosslife={2}", new object[]
					{
						fields[0],
						fields[1],
						fields[2],
						fields[3].Replace('$', ':')
					});
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						conn.ExecuteNonQuery(cmdText, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void SetYaoSaiBossFightLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string cmdText = string.Format("insert into t_yaosaiboss_fight (rid,otherrid,otherrname,invitetype,fightlife) values ({0}, {1}, '{2}', {3}, {4})", new object[]
					{
						fields[0],
						fields[1],
						fields[2],
						fields[3],
						fields[4]
					});
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						conn.ExecuteNonQuery(cmdText, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void CleanYaoSaiBossFightData(GameServerClient client, int nID, byte[] cmdParams, int count)
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
					string cmdText = string.Format("delete from t_yaosaiboss where rid={0}", fields[0]);
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						conn.ExecuteNonQuery(cmdText, 0);
					}
					cmdText = string.Format("delete from t_yaosaiboss_fight where rid={0}", fields[0]);
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						conn.ExecuteNonQuery(cmdText, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}
	}
}
