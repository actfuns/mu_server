using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	public class AllyPersistence
	{
		
		private AllyPersistence()
		{
		}

		
		public void InitConfig()
		{
			try
			{
				this.DataVersion = TimeUtil.NowDateTime().Ticks;
				XElement xmlFile = ConfigHelper.Load("config.xml");
				Consts.AllyNumMax = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "AllyNumMax", "value", 5L);
				Consts.AllyRequestClearSecond = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "AllyRequestClearSecond", "value", 86400L);
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int i = 0;
			try
			{
				i = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return i;
		}

		
		public List<AllyLogData> DBAllyLogList(int unionID)
		{
			List<AllyLogData> list = new List<AllyLogData>();
			try
			{
				string strSql = string.Format("SELECT l.myUnionID,l.unionID,u.unionZoneID,u.unionName,l.logTime,l.logState\r\n                                                FROM t_ally_log l,t_ally_union u\r\n                                                WHERE l.unionID = u.unionID AND l.unionID='{0}'", unionID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					list.Add(new AllyLogData
					{
						MyUnionID = Convert.ToInt32(sdr["myUnionID"]),
						UnionID = Convert.ToInt32(sdr["unionID"]),
						LogTime = Convert.ToDateTime(sdr["logTime"]),
						LogState = Convert.ToInt32(sdr["logState"])
					});
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		
		public bool DBAllyLogDel(int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_log WHERE myUnionID='{0}'", unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public bool DBAllyLogAdd(AllyLogData logData)
		{
			string sql = string.Format("INSERT INTO t_ally_log(myUnionID, unionID, logTime,logState) VALUES('{0}','{1}','{2}','{3}')", new object[]
			{
				logData.MyUnionID,
				logData.UnionID,
				logData.LogTime,
				logData.LogState
			});
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public KFAllyData DBUnionDataGet(int unionID)
		{
			KFAllyData item = null;
			try
			{
				string strSql = string.Format("SELECT unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID\r\n                                                FROM t_ally_union\r\n                                                WHERE unionID='{0}'", unionID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					item = new KFAllyData();
					item.UnionID = Convert.ToInt32(sdr["unionID"]);
					item.UnionZoneID = Convert.ToInt32(sdr["unionZoneID"]);
					item.UnionName = sdr["unionName"].ToString();
					item.UnionLevel = Convert.ToInt32(sdr["unionLevel"]);
					item.UnionNum = Convert.ToInt32(sdr["unionNum"]);
					item.LeaderID = Convert.ToInt32(sdr["leaderID"]);
					item.LeaderZoneID = Convert.ToInt32(sdr["leaderZoneID"]);
					item.LeaderName = sdr["leaderName"].ToString();
					item.LogTime = Convert.ToDateTime(sdr["logTime"]);
					item.ServerID = Convert.ToInt32(sdr["serverID"]);
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return item;
		}

		
		public KFAllyData DBUnionDataGet(int unionZoneID, string unionName)
		{
			KFAllyData item = null;
			try
			{
				string strSql = string.Format("SELECT unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID\r\n                                                FROM t_ally_union\r\n                                                WHERE unionZoneID='{0}' and unionName='{1}'", unionZoneID, unionName);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					item = new KFAllyData();
					item.UnionID = Convert.ToInt32(sdr["unionID"]);
					item.UnionZoneID = Convert.ToInt32(sdr["unionZoneID"]);
					item.UnionName = sdr["unionName"].ToString();
					item.UnionLevel = Convert.ToInt32(sdr["unionLevel"]);
					item.UnionNum = Convert.ToInt32(sdr["unionNum"]);
					item.LeaderID = Convert.ToInt32(sdr["leaderID"]);
					item.LeaderZoneID = Convert.ToInt32(sdr["leaderZoneID"]);
					item.LeaderName = sdr["leaderName"].ToString();
					item.LogTime = Convert.ToDateTime(sdr["logTime"]);
					item.ServerID = Convert.ToInt32(sdr["serverID"]);
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return item;
		}

		
		public bool DBUnionDataUpdate(KFAllyData data)
		{
			string sql = string.Format("REPLACE INTO t_ally_union(unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID) \r\n                                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", new object[]
			{
				data.UnionID,
				data.UnionZoneID,
				data.UnionName,
				data.UnionLevel,
				data.UnionNum,
				data.LeaderID,
				data.LeaderZoneID,
				data.LeaderName,
				data.LogTime,
				data.ServerID
			});
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public bool DBUnionDataDel(int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_union WHERE unionID={0}", unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public List<int> DBAllyIDList(int unionID)
		{
			List<int> idList = new List<int>();
			try
			{
				string strSql = string.Format("SELECT DISTINCT(unionID2) uid from t_ally where unionID1='{0}' \r\n                                                UNION\r\n                                                SELECT DISTINCT(unionID1) uid from t_ally where unionID2='{0}' \r\n                                                ORDER BY uid", unionID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					idList.Add(Convert.ToInt32(sdr["uid"]));
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return idList;
		}

		
		public bool DBAllyAdd(int myUnionID, int unionID, DateTime logTime)
		{
			string sql = string.Format("INSERT INTO t_ally(unionID1, unionID2, logTime) VALUES('{0}','{1}','{2}')", myUnionID, unionID, logTime);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public bool DBAllyDel(int unionID, int targetID)
		{
			string sql = string.Format("DELETE FROM t_ally WHERE (unionID1='{0}' and unionID2='{1}') or(unionID1='{1}' and unionID2='{0}')", unionID, targetID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public List<KFAllyData> DBAllyRequestList(int unionID)
		{
			List<KFAllyData> list = new List<KFAllyData>();
			try
			{
				string strSql = string.Format("SELECT unionID,logTime,logState FROM t_ally_request WHERE myUnionID='{0}'", unionID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					list.Add(new KFAllyData
					{
						UnionID = Convert.ToInt32(sdr["unionID"]),
						LogTime = Convert.ToDateTime(sdr["logTime"]),
						LogState = Convert.ToInt32(sdr["logState"])
					});
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		
		public bool DBAllyRequestAdd(int myUnionID, int unionID, DateTime logTime, int logState)
		{
			string sql = string.Format("INSERT INTO t_ally_request(myUnionID, unionID, logTime,logState) VALUES('{0}','{1}','{2}','{3}')", new object[]
			{
				myUnionID,
				unionID,
				logTime,
				logState
			});
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public bool DBAllyRequestDel(int myUnionID, int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_request WHERE myUnionID='{0}' and unionID='{1}'", myUnionID, unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public List<KFAllyData> DBAllyAcceptList(int unionID)
		{
			List<KFAllyData> list = new List<KFAllyData>();
			try
			{
				string strSql = string.Format("SELECT myUnionID,logTime,logState FROM t_ally_request WHERE unionID='{0}'", unionID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					list.Add(new KFAllyData
					{
						UnionID = Convert.ToInt32(sdr["myUnionID"]),
						LogTime = Convert.ToDateTime(sdr["logTime"]),
						LogState = Convert.ToInt32(sdr["logState"])
					});
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		
		public static readonly AllyPersistence Instance = new AllyPersistence();

		
		public long DataVersion = 0L;

		
		public bool Initialized = false;
	}
}
