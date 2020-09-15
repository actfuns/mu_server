using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000082 RID: 130
	public class TianTiPersistence
	{
		// Token: 0x06000654 RID: 1620 RVA: 0x00056E9C File Offset: 0x0005509C
		private TianTiPersistence()
		{
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00056F34 File Offset: 0x00055134
		public void InitConfig()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load("config.xml");
				this.SignUpWaitSecs3 = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SignUpWaitSecs3", "value", 10L);
				this.SignUpWaitSecsAll = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SignUpWaitSecsAll", "value", 15L);
				this.RankData.MaxPaiMingRank = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "MaxPaiMingRank", "value", 50000L);
				this.MaxSendDetailDataCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "MaxSendDetailDataCount", "value", 100L);
				this.MaxRolePairFightCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "MaxRolePairFightCount", "value", 3L);
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(id),0) FROM t_tianti_game_fuben;"));
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00057064 File Offset: 0x00055264
		public bool AddDelayWriteSql(string sql, List<Tuple<string, byte[]>> imgList = null, Action<object, int> callback = null)
		{
			bool result;
			lock (this.Mutex)
			{
				if (this.ServerStopping)
				{
					result = false;
				}
				else
				{
					this.DelayWriteSqlQueue.Enqueue(new Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>(sql, imgList, callback));
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x000570D4 File Offset: 0x000552D4
		public void DelayWriteDataProc()
		{
			List<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>> list = null;
			lock (this.Mutex)
			{
				if (this.DelayWriteSqlQueue.Count == 0)
				{
					return;
				}
				list = this.DelayWriteSqlQueue.ToList<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>>();
				this.DelayWriteSqlQueue.Clear();
			}
			foreach (Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>> sql in list)
			{
				try
				{
					LogManager.WriteLog(LogTypes.SQL, sql.Item1, null, true);
					int ret;
					if (null == sql.Item2)
					{
						ret = DbHelperMySQL.ExecuteSql(sql.Item1);
					}
					else
					{
						ret = DbHelperMySQL.ExecuteSqlInsertImg(sql.Item1, sql.Item2);
					}
					if (sql.Item3 != null)
					{
						sql.Item3(sql, ret);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("sql: {0}\r\n{1}", sql, ex.ToString()));
				}
			}
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00057230 File Offset: 0x00055430
		public void OnStopServer()
		{
			this.ServerStopping = true;
			this.DelayWriteDataProc();
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00057244 File Offset: 0x00055444
		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(LogTypes.Warning, "TianTi 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00057298 File Offset: 0x00055498
		public TianTiRankData GetTianTiRankData(DateTime modifyTime)
		{
			TianTiRankData tianTiRankData = new TianTiRankData();
			lock (this.Mutex)
			{
				tianTiRankData.ModifyTime = this.RankData.ModifyTime;
				tianTiRankData.MaxPaiMingRank = this.RankData.MaxPaiMingRank;
				if (modifyTime < this.RankData.ModifyTime && null != this.RankData.TianTiRoleInfoDataList)
				{
					tianTiRankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiRoleInfoDataList);
				}
				if (modifyTime < this.RankData.ModifyTime && null != this.RankData.TianTiMonthRoleInfoDataList)
				{
					tianTiRankData.TianTiMonthRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiMonthRoleInfoDataList);
				}
			}
			return tianTiRankData;
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00057394 File Offset: 0x00055594
		private bool ReloadTianTiRankDayList(List<TianTiRoleInfoData> tianTiRoleInfoDataList)
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_roles where duanweijifen>0 ORDER BY duanweijifen DESC,duanweirank DESC LIMIT {0};", this.RankData.MaxPaiMingRank);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
					tianTiRoleInfoData.RoleId = Convert.ToInt32(sdr["rid"]);
					if (index <= this.MaxSendDetailDataCount)
					{
						tianTiRoleInfoData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
						tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
						tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
						tianTiRoleInfoData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
						tianTiRoleInfoData.RoleName = sdr["rname"].ToString();
						if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
						{
							tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
						}
					}
					tianTiRoleInfoData.DuanWeiRank = index;
					tianTiRoleInfoDataList.Add(tianTiRoleInfoData);
					index++;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return false;
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0005752C File Offset: 0x0005572C
		private bool LoadTianTiRankDayList(List<TianTiRoleInfoData> tianTiRoleInfoDataList)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT r.rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_roles r, t_tianti_day_paihang d WHERE r.rid=d.rid ORDER BY d.`rank` ASC LIMIT {0};", this.RankData.MaxPaiMingRank);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
					tianTiRoleInfoData.RoleId = Convert.ToInt32(sdr["rid"]);
					if (index <= this.MaxSendDetailDataCount)
					{
						tianTiRoleInfoData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
						tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
						tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
						tianTiRoleInfoData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
						tianTiRoleInfoData.RoleName = sdr["rname"].ToString();
						if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
						{
							tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
						}
					}
					tianTiRoleInfoData.DuanWeiRank = index;
					tianTiRoleInfoDataList.Add(tianTiRoleInfoData);
					result = true;
					index++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return result;
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x000576D0 File Offset: 0x000558D0
		public void LoadTianTiRankData(DateTime now)
		{
			try
			{
				this.ExecuteSqlNoQuery("INSERT IGNORE INTO t_async(`id`,`value`) VALUES(4,1);");
				object ageObj = DbHelperMySQL.GetSingle("select value from t_async where id = " + 4);
				if (null != ageObj)
				{
					int dayId = (int)ageObj;
					DateTime modifyDate = DataHelper2.GetRealDate(dayId);
					List<TianTiRoleInfoData> tianTiRoleInfoDataList = new List<TianTiRoleInfoData>();
					List<TianTiRoleInfoData> tianTiMonthRoleInfoDataList = new List<TianTiRoleInfoData>();
					MySqlDataReader sdr = null;
					try
					{
						this.LoadTianTiRankDayList(tianTiRoleInfoDataList);
						sdr = DbHelperMySQL.ExecuteReader(string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_month_paihang ORDER BY `duanweirank` ASC LIMIT {0};", this.RankData.MaxPaiMingRank), false);
						int index = 1;
						while (sdr.Read())
						{
							TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
							tianTiRoleInfoData.RoleId = Convert.ToInt32(sdr["rid"]);
							if (index <= this.MaxSendDetailDataCount)
							{
								tianTiRoleInfoData.ZoneId = Convert.ToInt32(sdr["zoneid"]);
								tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(sdr["duanweiid"]);
								tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]);
								tianTiRoleInfoData.ZhanLi = Convert.ToInt32(sdr["zhanli"]);
								tianTiRoleInfoData.RoleName = sdr["rname"].ToString();
								if (!sdr.IsDBNull(sdr.GetOrdinal("data1")))
								{
									tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])sdr["data1"];
								}
							}
							tianTiRoleInfoData.DuanWeiRank = index;
							tianTiMonthRoleInfoDataList.Add(tianTiRoleInfoData);
							index++;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
					finally
					{
						if (null != sdr)
						{
							sdr.Close();
						}
					}
					lock (this.Mutex)
					{
						this.RankData.ModifyTime = modifyDate;
						this.RankData.TianTiRoleInfoDataList = tianTiRoleInfoDataList;
						this.RankData.TianTiMonthRoleInfoDataList = tianTiMonthRoleInfoDataList;
					}
					if (DataHelper2.GetOffsetDay(now) != dayId)
					{
						this.UpdateTianTiRankData(now, modifyDate.Month != now.Month, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x00057998 File Offset: 0x00055B98
		public void UpdateTianTiRankData(DateTime now, bool monthRank = false, bool force = false)
		{
			if (Monitor.TryEnter(this.MutexPaiHang))
			{
				try
				{
					if (!force)
					{
						lock (this.Mutex)
						{
							if (this.RankData.ModifyTime.DayOfYear == now.DayOfYear)
							{
								return;
							}
						}
					}
					if (!monthRank)
					{
						if (now.Day == 1)
						{
							monthRank = true;
						}
					}
					List<TianTiRoleInfoData> tianTiRoleInfoDataList = new List<TianTiRoleInfoData>();
					string strSql = "";
					MySqlDataReader sdr = null;
					try
					{
						this.ReloadTianTiRankDayList(tianTiRoleInfoDataList);
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						return;
					}
					finally
					{
						if (null != sdr)
						{
							sdr.Close();
						}
					}
					try
					{
						if (tianTiRoleInfoDataList.Count > 0)
						{
							int ret = DbHelperMySQL.ExecuteSql(string.Format("UPDATE t_tianti_roles SET `duanweirank`={0}", this.RankData.MaxPaiMingRank + 1));
							if (ret >= 0)
							{
								ret = DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_day_paihang;");
							}
							if (ret >= 0)
							{
								int c = tianTiRoleInfoDataList.Count;
								int numPerExec = 50;
								for (int i = 0; i < c; i++)
								{
									if (i % numPerExec == 0)
									{
										strSql = "INSERT INTO t_tianti_day_paihang(rid,rank) VALUES";
									}
									strSql += string.Format("({0},{1})", tianTiRoleInfoDataList[i].RoleId, tianTiRoleInfoDataList[i].DuanWeiRank);
									if (i % numPerExec == numPerExec - 1 || i == c - 1)
									{
										DbHelperMySQL.ExecuteSql(strSql);
									}
									else
									{
										strSql += ',';
									}
								}
								DbHelperMySQL.ExecuteSql("UPDATE t_tianti_roles r, t_tianti_day_paihang d SET r.`duanweirank` = d.`rank` WHERE r.`rid` = d.`rid`;");
								if (monthRank)
								{
									DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_month_paihang;");
									strSql = "INSERT INTO t_tianti_month_paihang SELECT * FROM t_tianti_roles WHERE rid IN (SELECT rid FROM t_tianti_day_paihang) ORDER BY `duanweirank` ASC;";
									DbHelperMySQL.ExecuteSql(strSql);
									DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_day_paihang;");
									DbHelperMySQL.ExecuteSql("UPDATE t_tianti_roles SET `duanweirank`=0,`duanweijifen`=0,`duanweiid`=0;");
								}
							}
							if (ret >= 0)
							{
								strSql = string.Format("UPDATE t_async SET `value`={1} WHERE `id`={0};", 4, DataHelper2.GetOffsetDay(now));
								this.ExecuteSqlNoQuery(strSql);
							}
						}
						lock (this.Mutex)
						{
							this.RankData.ModifyTime = now;
							if (monthRank)
							{
								this.RankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>();
								this.RankData.TianTiMonthRoleInfoDataList = tianTiRoleInfoDataList;
							}
							else
							{
								this.RankData.TianTiRoleInfoDataList = tianTiRoleInfoDataList;
							}
						}
						if (monthRank)
						{
							try
							{
								ZhengBaManagerK.Instance().ReloadSyncData(now);
							}
							catch (Exception ex)
							{
								LogManager.WriteLog(LogTypes.Error, "UpdateTianTiRankData -> zhengba reload execption", ex, true);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				finally
				{
					Monitor.Exit(this.MutexPaiHang);
				}
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00057D94 File Offset: 0x00055F94
		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			if (this.TianTiRoleInfoDataQueue.Count > 100000)
			{
				TianTiRoleInfoData tmpData;
				this.TianTiRoleInfoDataQueue.TryDequeue(out tmpData);
			}
			this.TianTiRoleInfoDataQueue.Enqueue(data);
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00057DD8 File Offset: 0x00055FD8
		public void WriteRoleInfoDataToDb(TianTiRoleInfoData data)
		{
			try
			{
				List<Tuple<string, byte[]>> imgList = new List<Tuple<string, byte[]>>();
				imgList.Add(new Tuple<string, byte[]>("content", data.TianTiPaiHangRoleData));
				imgList.Add(new Tuple<string, byte[]>("mirror", data.PlayerJingJiMirrorData));
				DbHelperMySQL.ExecuteSqlInsertImg(string.Format("INSERT INTO t_tianti_roles(rid,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,rname,data1,data2) VALUES({0},{1},{2},{3},{4},{5},'{6}',@content,@mirror) ON DUPLICATE KEY UPDATE `zoneid`={1},`duanweiid`={2},`duanweijifen`={3},`duanweirank`={4},`zhanli`={5},`rname`='{6}',`data1`=@content,`data2`=@mirror;", new object[]
				{
					data.RoleId,
					data.ZoneId,
					data.DuanWeiId,
					data.DuanWeiJiFen,
					data.DuanWeiRank,
					data.ZhanLi,
					data.RoleName
				}), imgList);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x00057EE4 File Offset: 0x000560E4
		public void WriteRoleInfoDataProc()
		{
			TianTiRoleInfoData data;
			for (int i = 0; i < 1000; i++)
			{
				if (!this.TianTiRoleInfoDataQueue.TryDequeue(out data))
				{
					break;
				}
				this.WriteRoleInfoDataToDb(data);
				lock (this.Mutex)
				{
					if (this.RankData.TianTiRoleInfoDataList.Count < 3)
					{
						if (!this.RankData.TianTiRoleInfoDataList.Exists((TianTiRoleInfoData x) => x.RoleId == data.RoleId))
						{
							this.RankData.ModifyTime = TimeUtil.NowDateTime();
							this.RankData.TianTiRoleInfoDataList.Add(data);
							data.DuanWeiRank = this.RankData.TianTiRoleInfoDataList.Count;
						}
					}
				}
			}
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0005800C File Offset: 0x0005620C
		public int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -15;
			}
			return result;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0005804C File Offset: 0x0005624C
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0005806C File Offset: 0x0005626C
		public void LogCreateTianTiFuBen(int gameId, int serverId, int fubenSeqId, int roleCount)
		{
			string sql = string.Format("INSERT INTO t_tianti_game_fuben(`id`,`serverid`,`fubensid`,`createtime`,`rolenum`) VALUES({0},{1},{2},'{3}',{4});", new object[]
			{
				gameId,
				serverId,
				fubenSeqId,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
				roleCount
			});
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x000580D0 File Offset: 0x000562D0
		public bool LoadZhanDuiData(Dictionary<int, TianTi5v5ZhanDuiData> dict)
		{
			bool result = false;
			string sql = "select `zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid,zorkjifen,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui";
			using (MySqlDataReader reader = DbHelperMySQL.ExecuteReader(sql, false))
			{
				result = true;
				while (reader.Read())
				{
					try
					{
						TianTi5v5ZhanDuiData data = new TianTi5v5ZhanDuiData();
						data.ZhanDuiID = Convert.ToInt32(reader[0].ToString());
						data.LeaderRoleID = Convert.ToInt32(reader["leaderid"].ToString());
						data.XuanYan = reader["xuanyan"].ToString();
						data.ZhanDuiName = reader["zhanduiname"].ToString();
						data.DuanWeiId = Convert.ToInt32(reader["duanweiid"].ToString());
						data.ZhanDouLi = Convert.ToInt64(reader["zhanli"].ToString());
						byte[] bytes = (reader["data1"] as byte[]) ?? new byte[0];
						data.teamerList = DataHelper2.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(bytes, 0, bytes.Length);
						data.DuanWeiJiFen = Convert.ToInt32(reader["duanweijifen"].ToString());
						data.DuanWeiRank = Convert.ToInt32(reader["duanweirank"].ToString());
						data.LianSheng = Convert.ToInt32(reader["liansheng"].ToString());
						data.FightCount = Convert.ToInt32(reader["fightcount"].ToString());
						data.SuccessCount = Convert.ToInt32(reader["successcount"].ToString());
						data.LastFightTime = Convert.ToDateTime(reader["lastfighttime"].ToString());
						data.MonthDuanWeiRank = Convert.ToInt32(reader["monthduanweirank"].ToString());
						data.LeaderRoleName = reader["leaderrolename"].ToString();
						data.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
						data.ZorkJiFen = Convert.ToInt32(reader["zorkjifen"].ToString());
						data.ZorkLastFightTime = Convert.ToDateTime(reader["zorklastfighttime"].ToString());
						data.EscapeJiFen = Convert.ToInt32(reader["escapejifen"].ToString());
						data.EscapeLastFightTime = Convert.ToDateTime(reader["escapelastfighttime"].ToString());
						dict[data.ZhanDuiID] = data;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			return result;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x000583A0 File Offset: 0x000565A0
		public int LoadZhanDuiRankList(List<TianTi5v5ZhanDuiData> list, int dayID)
		{
			int result = 0;
			string sql = string.Format("select `zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid from t_kf_5v5_zhandui_paihang where dayid={0} order by duanweirank", dayID);
			using (MySqlDataReader reader = DbHelperMySQL.ExecuteReader(sql, false))
			{
				result = 1;
				while (reader.Read())
				{
					try
					{
						TianTi5v5ZhanDuiData data = new TianTi5v5ZhanDuiData();
						data.ZhanDuiID = Convert.ToInt32(reader[0].ToString());
						data.LeaderRoleID = Convert.ToInt32(reader["leaderid"].ToString());
						data.XuanYan = reader["xuanyan"].ToString();
						data.ZhanDuiName = reader["zhanduiname"].ToString();
						data.DuanWeiId = Convert.ToInt32(reader["duanweiid"].ToString());
						data.ZhanDouLi = Convert.ToInt64(reader["zhanli"].ToString());
						byte[] bytes = (reader["data1"] as byte[]) ?? new byte[0];
						data.teamerList = DataHelper2.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(bytes, 0, bytes.Length);
						data.DuanWeiJiFen = Convert.ToInt32(reader["duanweijifen"].ToString());
						data.DuanWeiRank = Convert.ToInt32(reader["duanweirank"].ToString());
						data.LianSheng = Convert.ToInt32(reader["liansheng"].ToString());
						data.FightCount = Convert.ToInt32(reader["fightcount"].ToString());
						data.SuccessCount = Convert.ToInt32(reader["successcount"].ToString());
						data.LastFightTime = Convert.ToDateTime(reader["lastfighttime"].ToString());
						data.MonthDuanWeiRank = Convert.ToInt32(reader["monthduanweirank"].ToString());
						data.LeaderRoleName = reader["leaderrolename"].ToString();
						data.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
						list.Add(data);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			return result;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0005860C File Offset: 0x0005680C
		public void UpdateZhanDuiDayRank(List<TianTi5v5ZhanDuiData> list, int dayId, int maxRankCount, bool updateMonthRank)
		{
			try
			{
				this.AddDelayWriteSql("delete from t_kf_5v5_zhandui_paihang where dayid=" + dayId, null, null);
				int i = 0;
				while (i < maxRankCount && i < list.Count)
				{
					TianTi5v5ZhanDuiData data = list[i];
					string sql = string.Format("REPLACE INTO t_kf_5v5_zhandui_paihang (`dayid`,`zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`leaderrolename`,`zoneid`,`zhanli`,`data1`,`data2`,`duanweiid`,`duanweijifen`,`duanweirank`,`monthduanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`) VALUES({15},{0},{1},'{2}','{3}','{4}',{5},{6},@data1,@data2,{7},{8},{9},{10},{11},{12},{13},'{14}');", new object[]
					{
						data.ZhanDuiID,
						data.LeaderRoleID,
						data.XuanYan,
						data.ZhanDuiName,
						data.LeaderRoleName,
						data.ZoneID,
						data.ZhanDouLi,
						data.DuanWeiId,
						data.DuanWeiJiFen,
						data.DuanWeiRank,
						data.MonthDuanWeiRank,
						data.LianSheng,
						data.FightCount,
						data.SuccessCount,
						data.LastFightTime,
						dayId
					});
					this.AddDelayWriteSql(sql, new List<Tuple<string, byte[]>>
					{
						new Tuple<string, byte[]>("@data1", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList)),
						new Tuple<string, byte[]>("@data2", null)
					}, null);
					i++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x000587D0 File Offset: 0x000569D0
		public void UpdateZhanDuiRankData(List<TianTi5v5ZhanDuiData> list, int dayId, int maxRankCount, bool updateMonthRank)
		{
			try
			{
				if (updateMonthRank)
				{
					this.AddDelayWriteSql(string.Format("update t_kf_5v5_zhandui set `duanweirank`={0},`monthduanweirank`={1}", maxRankCount + 1, maxRankCount + 1), null, null);
					int i = 0;
					while (i < maxRankCount && i < list.Count)
					{
						TianTi5v5ZhanDuiData data = list[i];
						string sql = string.Format("update t_kf_5v5_zhandui set `duanweirank`={1},`monthduanweirank`={2}", data.ZhanDuiID, data.DuanWeiRank, data.MonthDuanWeiRank);
						this.AddDelayWriteSql(sql, null, null);
						i++;
					}
				}
				else
				{
					this.AddDelayWriteSql(string.Format("update t_kf_5v5_zhandui set `duanweirank`={0}", maxRankCount + 1), null, null);
					int i = 0;
					while (i < maxRankCount && i < list.Count)
					{
						TianTi5v5ZhanDuiData data = list[i];
						string sql = string.Format("update t_kf_5v5_zhandui set `duanweirank`={1} where zhanduiid={0}", data.ZhanDuiID, data.DuanWeiRank);
						this.AddDelayWriteSql(sql, null, null);
						i++;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00058918 File Offset: 0x00056B18
		public int InitZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("INSERT INTO t_kf_5v5_zhandui(`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`leaderrolename`,zoneid) VALUES('{1}','{2}','{3}','{4}','{5}',@content,'{6}',{7});", new object[]
			{
				data.ZhanDuiID,
				data.LeaderRoleID,
				data.XuanYan,
				data.ZhanDuiName,
				data.DuanWeiId,
				data.ZhanDouLi,
				data.LeaderRoleName,
				data.ZoneID
			});
			List<Tuple<string, byte[]>> imgList = new List<Tuple<string, byte[]>>();
			imgList.Add(new Tuple<string, byte[]>("@content", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList)));
			LogManager.WriteLog(LogTypes.SQL, sql, null, true);
			return (int)DbHelperMySQL.ExecuteSqlGetIncrement(sql, imgList);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x000589E0 File Offset: 0x00056BE0
		public int UpdateZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set `leaderid`={1},`xuanyan`='{2}',`zhanduiname`='{3}',`duanweiid`='{4}',`zhanli`='{5}',`data1`=@data1,leaderrolename='{6}',`liansheng`='{7}',`fightcount`='{8}',`successcount`='{9}',lastfighttime='{10}',`duanweijifen`='{11}',`duanweirank`='{12}',`monthduanweirank`='{13}' where `zhanduiid`={0};", new object[]
			{
				data.ZhanDuiID,
				data.LeaderRoleID,
				data.XuanYan,
				data.ZhanDuiName,
				data.DuanWeiId,
				data.ZhanDouLi,
				data.LeaderRoleName,
				data.LianSheng,
				data.FightCount,
				data.SuccessCount,
				data.LastFightTime,
				data.DuanWeiJiFen,
				data.DuanWeiRank,
				data.MonthDuanWeiRank
			});
			int result;
			if (!this.AddDelayWriteSql(sql, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("@data1", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList))
			}, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00058AFC File Offset: 0x00056CFC
		public void UpdateZhanDuiXuanYan(long teamId, string xuanyan)
		{
			string sql = string.Format("UPDATE t_kf_5v5_zhandui SET xuanyan='{1}' WHERE zhanduiid={0};", teamId, xuanyan);
			this.AddDelayWriteSql(sql, null, null);
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x00058B28 File Offset: 0x00056D28
		public int DeleteZhanDui(long teamid)
		{
			string sql = string.Format("DELETE FROM t_kf_5v5_zhandui WHERE zhanduiid={0};", teamid);
			LogManager.WriteLog(LogTypes.SQL, sql, null, true);
			return this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00058B5C File Offset: 0x00056D5C
		public int ClearZorkBattleRoleData()
		{
			string sql = string.Format("UPDATE `t_kf_5v5_zhandui_roles` SET `zorkkill`=0;", new object[0]);
			return this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00058B88 File Offset: 0x00056D88
		public void UpdateZorkBattleRoleData(ZorkBattleRoleInfo roleData, bool chgKill = true)
		{
			string sql = string.Format("INSERT INTO `t_kf_5v5_zhandui_roles`(`rid`, rname, zoneid, reborncount, rebornlev, `zorkkill`) VALUES({0},'{1}',{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE reborncount={3}, rebornlev={4}, `zorkkill`=`zorkkill`+{5};", new object[]
			{
				roleData.RoleID,
				roleData.Name,
				roleData.ZoneID,
				roleData.RebornCount,
				roleData.RebornLevel,
				roleData.KillRoleNum
			});
			this.AddDelayWriteSql(sql, null, null);
			if (chgKill)
			{
				sql = string.Format("UPDATE t_kf_5v5_zhandui_roles SET `ranktm_zorkkill`=NOW() WHERE `rid`={0};", roleData.RoleID);
				this.AddDelayWriteSql(sql, null, null);
			}
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00058C2C File Offset: 0x00056E2C
		public int UpdateZorkZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set zorkjifen={1}, zorklastfighttime='{2}' where `zhanduiid`={0};", data.ZhanDuiID, data.ZorkJiFen, data.ZorkLastFightTime.ToString("yyyy-MM-dd HH:mm:ss"));
			int result;
			if (!this.AddDelayWriteSql(sql, null, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00058C88 File Offset: 0x00056E88
		public int LoadZorkSeasonID()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object value = DbHelperMySQL.GetSingle("select value from t_async where id = " + 52);
			return Convert.ToInt32(value);
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00058CD4 File Offset: 0x00056ED4
		public void SaveZorkSeasonID(int seasonID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, seasonID));
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00058CF4 File Offset: 0x00056EF4
		public int LoadZorkTopZhanDui()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object value = DbHelperMySQL.GetSingle("select value from t_async where id = " + 53);
			return Convert.ToInt32(value);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x00058D40 File Offset: 0x00056F40
		public void SaveZorkTopZhanDui(int zhanduiID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 53, zhanduiID));
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00058D60 File Offset: 0x00056F60
		public int LoadZorkTopKiller()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object value = DbHelperMySQL.GetSingle("select value from t_async where id = " + 54);
			return Convert.ToInt32(value);
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00058DAC File Offset: 0x00056FAC
		public void SaveZorkTopKiller(int roleID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 54, roleID));
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00058DCC File Offset: 0x00056FCC
		private string FormatLoadZorkBattleRankSql(int rankType)
		{
			string strSql = "";
			if (rankType == 1)
			{
				strSql = string.Format("SELECT rid a, zorkkill b FROM `t_kf_5v5_zhandui_roles` WHERE zorkkill<>0 ORDER BY `zorkkill` DESC, `ranktm_zorkkill` ASC LIMIT {0};", 30);
			}
			return strSql;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00058E04 File Offset: 0x00057004
		public bool LoadZorkBattleRankInfo(int rankType, List<KFZorkRankInfo> rankList)
		{
			try
			{
				string strSql = this.FormatLoadZorkBattleRankSql(rankType);
				if (string.IsNullOrEmpty(strSql))
				{
					return false;
				}
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				while (sdr != null && sdr.Read())
				{
					KFZorkRankInfo rankInfo = new KFZorkRankInfo();
					rankInfo.Key = Convert.ToInt32(sdr["a"]);
					rankInfo.Value = Convert.ToInt32(sdr["b"]);
					if (rankType == 1)
					{
						string strParam = string.Format("SELECT zoneid,rname,rebornlev FROM `t_kf_5v5_zhandui_roles` WHERE rid={0};", rankInfo.Key);
						object[] arr;
						if (DbHelperMySQL.GetSingleValues(strParam, out arr) >= 0)
						{
							rankInfo.StrParam1 = KuaFuServerManager.FormatName(Convert.ToInt32(arr[0]), arr[1].ToString());
							rankInfo.Param1 = Convert.ToInt32(arr[2]);
						}
						rankList.Add(rankInfo);
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

		// Token: 0x06000678 RID: 1656 RVA: 0x00058F38 File Offset: 0x00057138
		public int UpdateEscapeZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set escapejifen={1}, escapelastfighttime='{2}' where `zhanduiid`={0};", data.ZhanDuiID, data.EscapeJiFen, data.EscapeLastFightTime.ToString("yyyy-MM-dd HH:mm:ss"));
			int result;
			if (!this.AddDelayWriteSql(sql, null, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00058F94 File Offset: 0x00057194
		public static int CompareKFEscapeRankInfo(KFEscapeRankInfo x, KFEscapeRankInfo y)
		{
			int ret;
			if (x != null && x != y)
			{
				ret = y.Value - x.Value;
				if (ret == 0)
				{
					ret = x.Key - y.Key;
				}
			}
			else if (x == null)
			{
				ret = 1;
			}
			else
			{
				ret = -1;
			}
			return ret;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00058FFC File Offset: 0x000571FC
		public List<KFEscapeRankInfo> LoadEscapeRankData(int season)
		{
			List<KFEscapeRankInfo> list = new List<KFEscapeRankInfo>();
			try
			{
				string strSql = string.Format("SELECT zhanduiid,zhanduiname,zoneid,duanweijifen,zhanli FROM t_zhandui_escape_paihang e where season={0}", season);
				using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
				{
					while (sdr != null && sdr.Read())
					{
						list.Add(new KFEscapeRankInfo
						{
							Key = Convert.ToInt32(sdr["zhanduiid"]),
							ZoneID = Convert.ToInt32(sdr["zoneid"]),
							StrParam1 = sdr["zhanduiname"].ToString(),
							Value = Convert.ToInt32(sdr["duanweijifen"]),
							Param1 = (long)Convert.ToInt32(sdr["zhanli"])
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			list.Sort(new Comparison<KFEscapeRankInfo>(TianTiPersistence.CompareKFEscapeRankInfo));
			return list;
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x00059128 File Offset: 0x00057328
		public int GetAsyncInt(int id, int value)
		{
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", id, value));
				return (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 14);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return -1;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x0005919C File Offset: 0x0005739C
		public bool SetAsyncInt(int id, int value)
		{
			try
			{
				string sql = string.Format("update t_async set value={1} where id = {0}", id, value);
				return DbHelperMySQL.ExecuteSql(sql) >= 0;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x000591FC File Offset: 0x000573FC
		public bool BuildEscapeRankList(int offsetDay, DateTime minFightTime)
		{
			try
			{
				string repSql = string.Format("REPLACE INTO t_zhandui_escape_paihang SELECT {0},zhanduiid,zhanduiname,zoneid,escapejifen,zhanli FROM t_kf_5v5_zhandui WHERE escapelastfighttime>'{1}' ORDER BY duanweijifen DESC, zhanduiid;", offsetDay, minFightTime.ToString("yyyy-MM-dd"));
				return DbHelperMySQL.ExecuteSql(repSql) >= 0;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00059260 File Offset: 0x00057460
		private List<EscapeBattleZhanDuiData> LoadZhanDuiData()
		{
			List<EscapeBattleZhanDuiData> list = new List<EscapeBattleZhanDuiData>();
			try
			{
				string strSql = string.Format("SELECT season,e.zhanduiid,zhanduiname,zoneid,e.duanweiid,e.duanweijifen,e.duanweirank,zhanli FROM t_zhandui_escape e left join t_kf_5v5_zhandui z on e.zhanduiid=z.zhanduiid order by e.duanweijifen desc,e.zhanduiid", new object[0]);
				using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
				{
					while (sdr != null && sdr.Read())
					{
						list.Add(new EscapeBattleZhanDuiData
						{
							Season = Convert.ToInt32(sdr["season"]),
							ZhanDuiID = Convert.ToInt32(sdr["zhanduiid"]),
							ZoneId = Convert.ToInt32(sdr["zoneid"]),
							ZhanDuiName = sdr["zhanduiname"].ToString(),
							DuanWeiId = Convert.ToInt32(sdr["duanweiid"]),
							DuanWeiRank = Convert.ToInt32(sdr["duanweirank"]),
							DuanWeiJiFen = Convert.ToInt32(sdr["duanweijifen"]),
							ZhanLi = Convert.ToInt64(sdr["zhanli"])
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return list;
		}

		// Token: 0x0400038E RID: 910
		public static readonly TianTiPersistence Instance = new TianTiPersistence();

		// Token: 0x0400038F RID: 911
		public object Mutex = new object();

		// Token: 0x04000390 RID: 912
		public object MutexPaiHang = new object();

		// Token: 0x04000391 RID: 913
		public bool Initialized = false;

		// Token: 0x04000392 RID: 914
		public int SignUpWaitSecs1 = 5;

		// Token: 0x04000393 RID: 915
		public int SignUpWaitSecs3 = 10;

		// Token: 0x04000394 RID: 916
		public int SignUpWaitSecsAll = 15;

		// Token: 0x04000395 RID: 917
		public int WaitForJoinMaxSecs = 60;

		// Token: 0x04000396 RID: 918
		private int MaxSendDetailDataCount = 100;

		// Token: 0x04000397 RID: 919
		public int MaxRolePairFightCount = 3;

		// Token: 0x04000398 RID: 920
		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();

		// Token: 0x04000399 RID: 921
		public TianTiRankData RankData = new TianTiRankData();

		// Token: 0x0400039A RID: 922
		public ConcurrentQueue<TianTiRoleInfoData> TianTiRoleInfoDataQueue = new ConcurrentQueue<TianTiRoleInfoData>();

		// Token: 0x0400039B RID: 923
		public Queue<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>> DelayWriteSqlQueue = new Queue<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>>();

		// Token: 0x0400039C RID: 924
		public bool ServerStopping;

		// Token: 0x0400039D RID: 925
		private int CurrGameId = Global.UninitGameId;
	}
}
