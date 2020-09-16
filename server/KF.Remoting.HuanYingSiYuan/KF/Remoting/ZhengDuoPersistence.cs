using System;
using System.Collections.Generic;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class ZhengDuoPersistence
	{
		
		private ZhengDuoPersistence()
		{
		}

		
		public int DBWeekAndStepGet(int type)
		{
			int result = 0;
			int result2;
			try
			{
				object obj = DbHelperMySQL.GetSingle("select value from t_async where id = " + type);
				if (obj != null && obj != DBNull.Value)
				{
					int.TryParse(obj.ToString(), out result);
				}
				result2 = result;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result2 = -1;
			}
			return result2;
		}

		
		public bool DBWeekAndStepSet(int type, int value)
		{
			string sql = string.Format("REPLACE INTO t_async(id,value) \r\n                                        VALUES('{0}','{1}')", type, value);
			return this.ExecuteSqlNoQuery(sql) > 0;
		}

		
		public Dictionary<int, ZhengDuoRankData> DBRankList(int week)
		{
			Dictionary<int, ZhengDuoRankData> dic = new Dictionary<int, ZhengDuoRankData>();
			try
			{
				string strSql = string.Format("SELECT bhid,zoneid,bhname,bhLevel,zhanli,rank1,rank2,state,millisecond,serverid,lose,enemy\r\n                                                FROM t_zhengduo_rank\r\n                                                WHERE week='{0}'\r\n                                                order by rank2,rank1,millisecond", week);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					ZhengDuoRankData item = new ZhengDuoRankData();
					item.Bhid = Convert.ToInt32(sdr["bhid"]);
					item.ZoneId = Convert.ToInt32(sdr["zoneid"]);
					item.BhName = sdr["bhname"].ToString();
					item.BhLevel = Convert.ToInt32(sdr["bhLevel"]);
					item.ZhanLi = (long)Convert.ToInt32(sdr["zhanli"]);
					item.Rank1 = Convert.ToInt32(sdr["rank1"]);
					item.Rank2 = Convert.ToInt32(sdr["rank2"]);
					item.State = Convert.ToInt32(sdr["state"]);
					item.UsedMillisecond = Convert.ToInt32(sdr["millisecond"]);
					item.ServerID = Convert.ToInt32(sdr["serverid"]);
					item.Lose = Convert.ToInt32(sdr["lose"]);
					item.Enemy = Convert.ToInt32(sdr["enemy"]);
					item.Week = week;
					dic[item.Rank1] = item;
				}
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return null;
			}
			return dic;
		}

		
		public bool DBRankUpdata(ZhengDuoRankData data)
		{
			bool result;
			try
			{
				string sql = string.Format("REPLACE INTO t_zhengduo_rank(week,bhid,zoneid,bhname,bhLevel,zhanli,rank1,rank2,state,millisecond,serverid,lose,enemy) \r\n                                            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},{12})", new object[]
				{
					data.Week,
					data.Bhid,
					data.ZoneId,
					data.BhName,
					data.BhLevel,
					data.ZhanLi,
					data.Rank1,
					data.Rank2,
					data.State,
					data.UsedMillisecond,
					data.ServerID,
					data.Lose,
					data.Enemy
				});
				result = (this.ExecuteSqlNoQuery(sql) >= 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result = false;
			}
			return result;
		}

		
		public long CreateZhengDuoFuBen(int gametype, int serverId)
		{
			string sql = string.Format("INSERT INTO t_game(gametype,serverid,createtime) VALUES({0},{1},NOW());", gametype, serverId);
			return DbHelperMySQL.ExecuteSqlGetIncrement(sql, null);
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
				LogManager.WriteException(ex.ToString());
				return -1;
			}
			return i;
		}

		
		public static readonly ZhengDuoPersistence Instance = new ZhengDuoPersistence();
	}
}
