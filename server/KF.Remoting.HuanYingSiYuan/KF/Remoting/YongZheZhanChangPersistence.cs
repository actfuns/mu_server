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
	
	public class YongZheZhanChangPersistence
	{
		
		private YongZheZhanChangPersistence()
		{
		}

		
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

		
		public void UpdateRoleInfoData(object data)
		{
			if (this.YongZheZhanChangRoleInfoDataQueue.Count > 100000)
			{
				object tmpData;
				this.YongZheZhanChangRoleInfoDataQueue.TryDequeue(out tmpData);
			}
			this.YongZheZhanChangRoleInfoDataQueue.Enqueue(data);
		}

		
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

		
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		
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

		
		public static readonly YongZheZhanChangPersistence Instance = new YongZheZhanChangPersistence();

		
		public object Mutex = new object();

		
		private int CurrGameId = Global.UninitGameId;

		
		public bool Initialized = false;

		
		private int MaxPaiMingRank = 100;

		
		public int KuaFuBossRoleCount = 50;

		
		public int YongZheZhanChangRoleCount = 100;

		
		public int KingOfBattleRoleCount = 40;

		
		public int MinEnterCount = 100;

		
		public int MaxServerCapcity = 30;

		
		public int ServerCapacityRate = 1;

		
		public int LangHunLingYuServerCapacityRate = 5;

		
		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();

		
		public ConcurrentQueue<object> YongZheZhanChangRoleInfoDataQueue = new ConcurrentQueue<object>();

		
		public bool LangHunLingYuInitialized = false;

		
		public TimeSpan LangHunLingYuResetCityTime;

		
		public DateTime LastLangHunLingYuResetCityTime;

		
		public int OtherListUpdateOffsetDay = 0;

		
		public Dictionary<int, CityLevelInfo> CityLevelInfoDict = new Dictionary<int, CityLevelInfo>();

		
		public DateTime LastLangHunLingYuBroadcastTime;

		
		public Dictionary<int, int> LangHunLingYuBroadcastServerIdHashSet = new Dictionary<int, int>();
	}
}
