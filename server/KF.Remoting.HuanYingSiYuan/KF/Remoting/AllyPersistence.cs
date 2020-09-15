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
	// Token: 0x02000087 RID: 135
	public class AllyPersistence
	{
		// Token: 0x060006EB RID: 1771 RVA: 0x0005BB11 File Offset: 0x00059D11
		private AllyPersistence()
		{
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0005BB2C File Offset: 0x00059D2C
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

		// Token: 0x060006ED RID: 1773 RVA: 0x0005BBD0 File Offset: 0x00059DD0
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

		// Token: 0x060006EE RID: 1774 RVA: 0x0005BC14 File Offset: 0x00059E14
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

		// Token: 0x060006EF RID: 1775 RVA: 0x0005BD00 File Offset: 0x00059F00
		public bool DBAllyLogDel(int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_log WHERE myUnionID='{0}'", unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0005BD30 File Offset: 0x00059F30
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

		// Token: 0x060006F1 RID: 1777 RVA: 0x0005BD9C File Offset: 0x00059F9C
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

		// Token: 0x060006F2 RID: 1778 RVA: 0x0005BF10 File Offset: 0x0005A110
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

		// Token: 0x060006F3 RID: 1779 RVA: 0x0005C088 File Offset: 0x0005A288
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

		// Token: 0x060006F4 RID: 1780 RVA: 0x0005C140 File Offset: 0x0005A340
		public bool DBUnionDataDel(int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_union WHERE unionID={0}", unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0005C170 File Offset: 0x0005A370
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

		// Token: 0x060006F6 RID: 1782 RVA: 0x0005C20C File Offset: 0x0005A40C
		public bool DBAllyAdd(int myUnionID, int unionID, DateTime logTime)
		{
			string sql = string.Format("INSERT INTO t_ally(unionID1, unionID2, logTime) VALUES('{0}','{1}','{2}')", myUnionID, unionID, logTime);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0005C248 File Offset: 0x0005A448
		public bool DBAllyDel(int unionID, int targetID)
		{
			string sql = string.Format("DELETE FROM t_ally WHERE (unionID1='{0}' and unionID2='{1}') or(unionID1='{1}' and unionID2='{0}')", unionID, targetID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0005C280 File Offset: 0x0005A480
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

		// Token: 0x060006F9 RID: 1785 RVA: 0x0005C358 File Offset: 0x0005A558
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

		// Token: 0x060006FA RID: 1786 RVA: 0x0005C3B0 File Offset: 0x0005A5B0
		public bool DBAllyRequestDel(int myUnionID, int unionID)
		{
			string sql = string.Format("DELETE FROM t_ally_request WHERE myUnionID='{0}' and unionID='{1}'", myUnionID, unionID);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0005C3E8 File Offset: 0x0005A5E8
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

		// Token: 0x040003BF RID: 959
		public static readonly AllyPersistence Instance = new AllyPersistence();

		// Token: 0x040003C0 RID: 960
		public long DataVersion = 0L;

		// Token: 0x040003C1 RID: 961
		public bool Initialized = false;
	}
}
