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
	
	public class YaoSaiMissionManager : SingletonTemplate<YaoSaiMissionManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20311, SingletonTemplate<YaoSaiMissionManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20312, SingletonTemplate<YaoSaiMissionManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20313, SingletonTemplate<YaoSaiMissionManager>.Instance());
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
			case 20311:
				this.GetYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			case 20312:
				this.SetYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			case 20313:
				this.DelYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			}
		}

		
		public List<YaoSaiMissionData> GetYaoSaiMissionDataByRid(int rid)
		{
			try
			{
				List<YaoSaiMissionData> roleMissionList = new List<YaoSaiMissionData>();
				MySQLConnection conn = null;
				try
				{
					string cmdText = string.Format("select * from t_yaosaimission where rid='{0}'", rid);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					conn = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					while (reader.Read())
					{
						int siteid = int.Parse(reader["siteid"].ToString());
						int missionid = int.Parse(reader["missionid"].ToString());
						int state = int.Parse(reader["state"].ToString());
						string zhipaijingling = reader["zhipaijingling"].ToString();
						DateTime starttime = DateTime.Parse(reader["starttime"].ToString());
						roleMissionList.Add(new YaoSaiMissionData
						{
							SiteID = siteid,
							MissionID = missionid,
							State = state,
							ZhiPaiJingLing = zhipaijingling,
							StartTime = starttime
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
				return roleMissionList;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色任务信息错误, ex={1}", ex.Message), null, true);
			}
			return null;
		}

		
		public void GetYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(nID, null);
				}
				client.sendCmd<List<YaoSaiMissionData>>(nID, this.GetYaoSaiMissionDataByRid(Convert.ToInt32(fields[0])));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void SetYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiMissionData>> missionDict = DataHelper.BytesToObject<Dictionary<int, List<YaoSaiMissionData>>>(cmdParams, 0, count);
				foreach (KeyValuePair<int, List<YaoSaiMissionData>> item in missionDict)
				{
					foreach (YaoSaiMissionData mission in item.Value)
					{
						string cmdText = string.Format("insert into t_yaosaimission values ({0},{1},{2},{3},'{4}','{5}') on duplicate key update missionid={2},state={3},zhipaijingling='{4}',starttime='{5}';\r\n", new object[]
						{
							item.Key,
							mission.SiteID,
							mission.MissionID,
							mission.State,
							mission.ZhiPaiJingLing,
							mission.StartTime
						});
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							conn.ExecuteNonQuery(cmdText, 0);
						}
					}
				}
				client.sendCmd<int>(nID, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 更新角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public void DelYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiMissionData>> missionDict = DataHelper.BytesToObject<Dictionary<int, List<YaoSaiMissionData>>>(cmdParams, 0, count);
				foreach (KeyValuePair<int, List<YaoSaiMissionData>> item in missionDict)
				{
					foreach (YaoSaiMissionData mission in item.Value)
					{
						string cmdText = string.Format("delete from t_yaosaimission where rid={0} and siteid={1}", item.Key, mission.SiteID);
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							conn.ExecuteNonQuery(cmdText, 0);
						}
					}
				}
				client.sendCmd<int>(nID, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 更新角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		
		public Dictionary<int, List<YaoSaiMissionData>> RoleMissionDict = new Dictionary<int, List<YaoSaiMissionData>>();
	}
}
