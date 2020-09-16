using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class CompPersistence
	{
		
		private CompPersistence()
		{
		}

		
		public bool LoadDatabase()
		{
			bool result;
			if (!this.LoadCompData(this.CompDataDict))
			{
				result = false;
			}
			else if (!this.LoadCompRoleData(this.CompRoleDataDict))
			{
				result = false;
			}
			else
			{
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					if (!this.LoadCompRankInfo(1, compLoop, this.CompRankJunXianDict, null))
					{
						return false;
					}
				}
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					if (!this.LoadCompRankInfo(2, compLoop, this.CompRankJunXianLastDict, null))
					{
						return false;
					}
				}
				if (!this.LoadCompRankInfo(3, 0, null, this.CompRankBossDamageList))
				{
					result = false;
				}
				else
				{
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						if (!this.LoadCompRankInfo(4, compLoop, this.CompRankBattleJiFenDict, null))
						{
							return false;
						}
					}
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						if (!this.LoadCompRankInfo(5, compLoop, this.CompRankMineJiFenDict, null))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
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

		
		private string FormatLoadCompRankSql(int rankType, int compType)
		{
			int[] RankLimit = new int[]
			{
				20,
				20,
				3,
				50
			};
			string strSql = "";
			switch (rankType)
			{
			case 1:
				strSql = string.Format("SELECT rid a, junxian b FROM t_comp_roles WHERE `type`={0} AND `junxian`<>0 ORDER BY `junxian` DESC, `ranktm_jx` ASC, rid DESC LIMIT {1};", compType, RankLimit[rankType - 1]);
				break;
			case 2:
				strSql = string.Format("SELECT rid a, junxian_last b FROM t_comp_roles WHERE `type_last`={0} AND `junxian_last`<>0 ORDER BY `junxian_last` DESC, `ranktm_jxl` ASC, rid DESC LIMIT {1};", compType, RankLimit[rankType - 1]);
				break;
			case 3:
				strSql = string.Format("SELECT `type` a, bossdamage b FROM t_comp ORDER BY `type` ASC LIMIT {0};", RankLimit[rankType - 1]);
				break;
			case 4:
				strSql = string.Format("SELECT rid a, battlejifen b FROM t_comp_roles WHERE `type_battle`={0} AND `battlejifen`<>0 ORDER BY `battlejifen` DESC, `ranktm_bjf` ASC, rid DESC;", compType);
				break;
			case 5:
				strSql = string.Format("SELECT rid a, minejifen b FROM t_comp_roles WHERE `type_mine`={0} AND `minejifen`<>0 ORDER BY `minejifen` DESC, `ranktm_mjf` ASC, rid DESC;", compType);
				break;
			default:
				return strSql;
			}
			return strSql;
		}

		
		public int GetCompDayID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 45);
			return Convert.ToInt32(value);
		}

		
		public int GetCompWeekDayID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 46);
			return Convert.ToInt32(value);
		}

		
		public int GetCompBattleWeekDayID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 47);
			return Convert.ToInt32(value);
		}

		
		public int GetCompMineWeekDayID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 48);
			return Convert.ToInt32(value);
		}

		
		public void SaveCompDayID(int dayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 45, dayId));
		}

		
		public void SaveCompWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 46, weekDayId));
		}

		
		public void SaveCompBattleWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 47, weekDayId));
		}

		
		public void SaveCompMineWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 48, weekDayId));
		}

		
		public bool LoadCompRankInfo(int rankType, int compType, KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankDict, KuaFuData<List<KFCompRankInfo>> CompRankList)
		{
			bool result;
			if (CompRankDict == null && null == CompRankList)
			{
				result = false;
			}
			else
			{
				List<KFCompRankInfo> rankList = null;
				if (null != CompRankDict)
				{
					if (!CompRankDict.V.TryGetValue(compType, out rankList))
					{
						rankList = (CompRankDict.V[compType] = new List<KFCompRankInfo>());
					}
					else
					{
						rankList.Clear();
					}
				}
				else
				{
					rankList = CompRankList.V;
					rankList.Clear();
				}
				try
				{
					string strSql = this.FormatLoadCompRankSql(rankType, compType);
					if (string.IsNullOrEmpty(strSql))
					{
						return false;
					}
					if (rankType == 3)
					{
						for (int i = 1; i <= 3; i++)
						{
							rankList.Add(new KFCompRankInfo());
						}
					}
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KFCompRankInfo rankInfo = new KFCompRankInfo();
						rankInfo.Key = Convert.ToInt32(sdr["a"]);
						rankInfo.Value = Convert.ToInt32(sdr["b"]);
						if (rankType == 3)
						{
							rankList[rankInfo.Key - 1] = rankInfo;
						}
						else
						{
							KuaFuData<KFCompRoleData> kfRoleData = null;
							if (this.CompRoleDataDict.TryGetValue(rankInfo.Key, out kfRoleData))
							{
								rankInfo.Param1 = KuaFuServerManager.FormatName(kfRoleData.V.RoleName, kfRoleData.V.ZoneID);
								rankInfo.tagInfo = kfRoleData;
							}
							rankList.Add(rankInfo);
						}
					}
					if (null != CompRankDict)
					{
						TimeUtil.AgeByNow(ref CompRankDict.Age);
					}
					if (null != CompRankList)
					{
						TimeUtil.AgeByNow(ref CompRankList.Age);
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

		
		private bool LoadCompData(KuaFuData<Dictionary<int, KFCompData>> CompDataDict)
		{
			bool result;
			if (null == CompDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSql = string.Format("SELECT * FROM `t_comp`", new object[0]);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KFCompData myCompData = new KFCompData();
						myCompData.InitPlunderResList();
						myCompData.CompType = Convert.ToInt32(sdr["type"]);
						myCompData.BoomValue = Convert.ToInt32(sdr["boomval"]);
						myCompData.YestdBoomValue = Convert.ToInt32(sdr["boomval_yestd"]);
						myCompData.EnemyCompType = Convert.ToInt32(sdr["enemytype"]);
						myCompData.EnemyCompTypeSet = Convert.ToInt32(sdr["enemyset"]);
						myCompData.Bulletin = sdr["bulletin"].ToString();
						myCompData.Crystal = Convert.ToInt32(sdr["crystal"]);
						myCompData.BossDamageTop = Convert.ToInt32(sdr["bossdamage"]);
						myCompData.Boss = Convert.ToInt32(sdr["boss"]);
						myCompData.YestdCrystal = Convert.ToInt32(sdr["crystal_yestd"]);
						myCompData.YestdBoss = Convert.ToInt32(sdr["boss_yestd"]);
						myCompData.ParsePlunderResListString(sdr["plunderres"].ToString(), myCompData.PlunderResList);
						myCompData.ParsePlunderResListString(sdr["plunderres_yestd"].ToString(), myCompData.YestdPlunderResList);
						myCompData.ParseStrongholdDictString(sdr["stronghold"].ToString(), myCompData.StrongholdDict);
						myCompData.BossKillCompType = Convert.ToInt32(sdr["bosskilltype"]);
						myCompData.YestdBossKillCompType = Convert.ToInt32(sdr["bosskilltype_yestd"]);
						myCompData.MineRes = Convert.ToInt32(sdr["mine"]);
						myCompData.MineRank = Convert.ToInt32(sdr["mine_rank"]);
						CompDataDict.V[myCompData.CompType] = myCompData;
					}
					TimeUtil.AgeByNow(ref CompDataDict.Age);
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

		
		private bool LoadCompRoleData(Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict)
		{
			bool result;
			if (null == CompRoleDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSql = string.Format("SELECT * FROM `t_comp_roles`", new object[0]);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KuaFuData<KFCompRoleData> myData = new KuaFuData<KFCompRoleData>();
						myData.V.RoleID = Convert.ToInt32(sdr["rid"]);
						myData.V.ZoneID = Convert.ToInt32(sdr["zoneid"]);
						myData.V.CompType = Convert.ToInt32(sdr["type"]);
						myData.V.CompTypeLast = Convert.ToInt32(sdr["type_last"]);
						myData.V.JunXian = Convert.ToInt32(sdr["junxian"]);
						myData.V.JunXianLast = Convert.ToInt32(sdr["junxian_last"]);
						myData.V.RoleName = Convert.ToString(sdr["rname"]);
						myData.V.BattleJiFen = Convert.ToInt32(sdr["battlejifen"]);
						myData.V.CompTypeBattle = Convert.ToInt32(sdr["type_battle"]);
						myData.V.CompTypeMine = Convert.ToInt32(sdr["type_mine"]);
						myData.V.MineJiFen = Convert.ToInt32(sdr["minejifen"]);
						string strRankTm = sdr["ranktm_bjf"].ToString();
						if (!string.IsNullOrEmpty(strRankTm))
						{
							DateTime.TryParse(strRankTm, out myData.V.RankTmBJF);
						}
						strRankTm = sdr["ranktm_mjf"].ToString();
						DateTime.TryParse(strRankTm, out myData.V.RankTmMJF);
						CompRoleDataDict[myData.V.RoleID] = myData;
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

		
		public byte[] LoadCompRoleData4Selector(int rid)
		{
			byte[] result;
			try
			{
				object roledata = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_comp_roles WHERE rid={0}", rid));
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

		
		public void SaveCompData(KFCompData compData, bool delay = true)
		{
			string sql = string.Format("INSERT INTO t_comp(`type`, boomval, enemytype, enemyset, `bulletin`, crystal, boss, crystal_yestd, boss_yestd, plunderres, plunderres_yestd, bosskilltype, bosskilltype_yestd, bossdamage, boomval_yestd, stronghold, mine, mine_rank) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},{8},'{9}','{10}',{11},{12},{13},{14},'{15}',{16},{17}) ON DUPLICATE KEY UPDATE boomval={1}, enemytype={2}, enemyset={3}, bulletin='{4}', crystal={5}, boss={6}, crystal_yestd={7}, boss_yestd={8},  plunderres='{9}', plunderres_yestd='{10}', bosskilltype={11}, bosskilltype_yestd={12}, bossdamage={13}, boomval_yestd={14}, stronghold='{15}', mine={16}, mine_rank={17};", new object[]
			{
				compData.CompType,
				compData.BoomValue,
				compData.EnemyCompType,
				compData.EnemyCompTypeSet,
				compData.Bulletin,
				compData.Crystal,
				compData.Boss,
				compData.YestdCrystal,
				compData.YestdBoss,
				compData.FormatPlunderResListString(compData.PlunderResList),
				compData.FormatPlunderResListString(compData.YestdPlunderResList),
				compData.BossKillCompType,
				compData.YestdBossKillCompType,
				compData.BossDamageTop,
				compData.YestdBoomValue,
				compData.FormatStrongholdDictString(compData.StrongholdDict),
				compData.MineRes,
				compData.MineRank
			});
			if (delay)
			{
				this.AddDelayWriteSql(sql);
			}
			else
			{
				this.ExecuteSqlNoQuery(sql);
			}
		}

		
		public void SaveCompRoleData(KFCompRoleData roleData, bool chgJX = false, bool chgJXL = false, bool chgJiFen = false, bool chgMine = false)
		{
			if (null != roleData.RoleData4Selector)
			{
				string sql = string.Format("INSERT INTO t_comp_roles(rid, `type`, rname, zoneid, junxian, junxian_last, type_last, battlejifen, type_battle, minejifen, type_mine, data1) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10},@content) ON DUPLICATE KEY UPDATE `type`={1}, rname='{2}', junxian={4}, junxian_last={5}, type_last={6}, battlejifen={7}, type_battle={8}, minejifen={9}, type_mine={10}, data1=@content;", new object[]
				{
					roleData.RoleID,
					roleData.CompType,
					roleData.RoleName,
					roleData.ZoneID,
					roleData.JunXian,
					roleData.JunXianLast,
					roleData.CompTypeLast,
					roleData.BattleJiFen,
					roleData.CompTypeBattle,
					roleData.MineJiFen,
					roleData.CompTypeMine
				});
				DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", roleData.RoleData4Selector)
				});
			}
			else
			{
				string sql = string.Format("INSERT INTO t_comp_roles(rid, `type`, rname, zoneid, junxian, junxian_last, type_last, battlejifen, type_battle, minejifen, type_mine) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10}) ON DUPLICATE KEY UPDATE `type`={1}, rname='{2}', junxian={4}, junxian_last={5}, type_last={6}, battlejifen={7}, type_battle={8}, minejifen={9}, type_mine={10};", new object[]
				{
					roleData.RoleID,
					roleData.CompType,
					roleData.RoleName,
					roleData.ZoneID,
					roleData.JunXian,
					roleData.JunXianLast,
					roleData.CompTypeLast,
					roleData.BattleJiFen,
					roleData.CompTypeBattle,
					roleData.MineJiFen,
					roleData.CompTypeMine
				});
				this.AddDelayWriteSql(sql);
			}
			if (null != roleData.RoleData4Selector)
			{
				if (chgJX)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_jx=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(sql);
				}
				if (chgJXL)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_jxl=ranktm_jx WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(sql);
				}
				if (chgJiFen)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_bjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(sql);
				}
				if (chgMine)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_mjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(sql);
				}
			}
			else
			{
				if (chgJX)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_jx=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(sql);
				}
				if (chgJXL)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_jxl=ranktm_jx WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(sql);
				}
				if (chgJiFen)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_bjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(sql);
				}
				if (chgMine)
				{
					string sql = string.Format("UPDATE t_comp_roles SET ranktm_mjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(sql);
				}
			}
		}

		
		public static readonly CompPersistence Instance = new CompPersistence();

		
		public object Mutex = new object();

		
		public KuaFuData<Dictionary<int, KFCompData>> CompDataDict = new KuaFuData<Dictionary<int, KFCompData>>();

		
		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		
		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianLastDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		
		public KuaFuData<List<KFCompRankInfo>> CompRankBossDamageList = new KuaFuData<List<KFCompRankInfo>>();

		
		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankBattleJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		
		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankMineJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		
		public Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict = new Dictionary<int, KuaFuData<KFCompRoleData>>();

		
		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
