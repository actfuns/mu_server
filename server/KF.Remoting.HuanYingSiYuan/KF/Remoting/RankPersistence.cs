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
	// Token: 0x02000045 RID: 69
	public class RankPersistence
	{
		// Token: 0x060002F2 RID: 754 RVA: 0x00029C0B File Offset: 0x00027E0B
		private RankPersistence()
		{
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00029C20 File Offset: 0x00027E20
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

		// Token: 0x060002F4 RID: 756 RVA: 0x00029C68 File Offset: 0x00027E68
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

		// Token: 0x060002F5 RID: 757 RVA: 0x00029E20 File Offset: 0x00028020
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

		// Token: 0x060002F6 RID: 758 RVA: 0x00029ED8 File Offset: 0x000280D8
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

		// Token: 0x060002F7 RID: 759 RVA: 0x00029FF8 File Offset: 0x000281F8
		public bool DBRankDelMore(int rankType, int rankMax)
		{
			string sql = string.Format("delete from t_rank where rankType={0} and rank>{1}", rankType, rankMax);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0002A030 File Offset: 0x00028230
		public bool DBRankDelByType(int rankType)
		{
			string sql = string.Format("delete from t_rank where rankType={0}", rankType);
			int i = this.ExecuteSqlNoQuery(sql);
			return i > 0;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0002A060 File Offset: 0x00028260
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

		// Token: 0x040001B4 RID: 436
		public long DataVersion = 0L;

		// Token: 0x040001B5 RID: 437
		public static readonly RankPersistence Instance = new RankPersistence();
	}
}
