using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Remoting;
using Server.Tools;

namespace KF.Remoting
{
	
	public class ZhanDuiZhengBaPersistence
	{
		
		private ZhanDuiZhengBaPersistence()
		{
		}

		
		public ZhanDuiZhengBaSyncData LoadZhengBaSyncData(DateTime now, int selectRoleIfNewCreate)
		{
			ZhanDuiZhengBaSyncData syncData = new ZhanDuiZhengBaSyncData();
			syncData.Month = ZhengBaUtils.MakeMonth(now);
			syncData.IsThisMonthInActivity = false;
			List<ZhanDuiZhengBaZhanDuiData> rankList = new List<ZhanDuiZhengBaZhanDuiData>();
			List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = new List<ZhanDuiZhengBaZhanDuiData>();
			List<ZhanDuiZhengBaPkLogData> pkLogList = new List<ZhanDuiZhengBaPkLogData>();
			if (KuaFuServerManager.IsGongNengOpened(113))
			{
				TianTi5v5RankData rankData = TianTi5v5Service.ZhanDuiGetRankingData(DateTime.MinValue);
				bool bMonthFirst = this.CheckZhengBaRank(selectRoleIfNewCreate, syncData.Month);
				if (bMonthFirst)
				{
					syncData.IsThisMonthInActivity = true;
				}
				else if (syncData.Month == ZhengBaUtils.MakeMonth(rankData.ModifyTime) && rankData.MonthPaiHangList.Count > ZhanDuiZhengBaConsts.MinZhanDuiNum)
				{
					syncData.IsThisMonthInActivity = true;
					int[] groupArray = MathEx.MatchGroupBinary(64);
					int i = 0;
					while (i < 64 && i < rankData.MonthPaiHangList.Count)
					{
						int group = Array.IndexOf<int>(groupArray, i + 1) + 1;
						TianTi5v5ZhanDuiData data = rankData.MonthPaiHangList[i];
						ZhanDuiZhengBaZhanDuiData zhandui = new ZhanDuiZhengBaZhanDuiData
						{
							ZhanDuiName = data.ZhanDuiName,
							ZhanDuiID = data.ZhanDuiID,
							ZhanLi = (long)((int)data.ZhanDouLi),
							DuanWeiId = data.DuanWeiId,
							DuanWeiRank = data.DuanWeiRank,
							ZoneId = data.ZoneID,
							MemberList = new List<RoleOccuNameZhanLi>(),
							Group = group
						};
						foreach (TianTi5v5ZhanDuiRoleData a in data.teamerList)
						{
							if (a.RoleID == data.LeaderRoleID)
							{
								zhandui.MemberList.Insert(0, new RoleOccuNameZhanLi
								{
									RoleName = a.RoleName,
									Occupation = a.RoleOcc,
									ZhanLi = a.ZhanLi
								});
							}
							else
							{
								zhandui.MemberList.Add(new RoleOccuNameZhanLi
								{
									RoleName = a.RoleName,
									Occupation = a.RoleOcc,
									ZhanLi = a.ZhanLi
								});
							}
						}
						rankList.Add(zhandui);
						i++;
					}
					if (!this.BuildZhengBaRank(syncData.Month, rankList))
					{
						LogManager.WriteLog(LogTypes.Fatal, "生成并写入战队争霸64强数据失败!", null, true);
						syncData.IsThisMonthInActivity = false;
					}
				}
				if (syncData.IsThisMonthInActivity)
				{
					zhanDuiList = this.LoadZhengBaRankData(syncData.Month);
					pkLogList = this.LoadPkLogList(syncData.Month);
				}
			}
			syncData.ZhanDuiList = zhanDuiList;
			syncData.PKLogList = pkLogList;
			syncData.RoleModTime = now;
			return syncData;
		}

		
		public int GetLastTopZhanDui(int month)
		{
			try
			{
				string strSql = string.Format("SELECT zhanduiid FROM t_zhandui_zhengba where `month`={0} and grade={1};", month, 1);
				return (int)DbHelperMySQL.GetSingleLong(strSql);
			}
			catch (Exception ex)
			{
			}
			return 0;
		}

		
		private List<ZhanDuiZhengBaZhanDuiData> LoadZhengBaRankData(int nowMonth)
		{
			List<ZhanDuiZhengBaZhanDuiData> roleList = new List<ZhanDuiZhengBaZhanDuiData>();
			try
			{
				string strSql = string.Format("SELECT zhanduiid,zhanduiname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,grade,`group`,state,data1,data2 FROM t_zhandui_zhengba where `month`={0} ORDER BY duanweirank ASC;", nowMonth);
				using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
				{
					while (sdr != null && sdr.Read())
					{
						ZhanDuiZhengBaZhanDuiData roleData = new ZhanDuiZhengBaZhanDuiData();
						roleData.ZhanDuiID = Convert.ToInt32(sdr["zhanduiid"]);
						roleData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
						roleData.ZhanDuiName = sdr["zhanduiname"].ToString();
						roleData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
						roleData.DuanWeiRank = Convert.ToInt32(sdr["duanweirank"]);
						roleData.ZhanLi = Convert.ToInt64(sdr["zhanli"]);
						if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
						{
							byte[] bytes = (byte[])sdr["data1"];
							roleData.MemberList = DataHelper2.BytesToObject<List<RoleOccuNameZhanLi>>(bytes, 0, bytes.Length);
						}
						roleData.Grade = Convert.ToInt32(sdr["grade"]);
						roleData.Group = Convert.ToInt32(sdr["group"]);
						roleData.State = Convert.ToInt32(sdr["state"]);
						roleList.Add(roleData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return roleList;
		}

		
		private List<ZhanDuiZhengBaPkLogData> LoadPkLogList(int nowMonth)
		{
			List<ZhanDuiZhengBaPkLogData> roleList = new List<ZhanDuiZhengBaPkLogData>();
			try
			{
				string strSql = string.Format("SELECT month,id,zhanduiid1,zoneid1,zhanduiname1,zhanduiid2,zoneid2,zhanduiname2,result,upgrade,starttime,endtime FROM t_zhandui_zhengba_pk_log where `month`={0} ORDER BY endtime ASC;", nowMonth);
				using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
				{
					while (sdr != null && sdr.Read())
					{
						roleList.Add(new ZhanDuiZhengBaPkLogData
						{
							Month = Convert.ToInt32(sdr["month"]),
							ID = Convert.ToInt32(sdr["id"]),
							ZhanDuiID1 = Convert.ToInt32(sdr["zhanduiid1"].ToString()),
							ZoneID1 = Convert.ToInt32(sdr["zoneid1"]),
							ZhanDuiName1 = sdr["zhanduiname1"].ToString(),
							ZhanDuiID2 = Convert.ToInt32(sdr["zhanduiid2"].ToString()),
							ZoneID2 = Convert.ToInt32(sdr["zoneid2"]),
							ZhanDuiName2 = sdr["zhanduiname2"].ToString(),
							PkResult = Convert.ToInt32(sdr["result"]),
							UpGrade = (Convert.ToInt32(sdr["upgrade"]) == 1),
							StartTime = Convert.ToDateTime(sdr["starttime"].ToString()),
							EndTime = Convert.ToDateTime(sdr["endtime"].ToString())
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return roleList;
		}

		
		public bool SavePkLog(ZhanDuiZhengBaPkLogData log)
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
					string sql = string.Format("INSERT INTO t_zhandui_zhengba_pk_log(month,id,zhanduiid1,zoneid1,zhanduiname1,zhanduiid2,zoneid2,zhanduiname2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},'{7}',{8},{9},'{10}','{11}');", new object[]
					{
						log.Month,
						log.ID,
						log.ZhanDuiID1,
						log.ZoneID1,
						log.ZhanDuiName1,
						log.ZhanDuiID2,
						log.ZoneID2,
						log.ZhanDuiName2,
						log.PkResult,
						log.UpGrade ? 1 : 0,
						log.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
						log.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
					});
					TianTiPersistence.Instance.AddDelayWriteSql(sql, null, null);
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

		
		public bool UpdateRole(int month, int zhanDuiID, int grade, int state)
		{
			try
			{
				string sql = string.Format("UPDATE t_zhandui_zhengba SET grade={0},state={1} WHERE month={2} AND zhanduiid={3}", new object[]
				{
					grade,
					state,
					month,
					zhanDuiID
				});
				TianTiPersistence.Instance.AddDelayWriteSql(sql, null, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "UpdateRole failed!", ex, true);
				return false;
			}
			return true;
		}

		
		private bool CheckZhengBaRank(int selectRoleIfNewCreate, int nowMonth)
		{
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 13, 201111));
				int oldMonth = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 13);
				return oldMonth == nowMonth;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		
		private bool BuildZhengBaRank(int nowMonth, List<ZhanDuiZhengBaZhanDuiData> rankList)
		{
			bool bMonthFirst = false;
			try
			{
				foreach (ZhanDuiZhengBaZhanDuiData roleData in rankList)
				{
					string repSql = string.Format("REPLACE INTO t_zhandui_zhengba(`month`,zhanduiid,zoneid,zhanduiname,duanweiid,duanweijifen,duanweirank,zhanli,`grade`,`group`,state,data1,data2) VALUES({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9},{10},@content,null)", new object[]
					{
						nowMonth,
						roleData.ZhanDuiID,
						roleData.ZoneId,
						roleData.ZhanDuiName,
						roleData.DuanWeiId,
						0,
						roleData.DuanWeiRank,
						roleData.ZhanLi,
						64,
						roleData.Group,
						0
					});
					DbHelperMySQL.ExecuteSqlInsertImg(repSql, new List<Tuple<string, byte[]>>
					{
						new Tuple<string, byte[]>("content", DataHelper2.ObjectToBytes<List<RoleOccuNameZhanLi>>(roleData.MemberList))
					});
				}
				DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 13, nowMonth));
				bMonthFirst = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return bMonthFirst;
		}

		
		public static readonly ZhanDuiZhengBaPersistence Instance = new ZhanDuiZhengBaPersistence();
	}
}
