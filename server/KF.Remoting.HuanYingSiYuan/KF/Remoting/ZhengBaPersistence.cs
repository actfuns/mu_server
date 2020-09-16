using System;
using System.Collections.Generic;
using GameServer.Logic;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	
	public class ZhengBaPersistence
	{
		
		private ZhengBaPersistence()
		{
		}

		
		public ZhengBaSyncData LoadZhengBaSyncData(DateTime now, int selectRoleIfNewCreate, long dayBeginTicks)
		{
			ZhengBaSyncData syncData = new ZhengBaSyncData();
			syncData.Month = ZhengBaUtils.MakeMonth(now);
			syncData.IsThisMonthInActivity = this.CheckThisMonthInActivity(now, dayBeginTicks);
			bool bMonthFirst;
			if (syncData.IsThisMonthInActivity)
			{
				bMonthFirst = this.CheckBuildZhengBaRank(selectRoleIfNewCreate, syncData.Month);
				syncData.RoleList = this.LoadZhengBaRankData(syncData.Month);
				syncData.SupportList = this.LoadZhengBaSupportData(syncData.Month);
			}
			else
			{
				bMonthFirst = false;
				syncData.RoleList = new List<ZhengBaRoleInfoData>();
				syncData.SupportList = new List<ZhengBaSupportAnalysisData>();
			}
			syncData.LastKingData = this.LoadZhengBaKingData(ref syncData.LastKingModTime);
			syncData.RoleModTime = now;
			syncData.SupportModTime = now;
			if (bMonthFirst && this.MonthRankFirstCreate != null)
			{
				this.MonthRankFirstCreate(selectRoleIfNewCreate);
			}
			return syncData;
		}

		
		public bool SaveSupportLog(ZhengBaSupportLogData data)
		{
			bool result;
			if (data == null)
			{
				result = false;
			}
			else
			{
				try
				{
					string sql = string.Format("INSERT INTO t_zhengba_support_log(month,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,`time`,rank_of_day,from_serverid) VALUES({0},{1},{2},'{3}',{4},{5},{6},'{7}',{8},{9});", new object[]
					{
						ZhengBaUtils.MakeMonth(data.Time),
						data.FromRoleId,
						data.FromZoneId,
						data.FromRolename,
						data.SupportType,
						data.ToUnionGroup,
						data.ToGroup,
						data.Time.ToString("yyyy-MM-dd HH:mm:ss"),
						data.RankOfDay,
						data.FromServerId
					});
					if (DbHelperMySQL.ExecuteSql(sql) <= 0)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, "SaveSupportLog failed!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public bool SavePkLog(ZhengBaPkLogData log)
		{
			bool result;
			if (log == null)
			{
				result = false;
			}
			else
			{
				try
				{
					string sql = string.Format("INSERT INTO t_zhengba_pk_log(month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9},{10},{11},'{12}','{13}');", new object[]
					{
						log.Month,
						log.Day,
						log.RoleID1,
						log.ZoneID1,
						log.RoleName1,
						log.IsMirror1 ? 1 : 0,
						log.RoleID2,
						log.ZoneID2,
						log.RoleName2,
						log.IsMirror2 ? 1 : 0,
						log.PkResult,
						log.UpGrade ? 1 : 0,
						log.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
						log.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
					});
					if (DbHelperMySQL.ExecuteSql(sql) <= 0)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, "SavePkLog failed!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public bool UpdateRole(int month, int rid, int grade, int state, int group)
		{
			try
			{
				string sql = string.Format("UPDATE t_zhengba_roles SET grade={0},`group`={1},state={2} WHERE month={3} AND rid={4}", new object[]
				{
					grade,
					group,
					state,
					month,
					rid
				});
				if (DbHelperMySQL.ExecuteSql(sql) <= 0)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "UpdateRole failed!", ex, true);
				return false;
			}
			return true;
		}

		
		private bool CheckThisMonthInActivity(DateTime now, long dayBeginTicks)
		{
			bool result;
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot0))
				{
					result = false;
				}
				else
				{
					DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, 201111));
					int oldMonth = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 30);
					if (oldMonth == 201111)
					{
						if (now.Day > ZhengBaConsts.StartMonthDay)
						{
							result = false;
						}
						else if (now.Day < ZhengBaConsts.StartMonthDay)
						{
							result = true;
						}
						else
						{
							result = (now.TimeOfDay.Ticks < dayBeginTicks);
						}
					}
					else
					{
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			return result;
		}

		
		private bool CheckBuildZhengBaRank(int selectRoleIfNewCreate, int nowMonth)
		{
			bool bMonthFirst = false;
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, 201111));
				int oldMonth = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 30);
				object ageObj_tiantiMonth = DbHelperMySQL.GetSingle("select value from t_async where id = " + 4);
				if (oldMonth != nowMonth && ageObj_tiantiMonth != null && ZhengBaUtils.MakeMonth(DataHelper2.GetRealDate((int)ageObj_tiantiMonth)) == nowMonth)
				{
					string strSql = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_month_paihang ORDER BY duanweirank ASC LIMIT {0};", selectRoleIfNewCreate);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						ZhengBaRoleInfoData roleData = new ZhengBaRoleInfoData();
						roleData.RoleId = Convert.ToInt32(sdr["rid"]);
						roleData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
						roleData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
						roleData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
						roleData.DuanWeiRank = Convert.ToInt32(sdr["duanweirank"]);
						roleData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
						roleData.RoleName = sdr["rname"].ToString();
						if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
						{
							roleData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
						}
						if (!sdr.IsDBNull(sdr.GetOrdinal("data2")))
						{
							roleData.PlayerJingJiMirrorData = (byte[])sdr["data2"];
						}
						if (string.IsNullOrEmpty(roleData.RoleName) && roleData.TianTiPaiHangRoleData != null)
						{
							TianTiPaiHangRoleData_OnlyName onlyName = DataHelper2.BytesToObject<TianTiPaiHangRoleData_OnlyName>(roleData.TianTiPaiHangRoleData, 0, roleData.TianTiPaiHangRoleData.Length);
							if (onlyName != null)
							{
								roleData.RoleName = onlyName.RoleName;
							}
						}
						string repSql = string.Format("REPLACE INTO t_zhengba_roles(`month`,rid,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,`grade`,`group`,state,rname,data1,data2) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},'{10}',@content,@mirror)", new object[]
						{
							nowMonth,
							roleData.RoleId,
							roleData.ZoneId,
							roleData.DuanWeiId,
							roleData.DuanWeiJiFen,
							roleData.DuanWeiRank,
							roleData.ZhanLi,
							100,
							0,
							0,
							roleData.RoleName
						});
						DbHelperMySQL.ExecuteSqlInsertImg(repSql, new List<Tuple<string, byte[]>>
						{
							new Tuple<string, byte[]>("content", roleData.TianTiPaiHangRoleData),
							new Tuple<string, byte[]>("mirror", roleData.PlayerJingJiMirrorData)
						});
					}
					if (sdr != null)
					{
						sdr.Close();
					}
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, nowMonth));
					bMonthFirst = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return bMonthFirst;
		}

		
		private List<ZhengBaRoleInfoData> LoadZhengBaRankData(int nowMonth)
		{
			List<ZhengBaRoleInfoData> roleList = new List<ZhengBaRoleInfoData>();
			try
			{
				string strSql = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,grade,`group`,state,data1,data2 FROM t_zhengba_roles where `month`={0} ORDER BY duanweirank ASC;", nowMonth);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					ZhengBaRoleInfoData roleData = new ZhengBaRoleInfoData();
					roleData.RoleId = Convert.ToInt32(sdr["rid"]);
					roleData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
					roleData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
					roleData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
					roleData.DuanWeiRank = Convert.ToInt32(sdr["duanweirank"]);
					roleData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
					if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
					{
						roleData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
					}
					if (!sdr.IsDBNull(sdr.GetOrdinal("data2")))
					{
						roleData.PlayerJingJiMirrorData = (byte[])sdr["data2"];
					}
					roleData.Grade = Convert.ToInt32(sdr["grade"]);
					roleData.Group = Convert.ToInt32(sdr["group"]);
					roleData.State = Convert.ToInt32(sdr["state"]);
					roleData.RoleName = sdr["rname"].ToString();
					roleList.Add(roleData);
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
			return roleList;
		}

		
		private ZhengBaRoleInfoData LoadZhengBaKingData(ref int month)
		{
			try
			{
				ZhengBaRoleInfoData kingRoleData = null;
				string strSql = string.Format("SELECT * FROM t_zhengba_roles WHERE grade={0} ORDER BY `month` DESC LIMIT 1;", 1);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				if (sdr != null && sdr.Read())
				{
					ZhengBaRoleInfoData roleData = new ZhengBaRoleInfoData();
					roleData.RoleId = Convert.ToInt32(sdr["rid"]);
					roleData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
					roleData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
					roleData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
					roleData.DuanWeiRank = Convert.ToInt32(sdr["duanweirank"]);
					roleData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
					if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
					{
						roleData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
					}
					if (!sdr.IsDBNull(sdr.GetOrdinal("data2")))
					{
						roleData.PlayerJingJiMirrorData = (byte[])sdr["data2"];
					}
					roleData.Grade = Convert.ToInt32(sdr["grade"]);
					roleData.Group = Convert.ToInt32(sdr["group"]);
					roleData.State = Convert.ToInt32(sdr["state"]);
					roleData.RoleName = sdr["rname"].ToString();
					month = Convert.ToInt32(sdr["month"]);
					kingRoleData = roleData;
				}
				if (sdr != null)
				{
					sdr.Close();
				}
				return kingRoleData;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		private List<ZhengBaSupportAnalysisData> LoadZhengBaSupportData(int nowMonth)
		{
			List<ZhengBaSupportAnalysisData> supportList = new List<ZhengBaSupportAnalysisData>();
			try
			{
				string strSql = string.Format("SELECT support_type,rank_of_day,to_union_group,to_group FROM t_zhengba_support_log where `month`={0}", nowMonth);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					int supportType = Convert.ToInt32(sdr["support_type"]);
					int toUnionGroup = Convert.ToInt32(sdr["to_union_group"]);
					int toGroup = Convert.ToInt32(sdr["to_group"]);
					int rankOfDay = Convert.ToInt32(sdr["rank_of_day"]);
					ZhengBaSupportAnalysisData support;
					if ((support = supportList.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == toUnionGroup && _s.Group == toGroup)) == null)
					{
						support = new ZhengBaSupportAnalysisData
						{
							UnionGroup = toUnionGroup,
							Group = toGroup,
							RankOfDay = rankOfDay
						};
						supportList.Add(support);
					}
					if (supportType == 1)
					{
						support.TotalSupport++;
					}
					else if (supportType == 2)
					{
						support.TotalOppose++;
					}
					else if (supportType == 3)
					{
						support.TotalYaZhu++;
					}
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
			return supportList;
		}

		
		public static readonly ZhengBaPersistence Instance = new ZhengBaPersistence();

		
		public ZhengBaPersistence.FirstCreateDbRank MonthRankFirstCreate = null;

		
		
		public delegate void FirstCreateDbRank(int selectRoleIfNewCreate);
	}
}
