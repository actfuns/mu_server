using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	public class KuaFuCopyDbMgr
	{
		
		private KuaFuCopyDbMgr()
		{
		}

		
		
		public static KuaFuCopyDbMgr Instance
		{
			get
			{
				return KuaFuCopyDbMgr.g_Instance;
			}
		}

		
		
		
		public KFTeamCountControl TeamControl
		{
			get
			{
				KFTeamCountControl control;
				lock (this._ControlMutex)
				{
					control = this._Control;
				}
				return control;
			}
			private set
			{
				lock (this._ControlMutex)
				{
					this._Control = value;
				}
			}
		}

		
		public void InitConfig()
		{
			try
			{
				KFTeamCountControl control = new KFTeamCountControl();
				XElement xmlFile = ConfigHelper.Load("config.xml");
				control.TeamMaxWaitMinutes = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "FuBenTeamMaxWaitMinutes", "value", 10L);
				this.TeamControl = control;
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(LogTypes.Warning, "KFCopyTeam 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		
		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -1;
			}
			return result;
		}

		
		public void CheckLogAsyncEvents(AsyncDataItem[] evList)
		{
			if (evList != null)
			{
				foreach (AsyncDataItem ev in evList)
				{
					string sql = string.Empty;
					try
					{
						if (ev.EventType == KuaFuEventTypes.KFCopyTeamCreate)
						{
							CopyTeamCreateData data = ev.Args[1] as CopyTeamCreateData;
							sql = string.Format("INSERT INTO t_kuafu_fuben_game_team(teamid,copyid,by_serverid,by_roleid,createtime) VALUES({0},{1},{2},{3},'{4}')", new object[]
							{
								data.TeamId,
								data.CopyId,
								data.Member.ServerId,
								data.Member.RoleID,
								TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
							});
						}
						else if (ev.EventType == KuaFuEventTypes.KFCopyTeamStart)
						{
							CopyTeamStartData data2 = ev.Args[1] as CopyTeamStartData;
							sql = string.Format("UPDATE t_kuafu_fuben_game_team SET starttime='{0}', kf_serverid={1} WHERE teamid={2}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), data2.ToServerId, data2.TeamId);
						}
						else if (ev.EventType == KuaFuEventTypes.KFCopyTeamDestroty)
						{
							CopyTeamDestroyData data3 = ev.Args[0] as CopyTeamDestroyData;
							sql = string.Format("UPDATE t_kuafu_fuben_game_team SET endtime='{0}' WHERE teamid={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), data3.TeamId);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Error, "KuaFuCopyDbMgr.CheckLogAsyncEvents Failed!!!", ex, true);
					}
					if (!string.IsNullOrEmpty(sql))
					{
						this.ExecuteSqlNoQuery(sql);
					}
				}
			}
		}

		
		public void SaveCopyTeamAnalysisData(KFCopyTeamAnalysis data)
		{
			if (data != null)
			{
				string nowTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				KFCopyTeamAnalysis.Item totalItem = new KFCopyTeamAnalysis.Item();
				foreach (KeyValuePair<int, KFCopyTeamAnalysis.Item> kvp in data.AnalysisDict)
				{
					int copyId = kvp.Key;
					KFCopyTeamAnalysis.Item item = kvp.Value;
					totalItem.TotalCopyCount += item.TotalCopyCount;
					totalItem.StartCopyCount += item.StartCopyCount;
					totalItem.UnStartCopyCount += item.UnStartCopyCount;
					totalItem.TotalRoleCount += item.TotalRoleCount;
					totalItem.StartRoleCount += item.StartRoleCount;
					totalItem.UnStartRoleCount += item.UnStartRoleCount;
					string sql = string.Format("REPLACE INTO t_kuafu_fuben_game_log(fuben_id,total_fuben_count,start_fuben_count,unstart_fuben_count,total_role_count,start_role_count,unstart_role_count,time)  VALUES({0},{1},{2},{3},{4},{5},{6},'{7}') ", new object[]
					{
						copyId,
						item.TotalCopyCount,
						item.StartCopyCount,
						item.UnStartCopyCount,
						item.TotalRoleCount,
						item.StartRoleCount,
						item.UnStartRoleCount,
						nowTime
					});
					this.ExecuteSqlNoQuery(sql);
				}
				string totalSql = string.Format("REPLACE INTO t_kuafu_fuben_game_log(fuben_id,total_fuben_count,start_fuben_count,unstart_fuben_count,total_role_count,start_role_count,unstart_role_count,time)  VALUES({0},{1},{2},{3},{4},{5},{6},'{7}') ", new object[]
				{
					-1,
					totalItem.TotalCopyCount,
					totalItem.StartCopyCount,
					totalItem.UnStartCopyCount,
					totalItem.TotalRoleCount,
					totalItem.StartRoleCount,
					totalItem.UnStartRoleCount,
					nowTime
				});
				this.ExecuteSqlNoQuery(totalSql);
				string deleteSql = string.Format("DELETE FROM t_kuafu_fuben_game_log WHERE time != '{0}'", nowTime);
				this.ExecuteSqlNoQuery(deleteSql);
			}
		}

		
		public int AddHongDongAwardRecordForUser(string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			string cmdText = string.Format("INSERT INTO t_huodongawarduserhist (userid, activitytype, keystr, hasgettimes,lastgettime) VALUES('{0}', {1}, '{2}', {3}, '{4}')", new object[]
			{
				userid,
				activitytype,
				keystr,
				hasgettimes,
				lastgettime
			});
			return this.ExecuteSqlNoQuery(cmdText);
		}

		
		public int UpdateHongDongAwardRecordForUser(string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			string cmdText = string.Format("update t_huodongawarduserhist set hasgettimes={0}, lastgettime='{1}' where userid='{2}' and activitytype={3} and keystr='{4}' and hasgettimes!={5}", new object[]
			{
				hasgettimes,
				lastgettime,
				userid,
				activitytype,
				keystr,
				hasgettimes
			});
			return this.ExecuteSqlNoQuery(cmdText);
		}

		
		public int GetAwardHistoryForUser(string userid, int activitytype, string keystr, out long hasgettimes, out string lastgettime)
		{
			hasgettimes = 0L;
			lastgettime = "";
			int ret = -1;
			MySqlDataReader reader = null;
			try
			{
				string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
				reader = DbHelperMySQL.ExecuteReader(cmdText, false);
				if (reader.Read())
				{
					hasgettimes = Convert.ToInt64(reader["hasgettimes"].ToString());
					lastgettime = reader["lastgettime"].ToString();
					ret = 0;
				}
			}
			finally
			{
				if (null != reader)
				{
					reader.Close();
				}
			}
			return ret;
		}

		
		private static readonly KuaFuCopyDbMgr g_Instance = new KuaFuCopyDbMgr();

		
		public object Mutex = new object();

		
		public bool Initialized = false;

		
		private KFTeamCountControl _Control = null;

		
		private object _ControlMutex = new object();
	}
}
