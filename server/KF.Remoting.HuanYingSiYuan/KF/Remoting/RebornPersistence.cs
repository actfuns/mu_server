using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class RebornPersistence
	{
		
		private RebornPersistence()
		{
		}

		
		public void AddDelayWriteSql(string sql)
		{
			lock (this.Mutex)
			{
				this.DelayWriteSqlQueue.Enqueue(sql);
			}
		}

		
		private void WriteDataToDb(string sql)
		{
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sql, null, true);
				DbHelperMySQL.ExecuteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(string.Format("sql: {0}\r\n{1}", sql, ex.ToString()));
			}
		}

		
		public void DelayWriteDataProc()
		{
			List<string> list = null;
			lock (this.Mutex)
			{
				if (this.DelayWriteSqlQueue.Count == 0)
				{
					return;
				}
				list = this.DelayWriteSqlQueue.ToList<string>();
				this.DelayWriteSqlQueue.Clear();
			}
			foreach (string sql in list)
			{
				this.WriteDataToDb(sql);
			}
		}

		
		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		
		public bool LoadDatabase()
		{
			bool result;
			if (!this.LoadRebornRoleData(this.RebornRoleDataDict))
			{
				result = false;
			}
			else
			{
				for (int rankType = 0; rankType <= 3; rankType++)
				{
					if (!this.LoadRebornRankInfo(rankType, this.RebornRankDict))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		
		private string FormatLoadRebornRankSql(int rankType)
		{
			int[] RankLimit = new int[]
			{
				100,
				10,
				10,
				10
			};
			string strSql = "";
			switch (rankType)
			{
			case 0:
				strSql = string.Format("SELECT rid a, lev b, ptid c FROM t_reborn_roles ORDER BY `lev` DESC, `ranktm_lev` ASC, rid DESC LIMIT {0};", RankLimit[rankType]);
				break;
			case 1:
				strSql = string.Format("SELECT rid a, rarity_last b, ptid c FROM t_reborn_roles WHERE `rarity_last`<>0 ORDER BY `rarity_last` DESC, `ranktm_rl` ASC, rid DESC LIMIT {0};", RankLimit[rankType]);
				break;
			case 2:
				strSql = string.Format("SELECT `rid` a, boss_last b, ptid c FROM t_reborn_roles WHERE `boss_last`<>0 ORDER BY `boss_last` DESC, `ranktm_bl` ASC, rid DESC LIMIT {0};", RankLimit[rankType]);
				break;
			case 3:
				strSql = string.Format("SELECT rid a, liansha_last b, ptid c FROM t_reborn_roles WHERE `liansha_last`<>0 ORDER BY `liansha_last` DESC, `ranktm_lsl` ASC, rid DESC LIMIT {0};", RankLimit[rankType]);
				break;
			default:
				return strSql;
			}
			return strSql;
		}

		
		public int GetRebornDayID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 49);
			return Convert.ToInt32(value);
		}

		
		public void SaveRebornDayID(int dayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 49, dayId));
		}

		
		public byte[] LoadRebornRoleData(int ptid, int rid)
		{
			byte[] result;
			try
			{
				object roledata = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_reborn_roles WHERE `ptid`={0} AND rid={1}", ptid, rid));
				if (null == roledata)
				{
					result = null;
				}
				else
				{
					result = (byte[])roledata;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		
		public bool LoadRebornRankInfo(int rankType, KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict)
		{
			bool result;
			if (null == RebornRankDict)
			{
				result = false;
			}
			else
			{
				List<KFRebornRankInfo> rankList = null;
				if (!RebornRankDict.V.TryGetValue(rankType, out rankList))
				{
					rankList = (RebornRankDict.V[rankType] = new List<KFRebornRankInfo>());
				}
				else
				{
					rankList.Clear();
				}
				try
				{
					string strSql = this.FormatLoadRebornRankSql(rankType);
					if (string.IsNullOrEmpty(strSql))
					{
						return false;
					}
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KFRebornRankInfo rankInfo = new KFRebornRankInfo();
						rankInfo.Key = Convert.ToInt32(sdr["a"]);
						rankInfo.Value = Convert.ToInt32(sdr["b"]);
						rankInfo.PtID = Convert.ToInt32(sdr["c"]);
						KuaFuData<KFRebornRoleData> kfRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(rankInfo.PtID, rankInfo.Key), out kfRoleData))
						{
							string worldRoleID = ConstData.FormatWorldRoleID(rankInfo.Key, rankInfo.PtID);
							KuaFuWorldRoleData worldRoleData = TSingleton<KuaFuWorldManager>.getInstance().LoadKuaFuWorldRoleData(rankInfo.Key, rankInfo.PtID, worldRoleID);
							if (null != worldRoleData)
							{
								int KFZoneID = ConstData.ConvertToKuaFuServerID(worldRoleData.ZoneID, worldRoleData.PTID);
								rankInfo.Param1 = KuaFuServerManager.FormatName(kfRoleData.V.RoleName, KFZoneID);
								rankInfo.Param2 = worldRoleData.Channel;
								rankInfo.UserID = worldRoleData.UserID;
								rankInfo.tagInfo = kfRoleData;
							}
						}
						rankList.Add(rankInfo);
					}
					if (null != RebornRankDict)
					{
						TimeUtil.AgeByNow(ref RebornRankDict.Age);
					}
					if (sdr != null)
					{
						sdr.Close();
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
					return false;
				}
				result = true;
			}
			return result;
		}

		
		private bool LoadRebornRoleData(Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict)
		{
			bool result;
			if (null == RebornRoleDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSql = string.Format("SELECT * FROM `t_reborn_roles`", new object[0]);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KuaFuData<KFRebornRoleData> myData = new KuaFuData<KFRebornRoleData>();
						myData.V.PtID = Convert.ToInt32(sdr["ptid"]);
						myData.V.RoleID = Convert.ToInt32(sdr["rid"]);
						myData.V.Lev = Convert.ToInt32(sdr["lev"]);
						myData.V.Rarity = Convert.ToInt32(sdr["rarity"]);
						myData.V.RarityLast = Convert.ToInt32(sdr["rarity_last"]);
						myData.V.Boss = Convert.ToInt32(sdr["boss"]);
						myData.V.BossLast = Convert.ToInt32(sdr["boss_last"]);
						myData.V.LianSha = Convert.ToInt32(sdr["liansha"]);
						myData.V.LianShaLast = Convert.ToInt32(sdr["liansha_last"]);
						myData.V.ParseBossAwardListString(Convert.ToString(sdr["boss_award"]), myData.V.BossAwardList);
						myData.V.RoleName = Convert.ToString(sdr["rname"]);
						string strRankTm = sdr["ranktm_lev"].ToString();
						if (!string.IsNullOrEmpty(strRankTm))
						{
							DateTime.TryParse(strRankTm, out myData.V.RankTmLev);
						}
						strRankTm = sdr["ranktm_r"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmR);
						strRankTm = sdr["ranktm_rl"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmRL);
						strRankTm = sdr["ranktm_b"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmB);
						strRankTm = sdr["ranktm_bl"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmBL);
						strRankTm = sdr["ranktm_ls"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmLS);
						strRankTm = sdr["ranktm_lsl"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmLSL);
						RebornRoleDataDict[new KeyValuePair<int, int>(myData.V.PtID, myData.V.RoleID)] = myData;
						myData.V.RoleData4Selector = (sdr["data1"] as byte[]);
						TimeUtil.AgeByNow(ref myData.Age);
					}
					if (sdr != null)
					{
						sdr.Close();
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public void UpdateRebornRoleDataBossAward(KFRebornRoleData roleData)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET boss_award='{2}' WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID, roleData.FormatBossAwardString(roleData.BossAwardList));
			this.AddDelayWriteSql(sql);
		}

		
		public void UpdateRebornRoleDataRoleName(KFRebornRoleData roleData)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET rname='{2}' WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID, roleData.RoleName);
			this.AddDelayWriteSql(sql);
		}

		
		public void UpdateRebornRoleData4Selector(KFRebornRoleData roleData)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET data1=@content WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("content", roleData.RoleData4Selector)
			});
		}

		
		public void UpdateRebornRoleData(KFRebornRoleData roleData, int chgMask, bool delay = true)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET lev={2}, rarity={3}, rarity_last={4}, boss={5}, boss_last={6}, liansha={7}, liansha_last={8} WHERE ptid={0} AND rid={1};", new object[]
			{
				roleData.PtID,
				roleData.RoleID,
				roleData.Lev,
				roleData.Rarity,
				roleData.RarityLast,
				roleData.Boss,
				roleData.BossLast,
				roleData.LianSha,
				roleData.LianShaLast
			});
			if ((chgMask & 1) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_lev=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 2) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_r=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 4) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_rl=ranktm_r WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 8) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_b=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 16) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_bl=ranktm_b WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 32) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_ls=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 64) > 0)
			{
				sql += string.Format("UPDATE t_reborn_roles SET ranktm_lsl=ranktm_ls WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if (delay)
			{
				this.AddDelayWriteSql(sql);
			}
			else
			{
				DbHelperMySQL.ExecuteSql(sql);
			}
		}

		
		public void InsertRebornRoleData(KFRebornRoleData roleData)
		{
			string sql = string.Format("INSERT INTO t_reborn_roles(ptid, rid, rname, lev, rarity, rarity_last, boss, boss_last, liansha, liansha_last, boss_award) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},'{10}');", new object[]
			{
				roleData.PtID,
				roleData.RoleID,
				roleData.RoleName,
				roleData.Lev,
				roleData.Rarity,
				roleData.RarityLast,
				roleData.Boss,
				roleData.BossLast,
				roleData.LianSha,
				roleData.LianShaLast,
				roleData.FormatBossAwardString(roleData.BossAwardList)
			});
			this.AddDelayWriteSql(sql);
		}

		
		public static readonly RebornPersistence Instance = new RebornPersistence();

		
		public object Mutex = new object();

		
		public Queue<string> DelayWriteSqlQueue = new Queue<string>();

		
		public Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>>();

		
		public KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict = new KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>();
	}
}
