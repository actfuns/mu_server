using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class RankPersistence
	{
		
		private RankPersistence()
		{
		}

		
		public void InitConfig()
		{
			try
			{
				this.DataVersion = TimeUtil.NowDateTime().Ticks;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public List<KFRankData> DBRankLoad(int rankType, int max)
		{
			MySqlDataReader sdr = null;
			List<KFRankData> result = new List<KFRankData>();
			try
			{
				string sql = string.Format("select rankType,rank,zoneID,roleID,roleName,grade,rankData,rankTime,serverID from t_rank where rankType='{0}' \r\n                            ORDER BY grade DESC, rankTime ASC, roleid ASC limit {1} ", rankType, max);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				while (sdr != null && sdr.Read())
				{
					KFRankData data = new KFRankData();
					data.RankType = Convert.ToInt32(sdr["rankType"]);
					data.Rank = Convert.ToInt32(sdr["rank"]);
					data.ZoneID = Convert.ToInt32(sdr["zoneID"]);
					data.RoleID = Convert.ToInt32(sdr["roleID"]);
					data.RoleName = sdr["roleName"].ToString();
					data.Grade = Convert.ToInt32(sdr["grade"]);
					if (!sdr.IsDBNull(sdr.GetOrdinal("rankData")))
					{
						data.RoleData = (byte[])sdr["rankData"];
					}
					data.RankTime = Convert.ToDateTime(sdr["rankTime"]);
					data.ServerID = Convert.ToInt32(sdr["serverID"]);
					data.RankOld = data.Rank;
					result.Add(data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			return result;
		}

		
		public bool DBRankDataUpdate(KFRankData data)
		{
			string sql = string.Format("REPLACE INTO t_rank(rankType,rank,zoneID,roleID,roleName,grade,rankTime,serverID,rankData) \r\n                                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',@roleData)", new object[]
			{
				data.RankType,
				data.Rank,
				data.ZoneID,
				data.RoleID,
				data.RoleName,
				data.Grade,
				data.RankTime,
				data.ServerID
			});
			DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("roleData", data.RoleData)
			});
			return true;
		}

		
		public void DBRankUpdate(List<KFRankData> list)
		{
			int index = 0;
			StringBuilder sb = new StringBuilder();
			foreach (KFRankData data in list)
			{
				if (data.Rank == data.RankOld)
				{
					index++;
				}
				else
				{
					sb.AppendFormat("UPDATE t_rank SET rank={0} WHERE rankType={1} AND roleID={2};", data.Rank, data.RankType, data.RoleID);
					sb.AppendLine();
					if ((index % 50 == 0 || index == list.Count - 1) && sb.Length > 0)
					{
						string sql = sb.ToString();
						sb.Clear();
						this.ExecuteSqlNoQuery(sql);
					}
					index++;
				}
			}
			if (sb.Length > 0)
			{
				string sql = sb.ToString();
				sb.Clear();
				this.ExecuteSqlNoQuery(sql);
			}
		}

		
		public bool DBRankDelMore(int rankType, int rankMax)
		{
			string sql = string.Format("delete from t_rank where rankType={0} and rank>{1}", rankType, rankMax);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		
		public bool DBRankDelByType(int rankType)
		{
			string sql = string.Format("delete from t_rank where rankType={0}", rankType);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
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

		
		public long DataVersion = 0L;

		
		public static readonly RankPersistence Instance = new RankPersistence();
	}
}
