using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	
	public class BangHuiMatchPersistence
	{
		
		private BangHuiMatchPersistence()
		{
		}

		
		public bool LoadDatabase(int seasonIDCur_Gold, int seasonIDLast_Gold, int seasonIDCur_Rookie, int seasonIDLast_Rookie)
		{
			return this.LoadBHMatchBHData(BangHuiMatchType.BHMT_Begin, this.BHMatchBHDataDict_Gold) && this.LoadBHMatchBHData(BangHuiMatchType.Rookie, this.BHMatchBHDataDict_Rookie) && this.LoadBHMatchPKInfoList(BangHuiMatchType.BHMT_Begin, this.BHMatchPKInfoList_Gold) && this.LoadBHMatchBHData_Join(BangHuiMatchType.BHMT_Begin, seasonIDCur_Gold, this.BHMatchBHDataList_GoldJoin, false) && this.LoadBHMatchBHData_Join(BangHuiMatchType.Rookie, seasonIDCur_Rookie, this.BHMatchBHDataList_RookieJoin, false) && this.LoadBHMatchBHData_Join(BangHuiMatchType.Rookie, seasonIDLast_Rookie, this.BHMatchBHDataList_RookieJoinLast, true) && this.LoadBHMatchBHData_LastSeason(BangHuiMatchType.BHMT_Begin, seasonIDLast_Gold) && this.LoadBHMatchBHData_LastSeason(BangHuiMatchType.Rookie, seasonIDLast_Rookie) && this.ReloadDatabasePerRound(1, seasonIDCur_Gold, seasonIDLast_Gold, true) && this.ReloadDatabasePerRound(2, seasonIDCur_Rookie, seasonIDLast_Rookie, true);
		}

		
		public bool ReloadRankInfo(int renkType, int seasonIDCur, int seasonIDLast)
		{
			return this.LoadBHMatchRankInfo(renkType, seasonIDCur, seasonIDLast, this.BHMatchRankInfoDict);
		}

		
		public bool ReloadDatabasePerRound(int matchType, int seasonIDCur, int seasonIDLast, bool changeSeason = false)
		{
			for (int rankLoop = 0; rankLoop < 14; rankLoop++)
			{
				if (this.CheckCanReloadRankInfo(matchType, rankLoop, changeSeason))
				{
					if (!this.LoadBHMatchRankInfo(rankLoop, seasonIDCur, seasonIDLast, this.BHMatchRankInfoDict))
					{
						return false;
					}
				}
			}
			return true;
		}

		
		private bool CheckCanReloadRankInfo(int matchType, int rankLoop, bool changeSeason)
		{
			switch (rankLoop)
			{
			case 0:
			case 2:
			case 4:
			case 6:
			case 8:
			case 10:
			case 12:
				if (matchType != 1)
				{
					return false;
				}
				goto IL_5F;
			}
			if (matchType != 2)
			{
				return false;
			}
			IL_5F:
			return changeSeason || (rankLoop != 4 && rankLoop != 0 && rankLoop != 5 && rankLoop != 1);
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

		
		public void SaveBHMatchBHData(BHMatchBHData data, bool chgChampion = true, bool chgKill = true)
		{
			string sql = string.Format("INSERT INTO t_banghui_match_bh(`type`, `bhid`, `bhname`, `zoneid_bh`, `rid`, `rname`, `zoneid_r`, `play`, `champion`, `kill`, bullshit, bestrecord) VALUES({0},{1},'{2}',{3},{4},'{5}',{6},{7},{8},{9},{10},{11}) ON DUPLICATE KEY UPDATE `bhname`='{2}',`zoneid_bh`={3},`rid`={4},`rname`='{5}',`zoneid_r`={6},`play`={7},`champion`={8},`kill`={9},`bullshit`={10},`bestrecord`={11};", new object[]
			{
				data.type,
				data.bhid,
				data.bhname,
				data.zoneid_bh,
				data.rid,
				data.rname,
				data.zoneid_r,
				data.hist_play,
				data.hist_champion,
				data.hist_kill,
				data.hist_bullshit,
				data.best_record
			});
			this.ExecuteSqlNoQuery(sql);
			if (chgChampion)
			{
				sql = string.Format("UPDATE t_banghui_match_bh SET ranktm_champion=NOW() WHERE `type`={0} AND `bhid`={1};", data.type, data.bhid);
				this.ExecuteSqlNoQuery(sql);
			}
			if (chgKill)
			{
				sql = string.Format("UPDATE t_banghui_match_bh SET ranktm_kill=NOW() WHERE `type`={0} AND `bhid`={1};", data.type, data.bhid);
				this.ExecuteSqlNoQuery(sql);
			}
		}

		
		public void SaveBHMatchBHSeasonData(int season, BHMatchBHData data, bool chgWin = true, bool chgScore = true)
		{
			string sql = string.Format("INSERT INTO t_banghui_match_bh_season(`type`, season, bhid, win, `group`, score) VALUES({0},{1},{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE win={3},`group`={4},score={5};", new object[]
			{
				data.type,
				season,
				data.bhid,
				data.cur_win,
				data.group,
				data.cur_score
			});
			this.ExecuteSqlNoQuery(sql);
			if (chgWin)
			{
				sql = string.Format("UPDATE t_banghui_match_bh_season SET ranktm_win=NOW() WHERE `type`={0} AND `season`={1} AND `bhid`={2};", data.type, season, data.bhid);
				this.ExecuteSqlNoQuery(sql);
			}
			if (chgScore)
			{
				sql = string.Format("UPDATE t_banghui_match_bh_season SET ranktm_score=NOW() WHERE `type`={0} AND `season`={1} AND `bhid`={2};", data.type, season, data.bhid);
				this.ExecuteSqlNoQuery(sql);
			}
		}

		
		public void SaveBHMatchRolesSeasonData(int season, BHMatchRoleData roleData, bool chgMvp = true, bool chgKill = true)
		{
			string sql = string.Format("INSERT INTO t_banghui_match_roles_season(`type`, season, rid, mvp, `kill`) VALUES({0},{1},{2},{3},{4}) ON DUPLICATE KEY UPDATE mvp=mvp+{3}, `kill`=`kill`+{4};", new object[]
			{
				roleData.type,
				season,
				roleData.rid,
				roleData.mvp,
				roleData.kill
			});
			this.AddDelayWriteSql(sql);
			if (chgMvp)
			{
				sql = string.Format("UPDATE t_banghui_match_roles_season SET ranktm_mvp=NOW() WHERE `type`={0} AND `season`={1} AND `rid`={2};", roleData.type, season, roleData.rid);
				this.AddDelayWriteSql(sql);
			}
			if (chgKill)
			{
				sql = string.Format("UPDATE t_banghui_match_roles_season SET ranktm_kill=NOW() WHERE `type`={0} AND `season`={1} AND `rid`={2};", roleData.type, season, roleData.rid);
				this.AddDelayWriteSql(sql);
			}
		}

		
		public void SaveBHMatchRolesData(int type, int rid, string rname, int zoneid, int bhid, byte[] roledata)
		{
			if (null != roledata)
			{
				string sql = string.Format("INSERT INTO t_banghui_match_roles(`type`, rid, rname, zoneid, bhid, data1) VALUES({0},{1},'{2}',{3},{4}, @content) ON DUPLICATE KEY UPDATE rname='{2}', bhid={4}, data1=@content;", new object[]
				{
					type,
					rid,
					rname,
					zoneid,
					bhid
				});
				DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", roledata)
				});
			}
			else
			{
				string sql = string.Format("INSERT INTO t_banghui_match_roles(`type`, rid, rname, zoneid, bhid) VALUES({0},{1},'{2}',{3},{4}) ON DUPLICATE KEY UPDATE rname='{2}', bhid={4};", new object[]
				{
					type,
					rid,
					rname,
					zoneid,
					bhid
				});
				this.AddDelayWriteSql(sql);
			}
		}

		
		public void SaveBHMatchPKInfo(BangHuiMatchPKInfo pkinfo)
		{
			string sql = string.Format("INSERT INTO t_banghui_match_pk_log(`type`, season, round, bhid1, zoneid1, bhid2, zoneid2, result, logtime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},'{8}')  ON DUPLICATE KEY UPDATE result={7};", new object[]
			{
				pkinfo.type,
				pkinfo.season,
				pkinfo.round,
				pkinfo.bhid1,
				pkinfo.zoneid1,
				pkinfo.bhid2,
				pkinfo.zoneid2,
				pkinfo.result,
				TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss")
			});
			this.ExecuteSqlNoQuery(sql);
		}

		
		public int LoadLastSeasonIDGold()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 44, 0));
			object value = DbHelperMySQL.GetSingle("select value from t_async where id = " + 44);
			return Convert.ToInt32(value);
		}

		
		public void SaveLastSeasonIDGold(int seasonID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 44, seasonID));
		}

		
		public int GetGoldSeasonID()
		{
			object value = DbHelperMySQL.GetSingle("SELECT MAX(season) FROM t_banghui_match_bh_season WHERE `type`=" + 1);
			return Convert.ToInt32(value);
		}

		
		public byte[] LoadBHMatchRoleData(int type, int rid)
		{
			byte[] result;
			try
			{
				object roledata = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}", type, rid));
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

		
		private bool LoadBHMatchBHData(BangHuiMatchType type, Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict)
		{
			bool result;
			if (null == BHMatchBHDataDict)
			{
				result = false;
			}
			else
			{
				BHMatchBHDataDict.Clear();
				try
				{
					string strSql = string.Format("SELECT * FROM t_banghui_match_bh where `type`={0}", (int)type);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						KuaFuData<BHMatchBHData> bhData = new KuaFuData<BHMatchBHData>();
						bhData.V.type = (int)type;
						bhData.V.bhid = Convert.ToInt32(sdr["bhid"]);
						bhData.V.bhname = (sdr["bhname"] as string);
						bhData.V.zoneid_bh = Convert.ToInt32(sdr["zoneid_bh"]);
						bhData.V.rid = Convert.ToInt32(sdr["rid"]);
						bhData.V.rname = (sdr["rname"] as string);
						bhData.V.zoneid_r = Convert.ToInt32(sdr["zoneid_r"]);
						bhData.V.hist_play = Convert.ToInt32(sdr["play"]);
						bhData.V.hist_champion = Convert.ToInt32(sdr["champion"]);
						bhData.V.hist_bullshit = Convert.ToInt32(sdr["bullshit"]);
						bhData.V.best_record = Convert.ToInt32(sdr["bestrecord"]);
						string strGetWin = string.Format("select sum(win) totalwin from t_banghui_match_bh_season where `type`={0} and bhid={1}", (int)type, bhData.V.bhid);
						bhData.V.hist_win = Convert.ToInt32(DbHelperMySQL.GetSingle(strGetWin));
						string strGetScore = string.Format("select sum(score) totalscore from t_banghui_match_bh_season where `type`={0} and bhid={1}", (int)type, bhData.V.bhid);
						bhData.V.hist_score = Convert.ToInt32(DbHelperMySQL.GetSingle(strGetScore));
						bhData.V.hist_kill = Convert.ToInt32(sdr["kill"]);
						TimeUtil.AgeByNow(ref bhData.Age);
						BHMatchBHDataDict[bhData.V.bhid] = bhData;
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

		
		private bool LoadBHMatchPKInfoList(BangHuiMatchType type, KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold)
		{
			bool result;
			if (null == BHMatchPKInfoList_Gold)
			{
				result = false;
			}
			else
			{
				BHMatchPKInfoList_Gold.V.Clear();
				try
				{
					KuaFuData<BHMatchBHData> bhData = null;
					string strSql = string.Format("SELECT * FROM t_banghui_match_pk_log WHERE `type`={0} ORDER BY `season` DESC, `round` DESC LIMIT {1}", (int)type, 80);
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						BangHuiMatchPKInfo pkInfo = new BangHuiMatchPKInfo();
						pkInfo.type = Convert.ToByte(sdr["type"]);
						pkInfo.season = Convert.ToInt32(sdr["season"]);
						pkInfo.round = Convert.ToByte(sdr["round"]);
						pkInfo.bhid1 = Convert.ToInt32(sdr["bhid1"]);
						pkInfo.bhid2 = Convert.ToInt32(sdr["bhid2"]);
						pkInfo.result = Convert.ToByte(sdr["result"]);
						pkInfo.zoneid1 = Convert.ToInt32(sdr["zoneid1"]);
						pkInfo.zoneid2 = Convert.ToInt32(sdr["zoneid2"]);
						if (this.BHMatchBHDataDict_Gold.TryGetValue(pkInfo.bhid1, out bhData))
						{
							pkInfo.bhname1 = KuaFuServerManager.FormatName(bhData.V.zoneid_bh, bhData.V.bhname);
						}
						if (this.BHMatchBHDataDict_Gold.TryGetValue(pkInfo.bhid2, out bhData))
						{
							pkInfo.bhname2 = KuaFuServerManager.FormatName(bhData.V.zoneid_bh, bhData.V.bhname);
						}
						BHMatchPKInfoList_Gold.V.Add(pkInfo);
					}
					TimeUtil.AgeByNow(ref BHMatchPKInfoList_Gold.Age);
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

		
		private bool LoadBHMatchBHData_LastSeason(BangHuiMatchType type, int seasonID)
		{
			try
			{
				KuaFuData<BHMatchBHData> bhData = null;
				string strSql = string.Format("SELECT * FROM t_banghui_match_bh_season where `type`={0} and `season`={1}", (int)type, seasonID);
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					int bhid = Convert.ToInt32(sdr["bhid"]);
					if (type == BangHuiMatchType.BHMT_Begin && this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out bhData))
					{
						TimeUtil.AgeByNow(ref bhData.Age);
						bhData.V.last_win = Convert.ToInt32(sdr["win"]);
						bhData.V.last_score = 0;
					}
					if (type == BangHuiMatchType.Rookie && this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out bhData))
					{
						TimeUtil.AgeByNow(ref bhData.Age);
						bhData.V.last_win = Convert.ToInt32(sdr["win"]);
						bhData.V.last_score = Convert.ToInt32(sdr["score"]);
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
				return false;
			}
			return true;
		}

		
		private bool LoadBHMatchBHData_Join(BangHuiMatchType type, int seasonID, List<BHMatchBHData> BHMatchBHDataList_Join, bool lastSeason = false)
		{
			bool result;
			if (null == BHMatchBHDataList_Join)
			{
				result = false;
			}
			else
			{
				BHMatchBHDataList_Join.Clear();
				try
				{
					KuaFuData<BHMatchBHData> bhData = null;
					string strSql = string.Format("SELECT * FROM t_banghui_match_bh_season WHERE `type`={0} AND `season`={1}", (int)type, seasonID);
					if (type == BangHuiMatchType.BHMT_Begin)
					{
						strSql += string.Format(" ORDER BY `group` ASC", new object[0]);
					}
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						int bhid = Convert.ToInt32(sdr["bhid"]);
						if (type == BangHuiMatchType.BHMT_Begin && this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out bhData))
						{
							TimeUtil.AgeByNow(ref bhData.Age);
							if (!lastSeason)
							{
								bhData.V.cur_win = Convert.ToInt32(sdr["win"]);
								bhData.V.group = Convert.ToInt32(sdr["group"]);
								bhData.V.cur_score = 0;
							}
							BHMatchBHDataList_Join.Add(bhData.V);
						}
						if (type == BangHuiMatchType.Rookie && this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out bhData))
						{
							TimeUtil.AgeByNow(ref bhData.Age);
							if (!lastSeason)
							{
								bhData.V.cur_win = Convert.ToInt32(sdr["win"]);
								bhData.V.group = 0;
								bhData.V.cur_score = Convert.ToInt32(sdr["score"]);
							}
							BHMatchBHDataList_Join.Add(bhData.V);
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
					return false;
				}
				result = true;
			}
			return result;
		}

		
		private string FormatLoadBHMatchRankSql(int rankType, int seasonIDCur, int seasonIDLast)
		{
			int RankLimit = BHMatchConsts.MatchRankLimit[rankType];
			string strSql = "";
			switch (rankType)
			{
			case 0:
				strSql = string.Format("SELECT bhid a, win b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY `group` ASC LIMIT {2};", 1, seasonIDLast, RankLimit);
				break;
			case 1:
				strSql = string.Format("SELECT bhid a, score b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY score DESC, ranktm_score ASC, bhid ASC LIMIT {2};", 2, seasonIDLast, RankLimit);
				break;
			case 2:
				strSql = string.Format("SELECT bhid a, champion b FROM t_banghui_match_bh WHERE `type`={0} AND champion<>0 ORDER BY champion DESC, ranktm_champion ASC, bhid ASC LIMIT {1};", 1, RankLimit);
				break;
			case 3:
				strSql = string.Format("SELECT bhid a, champion b FROM t_banghui_match_bh WHERE `type`={0} AND champion<>0 ORDER BY champion DESC, ranktm_champion ASC, bhid ASC LIMIT {1};", 2, RankLimit);
				break;
			case 4:
				strSql = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 1, seasonIDLast, RankLimit);
				break;
			case 5:
				strSql = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 2, seasonIDLast, RankLimit);
				break;
			case 6:
				strSql = string.Format("SELECT rid a, mvp b FROM (SELECT rid, SUM(mvp) mvp, MAX(ranktm_mvp) ranktm FROM t_banghui_match_roles_season WHERE `type`={0} GROUP BY rid) a1\r\n                                        WHERE mvp<>0 ORDER BY mvp DESC, ranktm ASC, rid ASC LIMIT {1};", 1, RankLimit);
				break;
			case 7:
				strSql = string.Format("SELECT rid a, mvp b FROM (SELECT rid, SUM(mvp) mvp, MAX(ranktm_mvp) ranktm FROM t_banghui_match_roles_season WHERE `type`={0} GROUP BY rid) a1 \r\n                                        WHERE mvp<>0 ORDER BY mvp DESC, ranktm ASC, rid ASC LIMIT {1};", 2, RankLimit);
				break;
			case 8:
				strSql = string.Format("SELECT bhid a, win b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY win DESC, `group` ASC, ranktm_win ASC, bhid ASC LIMIT {2};", 1, seasonIDCur, RankLimit);
				break;
			case 9:
				strSql = string.Format("SELECT bhid a, score b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY score DESC, ranktm_score ASC, bhid ASC LIMIT {2};", 2, seasonIDCur, RankLimit);
				break;
			case 10:
				strSql = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 1, seasonIDCur, RankLimit);
				break;
			case 11:
				strSql = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 2, seasonIDCur, RankLimit);
				break;
			case 12:
				strSql = string.Format("SELECT rid a, `kill` b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND `kill`<>0 ORDER BY `kill` DESC, ranktm_kill ASC, rid ASC LIMIT {2};", 1, seasonIDCur, RankLimit);
				break;
			case 13:
				strSql = string.Format("SELECT rid a, `kill` b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND `kill`<>0 ORDER BY `kill` DESC, ranktm_kill ASC, rid ASC LIMIT {2};", 2, seasonIDCur, RankLimit);
				break;
			default:
				return strSql;
			}
			return strSql;
		}

		
		private bool LoadBHMatchRankInfo(int rankType, int seasonCur, int seasonLast, KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict)
		{
			bool result;
			if (null == BHMatchRankInfoDict)
			{
				result = false;
			}
			else
			{
				List<BangHuiMatchRankInfo> rankList = null;
				if (!BHMatchRankInfoDict.V.TryGetValue(rankType, out rankList))
				{
					rankList = (BHMatchRankInfoDict.V[rankType] = new List<BangHuiMatchRankInfo>());
				}
				else
				{
					rankList.Clear();
				}
				try
				{
					string strSql = this.FormatLoadBHMatchRankSql(rankType, seasonCur, seasonLast);
					if (string.IsNullOrEmpty(strSql))
					{
						return false;
					}
					MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					while (sdr != null && sdr.Read())
					{
						BangHuiMatchRankInfo rankInfo = new BangHuiMatchRankInfo();
						rankInfo.Key = Convert.ToInt32(sdr["a"]);
						rankInfo.Value = Convert.ToInt32(sdr["b"]);
						switch (rankType)
						{
						case 4:
						case 6:
						case 10:
						case 12:
						{
							string strParam = string.Format("SELECT zoneid,rname FROM t_banghui_match_roles WHERE `type`={0} AND rid={1};", 1, rankInfo.Key);
							object[] arr;
							if (DbHelperMySQL.GetSingleValues(strParam, out arr) >= 0)
							{
								rankInfo.Param1 = KuaFuServerManager.FormatName(Convert.ToInt32(arr[0]), arr[1].ToString());
							}
							string strParam2 = string.Format("SELECT zoneid_bh,bhname FROM t_banghui_match_bh, \r\n                                                (SELECT bhid FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}) a1 WHERE t_banghui_match_bh.bhid = a1.bhid;", 1, rankInfo.Key);
							if (DbHelperMySQL.GetSingleValues(strParam2, out arr) >= 0)
							{
								rankInfo.Param2 = KuaFuServerManager.FormatName(Convert.ToInt32(arr[0]), arr[1].ToString());
							}
							rankList.Add(rankInfo);
							break;
						}
						case 5:
						case 7:
						case 11:
						case 13:
						{
							string strParam = string.Format("SELECT zoneid,rname FROM t_banghui_match_roles WHERE `type`={0} AND rid={1};", 2, rankInfo.Key);
							rankInfo.Param1 = Convert.ToString(DbHelperMySQL.GetSingle(strParam));
							object[] arr;
							if (DbHelperMySQL.GetSingleValues(strParam, out arr) >= 0)
							{
								rankInfo.Param1 = KuaFuServerManager.FormatName(Convert.ToInt32(arr[0]), arr[1].ToString());
							}
							string strParam2 = string.Format("SELECT zoneid_bh,bhname FROM t_banghui_match_bh, \r\n                                                (SELECT bhid FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}) a1 WHERE t_banghui_match_bh.bhid = a1.bhid;", 2, rankInfo.Key);
							if (DbHelperMySQL.GetSingleValues(strParam2, out arr) >= 0)
							{
								rankInfo.Param2 = KuaFuServerManager.FormatName(Convert.ToInt32(arr[0]), arr[1].ToString());
							}
							rankList.Add(rankInfo);
							break;
						}
						case 8:
						case 9:
							goto IL_24B;
						default:
							goto IL_24B;
						}
						continue;
						IL_24B:
						KuaFuData<BHMatchBHData> bhData = null;
						if (this.BHMatchBHDataDict_Gold.TryGetValue(rankInfo.Key, out bhData))
						{
							rankInfo.Param1 = KuaFuServerManager.FormatName(bhData.V.zoneid_bh, bhData.V.bhname);
							rankInfo.Param2 = KuaFuServerManager.FormatName(bhData.V.zoneid_r, bhData.V.rname);
							rankList.Add(rankInfo);
						}
						else if (this.BHMatchBHDataDict_Rookie.TryGetValue(rankInfo.Key, out bhData))
						{
							rankInfo.Param1 = KuaFuServerManager.FormatName(bhData.V.zoneid_bh, bhData.V.bhname);
							rankInfo.Param2 = KuaFuServerManager.FormatName(bhData.V.zoneid_r, bhData.V.rname);
							rankList.Add(rankInfo);
						}
					}
					TimeUtil.AgeByNow(ref BHMatchRankInfoDict.Age);
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

		
		public static readonly BangHuiMatchPersistence Instance = new BangHuiMatchPersistence();

		
		public object Mutex = new object();

		
		public KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict = new KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>>();

		
		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		
		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		
		public KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold = new KuaFuData<List<BangHuiMatchPKInfo>>();

		
		public List<BHMatchBHData> BHMatchBHDataList_GoldJoin = new List<BHMatchBHData>();

		
		public List<BHMatchBHData> BHMatchBHDataList_RookieJoin = new List<BHMatchBHData>();

		
		public List<BHMatchBHData> BHMatchBHDataList_RookieJoinLast = new List<BHMatchBHData>();

		
		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
