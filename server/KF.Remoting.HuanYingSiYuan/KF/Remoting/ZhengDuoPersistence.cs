using System;
using System.Collections.Generic;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	// Token: 0x02000079 RID: 121
	public class ZhengDuoPersistence
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x000504F0 File Offset: 0x0004E6F0
		private ZhengDuoPersistence()
		{
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000504FC File Offset: 0x0004E6FC
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

		// Token: 0x060005EB RID: 1515 RVA: 0x00050570 File Offset: 0x0004E770
		public bool DBWeekAndStepSet(int type, int value)
		{
			string sql = string.Format("REPLACE INTO t_async(id,value) \r\n                                        VALUES('{0}','{1}')", type, value);
			return this.ExecuteSqlNoQuery(sql) > 0;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000505A4 File Offset: 0x0004E7A4
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

		// Token: 0x060005ED RID: 1517 RVA: 0x00050764 File Offset: 0x0004E964
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

		// Token: 0x060005EE RID: 1518 RVA: 0x00050870 File Offset: 0x0004EA70
		public long CreateZhengDuoFuBen(int gametype, int serverId)
		{
			string sql = string.Format("INSERT INTO t_game(gametype,serverid,createtime) VALUES({0},{1},NOW());", gametype, serverId);
			return DbHelperMySQL.ExecuteSqlGetIncrement(sql, null);
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x000508A0 File Offset: 0x0004EAA0
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

		// Token: 0x0400033D RID: 829
		public static readonly ZhengDuoPersistence Instance = new ZhengDuoPersistence();
	}
}
