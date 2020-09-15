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
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x0200008A RID: 138
	public class YongZheZhanChangPersistence
	{
		// Token: 0x06000729 RID: 1833 RVA: 0x0005EF2C File Offset: 0x0005D12C
		private YongZheZhanChangPersistence()
		{
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0005EFD8 File Offset: 0x0005D1D8
		public void InitConfig()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load("config.xml");
				this.YongZheZhanChangRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "MinEnterCount", "value", 100L);
				this.KuaFuBossRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "KuaFuBossRoleCount", "value", 50L);
				this.KingOfBattleRoleCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "KingOfBattleRoleCount", "value", 40L);
				string strLangHunLingYuResetCityTime = ConfigHelper.GetElementAttributeValue(xmlFile, "add", "key", "LangHunLingYuResetCityTime", "value", "");
				if (string.IsNullOrEmpty(strLangHunLingYuResetCityTime) || !TimeSpan.TryParse(strLangHunLingYuResetCityTime, out this.LangHunLingYuResetCityTime))
				{
					this.LangHunLingYuResetCityTime = TimeSpan.MaxValue;
				}
				int MaxRoleCount = Math.Max(this.YongZheZhanChangRoleCount, this.KuaFuBossRoleCount);
				MaxRoleCount = Math.Max(MaxRoleCount, this.KingOfBattleRoleCount);
				this.MaxServerCapcity = Math.Max(3000 / MaxRoleCount, 30);
				this.InitLangHunLingYuConfig();
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(id),0) FROM t_yongzhezhanchang_game_fuben;"));
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0005F14C File Offset: 0x0005D34C
		private void InitLangHunLingYuConfig()
		{
			string fileName = "";
			lock (this.Mutex)
			{
				try
				{
					Dictionary<int, CityLevelInfo> cityLevelInfoDict = new Dictionary<int, CityLevelInfo>();
					fileName = "Config/MU_City.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement t in nodes)
					{
						string type = ConfigHelper.GetElementAttributeValue(t, "TypeID", "");
						if (string.Compare(type, KuaFuServerManager.platformType.ToString(), true) == 0)
						{
							foreach (XElement node in t.Elements())
							{
								CityLevelInfo item = new CityLevelInfo();
								item.ID = (int)ConfigHelper.GetElementAttributeValueLong(node, "ID", 0L);
								item.CityLevel = (int)ConfigHelper.GetElementAttributeValueLong(node, "CityLevel", 0L);
								item.CityNum = (int)ConfigHelper.GetElementAttributeValueLong(node, "CityNum", 0L);
								item.MaxNum = (int)ConfigHelper.GetElementAttributeValueLong(node, "MaxNum", 0L);
								string strAttackWeekDay = ConfigHelper.GetElementAttributeValue(node, "AttackWeekDay", "");
								item.AttackWeekDay = ConfigHelper.String2IntArray(strAttackWeekDay, ',');
								if (!ConfigHelper.ParserTimeRangeListWithDay(item.BaoMingTime, ConfigHelper.GetElementAttributeValue(node, "BaoMingTime", "").Replace(';', '|'), true, '|', '-', ','))
								{
									LogManager.WriteLog(LogTypes.Fatal, string.Format("解析文件{0}的BaoMingTime出错", fileName), null, true);
									KuaFuServerManager.LoadConfigSuccess = false;
								}
								if (!ConfigHelper.ParserTimeRangeList(item.AttackTime, ConfigHelper.GetElementAttributeValue(node, "AttackTime", ""), true, '|', '-'))
								{
									LogManager.WriteLog(LogTypes.Fatal, string.Format("解析文件{0}的BaoMingTime出错", fileName), null, true);
									KuaFuServerManager.LoadConfigSuccess = false;
								}
								cityLevelInfoDict[item.CityLevel] = item;
							}
							break;
						}
					}
					this.CityLevelInfoDict = cityLevelInfoDict;
					if (this.CityLevelInfoDict.Count == 0)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("读取配置{0}失败,读取到的城池配置数为0", new object[0]), null, true);
						KuaFuServerManager.LoadConfigSuccess = false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()), null, true);
					KuaFuServerManager.LoadConfigSuccess = false;
				}
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0005F480 File Offset: 0x0005D680
		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(LogTypes.Warning, "YongZheZhanChang 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0005F4D4 File Offset: 0x0005D6D4
		public void UpdateRoleInfoData(object data)
		{
			if (this.YongZheZhanChangRoleInfoDataQueue.Count > 100000)
			{
				object tmpData;
				this.YongZheZhanChangRoleInfoDataQueue.TryDequeue(out tmpData);
			}
			this.YongZheZhanChangRoleInfoDataQueue.Enqueue(data);
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0005F518 File Offset: 0x0005D718
		public void WriteRoleInfoDataToDb(object obj)
		{
			try
			{
				string sql = "";
				if (obj is YongZheZhanChangStatisticalData)
				{
					YongZheZhanChangStatisticalData data = obj as YongZheZhanChangStatisticalData;
					sql = string.Format("INSERT INTO t_yongzhezhanchang_fuben_log(gameid,allrolecount,winrolecount,loserolecount,lianshascore,zhongjiescore,caijiscore,bossscore,killscore,gametime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},now())", new object[]
					{
						data.GameId,
						data.AllRoleCount,
						data.WinRoleCount,
						data.LoseRoleCount,
						data.LianShaScore,
						data.ZhongJieScore,
						data.CaiJiScore,
						data.BossScore,
						data.KillScore
					});
				}
				else if (obj is KingOfBattleStatisticalData)
				{
					KingOfBattleStatisticalData data2 = obj as KingOfBattleStatisticalData;
					sql = string.Format("INSERT INTO t_kingofbattle_fuben_log(gameid,allrolecount,winrolecount,loserolecount,lianshascore,zhongjiescore,caijiscore,bossscore,killscore,gametime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},now())", new object[]
					{
						data2.GameId,
						data2.AllRoleCount,
						data2.WinRoleCount,
						data2.LoseRoleCount,
						data2.LianShaScore,
						data2.ZhongJieScore,
						data2.CaiJiScore,
						data2.BossScore,
						data2.KillScore
					});
				}
				else if (obj is KuaFuBossStatisticalData)
				{
					KuaFuBossStatisticalData data3 = obj as KuaFuBossStatisticalData;
					for (int i = 0; i < data3.MonsterDieTimeList.Count - 1; i += 2)
					{
						sql += string.Format("INSERT INTO t_kuafu_boss_log VALUES({0},{1},{2});", data3.GameId, data3.MonsterDieTimeList[i], data3.MonsterDieTimeList[i + 1]);
					}
				}
				else if (obj is GameLogItem)
				{
					GameLogItem data4 = obj as GameLogItem;
					sql = string.Format("INSERT INTO t_yongzhezhanchang_role_statistics_log(servercount,fubencount,signupcount,entercount,gametime) VALUES({0},{1},{2},{3},now());", new object[]
					{
						data4.ServerCount,
						data4.FubenCount,
						data4.SignUpCount,
						data4.EnterCount
					});
				}
				if (!string.IsNullOrEmpty(sql))
				{
					DbHelperMySQL.ExecuteSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0005F7EC File Offset: 0x0005D9EC
		public void WriteRoleInfoDataProc()
		{
			for (int i = 0; i < 1000; i++)
			{
				object data;
				if (!this.YongZheZhanChangRoleInfoDataQueue.TryDequeue(out data))
				{
					break;
				}
				this.WriteRoleInfoDataToDb(data);
			}
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0005F82C File Offset: 0x0005DA2C
		private bool ExecuteSqlNoQuery(string sqlCmd)
		{
			bool result;
			try
			{
				DbHelperMySQL.ExecuteSql(sqlCmd);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			return result;
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0005F86C File Offset: 0x0005DA6C
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0005F88C File Offset: 0x0005DA8C
		public void LogCreateYongZheFuBen(int kfSrvId, int gameId, int fubenSeq, int roleNum)
		{
			string sql = string.Format("INSERT INTO t_yongzhezhanchang_game_fuben(`id`,`serverid`,`fubensid`,`createtime`,`rolenum`) VALUES({0},{1},{2},'{3}',{4});", new object[]
			{
				gameId,
				kfSrvId,
				fubenSeq,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
				roleNum
			});
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x0005F8F0 File Offset: 0x0005DAF0
		public void SaveCityData(LangHunLingYuCityDataEx cityDataEx)
		{
			this.ExecuteSqlNoQuery(string.Format("INSERT INTO t_lhly_city(cityid,citylevel,owner,attacker1,attacker2,attacker3) VALUES({0},{1},{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE citylevel={1},owner={2},attacker1={3},attacker2={4},attacker3={5};", new object[]
			{
				cityDataEx.CityId,
				cityDataEx.CityLevel,
				cityDataEx.Site[0],
				cityDataEx.Site[1],
				cityDataEx.Site[2],
				cityDataEx.Site[3]
			}));
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0005F974 File Offset: 0x0005DB74
		public void SaveBangHuiData(LangHunLingYuBangHuiDataEx bangHuiDataEx)
		{
			this.ExecuteSqlNoQuery(string.Format("INSERT INTO t_lhly_banghui(bhid,bhname,zoneid,level) VALUES({0},'{1}',{2},{3}) ON DUPLICATE KEY UPDATE bhname='{1}',zoneid={2},level={3};", new object[]
			{
				bangHuiDataEx.Bhid,
				bangHuiDataEx.BhName,
				bangHuiDataEx.ZoneId,
				bangHuiDataEx.Level
			}));
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0005F9D0 File Offset: 0x0005DBD0
		public bool LoadCityOwnerHistory(List<LangHunLingYuKingHist> LHLYCityOwnerList)
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT * FROM `t_lhly_hist`", new object[0]);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					LangHunLingYuKingHist OwnerData = new LangHunLingYuKingHist();
					OwnerData.rid = Convert.ToInt32(sdr["role_id"]);
					OwnerData.AdmireCount = Convert.ToInt32(sdr["admire_count"]);
					OwnerData.CompleteTime = DateTime.Parse(sdr["time"].ToString());
					if (!sdr.IsDBNull(sdr.GetOrdinal("data")))
					{
						OwnerData.CityOwnerRoleData = (byte[])sdr["data"];
					}
					LHLYCityOwnerList.Add(OwnerData);
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

		// Token: 0x06000736 RID: 1846 RVA: 0x0005FAF0 File Offset: 0x0005DCF0
		public void InsertCityOwnerHistory(LangHunLingYuKingHist CityOwnerData)
		{
			try
			{
				string sql = string.Format("INSERT INTO t_lhly_hist(role_id, admire_count, time, data) VALUES({0}, {1}, '{2}', @content)", CityOwnerData.rid, CityOwnerData.AdmireCount, CityOwnerData.CompleteTime.ToString());
				DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", CityOwnerData.CityOwnerRoleData)
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0005FB7C File Offset: 0x0005DD7C
		public void AdmireCityOwner(int rid)
		{
			try
			{
				string sql = string.Format("UPDATE t_lhly_hist SET admire_count=admire_count+1 WHERE role_id={0}", rid);
				DbHelperMySQL.ExecuteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0005FBC8 File Offset: 0x0005DDC8
		public bool LoadBangHuiDataExList(List<LangHunLingYuBangHuiDataEx> list)
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT * FROM `t_lhly_banghui`;", new object[0]);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new LangHunLingYuBangHuiDataEx
					{
						Bhid = Convert.ToInt32(sdr["bhid"]),
						ZoneId = Convert.ToInt32(sdr["zoneid"]),
						BhName = sdr["bhname"].ToString()
					});
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

		// Token: 0x06000739 RID: 1849 RVA: 0x0005FCB0 File Offset: 0x0005DEB0
		public bool LoadCityDataExList(List<LangHunLingYuCityDataEx> list)
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT * FROM `t_lhly_city`;", new object[0]);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					LangHunLingYuCityDataEx data = new LangHunLingYuCityDataEx();
					data.CityId = Convert.ToInt32(sdr["cityid"]);
					data.CityLevel = Convert.ToInt32(sdr["citylevel"]);
					data.Site[0] = (long)Convert.ToInt32(sdr["owner"].ToString());
					data.Site[1] = (long)Convert.ToInt32(sdr["attacker1"].ToString());
					data.Site[2] = (long)Convert.ToInt32(sdr["attacker2"].ToString());
					data.Site[3] = (long)Convert.ToInt32(sdr["attacker3"].ToString());
					list.Add(data);
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

		// Token: 0x0600073A RID: 1850 RVA: 0x0005FE18 File Offset: 0x0005E018
		public int GetKuaFoWorldMaxTempRoleID(out int limit)
		{
			int asyncID = 50;
			int baseID = (int)DbHelperMySQL.GetSingleLong("select value from t_async where id = " + asyncID);
			if (baseID < 0)
			{
				baseID = 0;
				this.ExecuteSqlNoQuery(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", asyncID, baseID));
			}
			limit = baseID + 199980;
			string sql = string.Format("select max(temprid) from `t_pt_roles` where temprid>={0} and temprid<={1}", baseID, baseID + 199999);
			int maxTempRoleID = (int)DbHelperMySQL.GetSingleLong(sql);
			if (maxTempRoleID <= 0)
			{
				maxTempRoleID = 0;
			}
			return maxTempRoleID;
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0005FEB4 File Offset: 0x0005E0B4
		public KuaFuWorldRoleData QueryKuaFuWorldRoleData(int roleID, int ptid)
		{
			KuaFuWorldRoleData data = null;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT ptid,rid,temprid,userid,zoneid,channel,roledata FROM `t_pt_roles` where ptid={0} and rid={1}", ptid, roleID);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				if (sdr.Read())
				{
					data = new KuaFuWorldRoleData();
					data.PTID = Convert.ToInt32(sdr["ptid"].ToString());
					data.LocalRoleID = Convert.ToInt32(sdr["rid"].ToString());
					data.TempRoleID = Convert.ToInt32(sdr["temprid"].ToString());
					data.UserID = sdr["userid"].ToString();
					data.ZoneID = Convert.ToInt32(sdr["zoneid"].ToString());
					data.Channel = sdr["channel"].ToString();
					data.RoleData = (sdr["roledata"] as byte[]);
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
			return data;
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00060018 File Offset: 0x0005E218
		public KuaFuWorldRoleData InsertKuaFuWorldRoleData(KuaFuWorldRoleData data, int tempRoleID)
		{
			try
			{
				object[] ptidAndRid;
				int result = DbHelperMySQL.GetSingleValues(string.Format("SELECT ptid,rid FROM `t_pt_roles` where temprid={0}", data.LocalRoleID), out ptidAndRid);
				bool flag;
				if (result >= 0)
				{
					flag = !ptidAndRid.All((object x) => x != null);
				}
				else
				{
					flag = true;
				}
				if (!flag)
				{
					if (!(data.PTID.ToString() != ptidAndRid[0].ToString()))
					{
						return this.QueryKuaFuWorldRoleData(data.LocalRoleID, data.PTID);
					}
					data.TempRoleID = tempRoleID;
				}
				else
				{
					data.TempRoleID = data.LocalRoleID;
				}
				List<Tuple<string, byte[]>> imgList = new List<Tuple<string, byte[]>>();
				imgList.Add(new Tuple<string, byte[]>("content", data.RoleData));
				string strSql = string.Format("insert into `t_pt_roles`(ptid,rid,temprid,userid,zoneid,channel,roledata) values('{0}','{1}','{2}','{3}','{4}','{5}',@content)", new object[]
				{
					data.PTID,
					data.LocalRoleID,
					data.TempRoleID,
					data.UserID,
					data.ZoneID,
					data.Channel
				});
				result = DbHelperMySQL.ExecuteSqlInsertImg(strSql, imgList);
				if (result >= 0)
				{
					return data;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x000601B0 File Offset: 0x0005E3B0
		public int WriteKuaFuWorldRoleData(KuaFuWorldRoleData data)
		{
			int result = -15;
			try
			{
				string strSql = string.Format("update `t_pt_roles` set roledata=@content where ptid={0} and rid={1}", data.PTID, data.PTID);
				result = DbHelperMySQL.ExecuteSqlInsertImg(strSql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", data.RoleData)
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		// Token: 0x040003D7 RID: 983
		public static readonly YongZheZhanChangPersistence Instance = new YongZheZhanChangPersistence();

		// Token: 0x040003D8 RID: 984
		public object Mutex = new object();

		// Token: 0x040003D9 RID: 985
		private int CurrGameId = Global.UninitGameId;

		// Token: 0x040003DA RID: 986
		public bool Initialized = false;

		// Token: 0x040003DB RID: 987
		private int MaxPaiMingRank = 100;

		// Token: 0x040003DC RID: 988
		public int KuaFuBossRoleCount = 50;

		// Token: 0x040003DD RID: 989
		public int YongZheZhanChangRoleCount = 100;

		// Token: 0x040003DE RID: 990
		public int KingOfBattleRoleCount = 40;

		// Token: 0x040003DF RID: 991
		public int MinEnterCount = 100;

		// Token: 0x040003E0 RID: 992
		public int MaxServerCapcity = 30;

		// Token: 0x040003E1 RID: 993
		public int ServerCapacityRate = 1;

		// Token: 0x040003E2 RID: 994
		public int LangHunLingYuServerCapacityRate = 5;

		// Token: 0x040003E3 RID: 995
		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();

		// Token: 0x040003E4 RID: 996
		public ConcurrentQueue<object> YongZheZhanChangRoleInfoDataQueue = new ConcurrentQueue<object>();

		// Token: 0x040003E5 RID: 997
		public bool LangHunLingYuInitialized = false;

		// Token: 0x040003E6 RID: 998
		public TimeSpan LangHunLingYuResetCityTime;

		// Token: 0x040003E7 RID: 999
		public DateTime LastLangHunLingYuResetCityTime;

		// Token: 0x040003E8 RID: 1000
		public int OtherListUpdateOffsetDay = 0;

		// Token: 0x040003E9 RID: 1001
		public Dictionary<int, CityLevelInfo> CityLevelInfoDict = new Dictionary<int, CityLevelInfo>();

		// Token: 0x040003EA RID: 1002
		public DateTime LastLangHunLingYuBroadcastTime;

		// Token: 0x040003EB RID: 1003
		public Dictionary<int, int> LangHunLingYuBroadcastServerIdHashSet = new Dictionary<int, int>();
	}
}
