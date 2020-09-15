using System;
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
	// Token: 0x02000024 RID: 36
	public class JunTuanPersistence
	{
		// Token: 0x06000123 RID: 291 RVA: 0x0000F4E4 File Offset: 0x0000D6E4
		private JunTuanPersistence()
		{
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000F58C File Offset: 0x0000D78C
		public void InitConfig()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load("config.xml");
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(juntuanid),0) FROM t_juntuan;"));
				}
				string fileName = "";
				lock (this.RuntimeData.Mutex)
				{
					try
					{
						this.RuntimeData.LegionsNeed = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsNeed", -1);
						this.RuntimeData.LegionsCreateCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsCreateCD", -1);
						this.RuntimeData.LegionsCastZuanShi = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsCastZuanShi", -1);
						this.RuntimeData.LegionsJionCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsJionCD", -1);
						this.RuntimeData.LegionsEliteNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LegionsEliteNum", -1);
						this.RuntimeData.LegionProsperityCost = KuaFuServerManager.systemParamsList.GetParamValueIntArrayByName("LegionProsperityCost");
						string timeStr = KuaFuServerManager.systemParamsList.GetParamValueByName("LegionTasksTime");
						if (!ConfigHelper.ParserTimeRangeListWithDay2(this.RuntimeData.TaskStartEndTimeList, timeStr, true, '|', '-', ',') || this.RuntimeData.TaskStartEndTimeList.Count != 2)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("解析systemparams.xml的LegionTasksTime出错", fileName), null, true);
						}
						fileName = "Config/LegionsManager.xml";
						string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.RolePermissionDict.Load(fullPathFileName, null);
						fileName = "Config/LegionTasks.xml";
						fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.TaskList.Load(fullPathFileName, null);
						fileName = "Config/LegionsWar.xml";
						fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
						this.RuntimeData.KarenBattleMapList.Load(fullPathFileName, null);
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
					}
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000F810 File Offset: 0x0000DA10
		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(LogTypes.Warning, "JunTuan 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000F864 File Offset: 0x0000DA64
		public bool LoadDatabase()
		{
			YaoSaiService.Instance().LoadYaoSaiData();
			JunTuanEraService.Instance().LoadJunTuanEraData();
			int weekDayId = TimeUtil.GetWeekStartDayIdNow();
			List<JunTuanData> list = new List<JunTuanData>();
			if (!this.LoadJunTuanDataList(list))
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载军团数据失败", null, true);
			}
			foreach (JunTuanData junTuanData in list)
			{
				List<int> bhIdList = new List<int>();
				List<JunTuanBangHuiData> bhDataList = new List<JunTuanBangHuiData>();
				this.LoadJunTuanBangHuiList(bhDataList, bhIdList, junTuanData.JunTuanId);
				if (bhIdList.Count == 0)
				{
					this.DeleteJunTuan(junTuanData.JunTuanId);
				}
				else
				{
					List<JunTuanTaskData> taskDataList = new List<JunTuanTaskData>();
					this.LoadJunTuanTaskList(taskDataList, weekDayId, junTuanData.JunTuanId);
					List<JunTuanEventLog> logList = new List<JunTuanEventLog>();
					this.LoadJunTuanLogList(logList, junTuanData.JunTuanId);
					List<JunTuanRequestData> requestDataList = new List<JunTuanRequestData>();
					this.LoadJunTuanRequestList(requestDataList, junTuanData.JunTuanId);
					int junTuanId = junTuanData.JunTuanId;
					JunTuanDetailData detailData;
					if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						detailData = new JunTuanDetailData
						{
							JunTuanId = junTuanId
						};
						this.JunTuanAllDataDict[junTuanId] = detailData;
					}
					detailData.JunTuanData.V = junTuanData;
					detailData.JunTuanTaskAllData.V.JunTuanId = junTuanId;
					detailData.JunTuanTaskAllData.V.TaskList = taskDataList;
					detailData.JunTuanBangHuiList.V = bhDataList;
					detailData.JunTuanBaseData.V.BhList = bhIdList;
					detailData.JoinDataList.V = requestDataList;
					detailData.EventLogList.V = logList;
					detailData.JunTuanData.Age = 1L;
					detailData.JunTuanRoleDataList.Age = 1L;
					detailData.JunTuanTaskAllData.Age = 1L;
					detailData.JunTuanBangHuiList.Age = 1L;
					detailData.JoinDataList.Age = 1L;
					detailData.EventLogList.Age = 1L;
					foreach (JunTuanRequestData data in requestDataList)
					{
						JunTuanBangHuiData bhData;
						if (!this.JunTuanBangHuiDataDict.TryGetValue(data.BhId, out bhData))
						{
							bhData = new JunTuanBangHuiData();
							bhData.BhId = data.BhId;
							this.JunTuanBangHuiDataDict[bhData.BhId] = bhData;
							bhData.BhName = data.BhName;
							bhData.BhZoneId = data.BhZoneId;
							bhData.LeaderName = data.LeaderName;
							bhData.LeaderZoneId = data.LeaderZoneId;
							bhData.RoleNum = data.RoleNum;
							bhData.ZhanLi = data.ZhanLi;
							bhData.LeaderOccupation = data.Occupation;
							bhData.LeaderRoleId = data.LeaderRoleId;
						}
					}
					foreach (JunTuanBangHuiData bhData in bhDataList)
					{
                        this.JunTuanBangHuiDataDict[bhData.BhId] = bhData;
					}
					this.ReloadJunTuanRoleDataList(detailData);
					this.UpdateRequestDataListCmdData(detailData);
					this.UpdateLogDataListCmdData(detailData);
				}
			}
			this.UpdateJunTuanRankDataList();
			this.UpdateCmdData();
			return true;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000FC38 File Offset: 0x0000DE38
		public void ReloadJunTuanRoleDataList(JunTuanDetailData detailData)
		{
			List<JunTuanRoleData> roleDataList = new List<JunTuanRoleData>();
			this.LoadJunTuanRoleList(roleDataList, detailData.JunTuanBaseData.V.BhList, detailData.JunTuanId);
			detailData.JunTuanRoleDataList.V = roleDataList;
			this.JunTuanUpdateBhList(detailData, false, true);
			this.UpdateRoleDataListCmdData(detailData);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000FC88 File Offset: 0x0000DE88
		public void UpdateCmdData()
		{
			this.UpdateJunTuanMiniDataList();
			this.UpdateJunTuanBaseDataList();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000FC9C File Offset: 0x0000DE9C
		public void UpdateRequestDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
			detailData.JunTuanData.V.RequestCount = detailData.JoinDataList.V.Count;
			TimeUtil.AgeByNow(ref detailData.RequestDataListCmdData.Age);
			detailData.RequestDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRequestData>>(detailData.JoinDataList.V);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000FD07 File Offset: 0x0000DF07
		public void UpdateRoleDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.JunTuanRoleDataListCmdData.Age);
			detailData.JunTuanRoleDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRoleData>>(detailData.JunTuanRoleDataList.V);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000FD36 File Offset: 0x0000DF36
		public void UpdateLogDataListCmdData(JunTuanDetailData detailData)
		{
			TimeUtil.AgeByNow(ref detailData.EventLogListCmdData.Age);
			detailData.EventLogListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanEventLog>>(detailData.EventLogList.V);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000FD68 File Offset: 0x0000DF68
		public int JunTuanRankDataComparison(JunTuanRankData x, JunTuanRankData y)
		{
			int result = y.Point - x.Point;
			int result2;
			if (result != 0)
			{
				result2 = result;
			}
			else
			{
				result2 = DateTime.Compare(y.LastTime, x.LastTime);
			}
			return result2;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000FDA8 File Offset: 0x0000DFA8
		public void UpdateJunTuanRankDataList()
		{
			int weekDayId = TimeUtil.GetWeekStartDayIdNow();
			List<JunTuanRankData> rankDataList = new List<JunTuanRankData>();
			if (!this.LoadJunTuanRankList(rankDataList, weekDayId))
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载军团贡献排行数据失败", null, true);
			}
			else
			{
				lock (this.Mutex)
				{
					int oldCount = this.JunTuanRankDataList.V.Count;
					this.JunTuanRankDataList.V.Clear();
					for (int i = 0; i < rankDataList.Count; i++)
					{
						JunTuanRankData data = rankDataList[i];
						if (data.Point > 0)
						{
							data.Rank = i + 1;
							if (i < this.MaxPaiMingRank)
							{
								this.JunTuanRankDataList.V.Add(data);
							}
						}
						int junTuanId = data.JunTuanId;
						JunTuanDetailData detailData;
						if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
						{
							if (detailData.JunTuanTaskAllData.V.TaskPoint != data.Point || detailData.JunTuanTaskAllData.V.TaskLastTime != data.LastTime)
							{
								detailData.JunTuanTaskAllData.V.TaskPoint = data.Point;
								detailData.JunTuanTaskAllData.V.TaskLastTime = data.LastTime;
								TimeUtil.AgeByNow(ref detailData.JunTuanTaskAllData.Age);
							}
							if (detailData.JunTuanData.V.WeekRank != data.Rank || detailData.JunTuanData.V.WeekPoint != data.Point)
							{
								detailData.JunTuanData.V.WeekRank = data.Rank;
								detailData.JunTuanData.V.WeekPoint = data.Point;
								TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
							}
						}
					}
					if (oldCount != 0 || this.JunTuanRankDataList.V.Count != 0)
					{
						TimeUtil.AgeByNow(ref this.JunTuanRankDataList.Age);
						this.JunTuanRankDataListCmdData.Age = this.JunTuanRankDataList.Age;
						this.JunTuanRankDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRankData>>(this.JunTuanRankDataList.V);
					}
				}
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00010058 File Offset: 0x0000E258
		public int AddJunTuanPoint(JunTuanDetailData detailData, int point, bool taskPoint = false)
		{
			int point2;
			lock (this.Mutex)
			{
				detailData.JunTuanData.V.Point += point;
				if (taskPoint)
				{
					detailData.JunTuanData.V.WeekPoint += point;
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				this.UpdateJunTuanPointData(detailData.JunTuanData.V.JunTuanId, detailData.JunTuanData.V.Point);
				point2 = detailData.JunTuanData.V.Point;
			}
			return point2;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0001014C File Offset: 0x0000E34C
		public void JunTuanUpdateBhList(JunTuanDetailData detailData, bool updateAll = true, bool updateLeaderInfo = false)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < detailData.JunTuanBangHuiList.V.Count; i++)
				{
					JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V[i];
					list.Add(bhData.BhId);
					if (i == 0)
					{
						bhData.JuTuanZhiWu = 1;
						detailData.LeaderBhId = bhData.BhId;
					}
					else
					{
						bhData.JuTuanZhiWu = 0;
					}
				}
				if (updateLeaderInfo)
				{
					using (List<JunTuanRoleData>.Enumerator enumerator = detailData.JunTuanRoleDataList.V.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JunTuanRoleData roleData = enumerator.Current;
							if (roleData.JuTuanZhiWu == 1 || roleData.JuTuanZhiWu == 2)
							{
								JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == roleData.BhId);
								if (null != bhData)
								{
									bhData.LeaderOccupation = roleData.Occu;
									bhData.LeaderRoleId = roleData.RoleId;
									bhData.LeaderName = roleData.RoleName;
									bhData.LeaderZoneId = roleData.ZoneId;
								}
								if (roleData.BhId == detailData.LeaderBhId)
								{
									detailData.JunTuanData.V.LeaderZoneId = roleData.ZoneId;
									detailData.JunTuanData.V.LeaderName = roleData.RoleName;
									detailData.JunTuanData.V.LeaderRoleId = roleData.RoleId;
								}
							}
						}
					}
				}
				detailData.JunTuanBaseData.V.BhList = list;
				TimeUtil.AgeByNow(ref detailData.JunTuanBaseData.Age);
				detailData.JunTuanData.V.BangHuiNum = detailData.JunTuanBangHuiList.V.Count;
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiListCmdData.Age);
				detailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(detailData.JunTuanBangHuiList.V);
				if (updateAll)
				{
					this.UpdateJunTuanMiniDataList();
					this.UpdateJunTuanBaseDataList();
				}
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00010448 File Offset: 0x0000E648
		public void UpdateJunTuanMiniDataList()
		{
			lock (this.Mutex)
			{
				List<JunTuanMiniData> list = new List<JunTuanMiniData>();
				foreach (JunTuanDetailData d in this.JunTuanAllDataDict.Values)
				{
					list.Add(new JunTuanMiniData
					{
						JunTuanId = d.JunTuanData.V.JunTuanId,
						JunTuanName = d.JunTuanData.V.JunTuanName,
						LeaderZoneId = d.JunTuanData.V.LeaderZoneId,
						LeaderName = d.JunTuanData.V.LeaderName,
						BangHuiNum = d.JunTuanData.V.BangHuiNum,
						LingDi = d.JunTuanData.V.LingDi
					});
				}
				TimeUtil.AgeByNow(ref this.JunTuanMiniDataListCmdData.Age);
				this.JunTuanMiniDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanMiniData>>(list);
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000105B4 File Offset: 0x0000E7B4
		public void UpdateJunTuanTaskList()
		{
			bool timeout = false;
			bool removeTask = true;
			DateTime now = TimeUtil.NowDateTime();
			TimeSpan ts = TimeUtil.GetTimeOfWeekNow();
			if (ts < this.RuntimeData.TaskStartEndTimeList[0])
			{
				removeTask = false;
			}
			else if (ts > this.RuntimeData.TaskStartEndTimeList[1])
			{
				timeout = true;
			}
			int weekDayId = TimeUtil.GetWeekStartDayIdNow();
			lock (this.Mutex)
			{
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				foreach (JunTuanDetailData detailData in this.JunTuanAllDataDict.Values)
				{
					bool changed = false;
					if (detailData.JunTuanTaskAllData.V.TaskList == null || detailData.JunTuanTaskAllData.V.TaskList.Count == 0)
					{
						List<JunTuanTaskData> taskList = new List<JunTuanTaskData>();
						foreach (JunTuanTaskInfo taskInfo in this.RuntimeData.TaskList.Value.Values)
						{
							JunTuanTaskData taskData = new JunTuanTaskData
							{
								TaskId = taskInfo.ID,
								WeekDay = weekDayId
							};
							taskList.Add(taskData);
							this.UpdateJunTuanTaskData(detailData.JunTuanId, taskData, now, 0);
						}
						detailData.JunTuanTaskAllData.V.TaskList = taskList;
						changed = true;
					}
					List<JunTuanTaskData> removeList = null;
					foreach (JunTuanTaskData taskData in detailData.JunTuanTaskAllData.V.TaskList)
					{
                        if (taskData.WeekDay != weekDayId && removeTask)
						{
							if (null == removeList)
							{
								removeList = new List<JunTuanTaskData>();
							}
							removeList.Add(taskData);
						}
						else if (taskData.TaskState == 0L && timeout)
						{
							taskData.TaskState = 2L;
							this.UpdateJunTuanTaskData(detailData.JunTuanId, taskData);
							changed = true;
						}
					}
					if (null != removeList)
					{
						changed = true;
						foreach (JunTuanTaskData task in removeList)
						{
							detailData.JunTuanTaskAllData.V.TaskList.Remove(task);
						}
					}
					if (changed)
					{
						TimeUtil.AgeByNow(ref detailData.JunTuanTaskAllData.Age);
					}
				}
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0001093C File Offset: 0x0000EB3C
		public void CheckJunTuanBangHuiList()
		{
			long nowTicks = TimeUtil.NOW();
			lock (this.Mutex)
			{
				foreach (JunTuanDetailData detailData in this.JunTuanAllDataDict.Values)
				{
					bool changed = false;
					if (detailData.JunTuanBangHuiList.V != null)
					{
						foreach (JunTuanBangHuiData bhData in detailData.JunTuanBangHuiList.V)
						{
							if (bhData.NextUpdateTicks > 0L && bhData.NextUpdateTicks < nowTicks)
							{
								changed = true;
								bhData.NextUpdateTicks = 0L;
							}
						}
					}
					if (changed)
					{
						TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiListCmdData.Age);
						detailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(detailData.JunTuanBangHuiList.V);
					}
				}
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00010AC8 File Offset: 0x0000ECC8
		public void UpdateJunTuanBaseDataList()
		{
			lock (this.Mutex)
			{
				this.BangHuiJunTuanIdDict.Clear();
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				foreach (JunTuanDetailData d in this.JunTuanAllDataDict.Values)
				{
					JunTuanBaseData data = new JunTuanBaseData
					{
						JunTuanId = d.JunTuanData.V.JunTuanId,
						JunTuanName = d.JunTuanData.V.JunTuanName,
						LingDi = d.JunTuanData.V.LingDi
					};
					data.BhList = new List<int>();
					foreach (JunTuanBangHuiData bh in d.JunTuanBangHuiList.V)
					{
						this.BangHuiJunTuanIdDict[bh.BhId] = data.JunTuanId;
						data.BhList.Add(bh.BhId);
					}
					list.Add(data);
				}
				TimeUtil.AgeByNow(ref this.JunTuanBaseDataListCmdData.Age);
				this.JunTuanBaseDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBaseData>>(list);
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00010C94 File Offset: 0x0000EE94
		public void AddJuntuanEventLog(JunTuanDetailData detailData, JunTuanEventLog logData)
		{
			try
			{
				lock (this.Mutex)
				{
					TimeUtil.AgeByNow(ref detailData.EventLogList.Age);
					detailData.EventLogList.V.Add(logData);
					if (detailData.EventLogList.V.Count > this.MaxLogCount)
					{
						detailData.EventLogList.V.RemoveRange(this.MaxLogCount, detailData.EventLogList.V.Count - this.MaxLogCount);
					}
					this.UpdateLogDataListCmdData(detailData);
				}
				this.AddJunTuanEventLog(detailData.JunTuanId, logData);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00010D84 File Offset: 0x0000EF84
		public void CheckJunTuanPoint()
		{
			DateTime now = TimeUtil.NowDateTime();
			int dayId = TimeUtil.GetOffsetDay2(now);
			lock (this.Mutex)
			{
				List<JunTuanBaseData> list = new List<JunTuanBaseData>();
				List<JunTuanDetailData> deleteList = new List<JunTuanDetailData>();
				foreach (JunTuanDetailData detailData in this.JunTuanAllDataDict.Values)
				{
					if (detailData.JunTuanData.V.PointCostDay < dayId)
					{
						detailData.JunTuanData.V.PointCostDay = dayId;
						this.AddJunTuanPoint(detailData, -this.RuntimeData.LegionProsperityCost[1], false);
						this.AddDelayWriteSql(string.Format("update t_juntuan set pointcostday={1} where juntuanid={0}", detailData.JunTuanId, dayId));
						if (detailData.JunTuanData.V.Point < this.RuntimeData.LegionProsperityCost[3])
						{
							if (!LingDiCaiJiService.Instance().isLingZhu(detailData.JunTuanData.V.JunTuanId))
							{
								deleteList.Add(detailData);
							}
						}
					}
				}
				if (deleteList.Count > 0)
				{
					foreach (JunTuanDetailData detailData in deleteList)
					{
						this.DestroyJunTuan(detailData);
						this.AddJunTuanEventLog(detailData.JunTuanId, new JunTuanEventLog
						{
							EventType = 7,
							Message = "AutoDestroy",
							Time = now
						});
					}
				}
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00010FB8 File Offset: 0x0000F1B8
		private bool LoadJunTuanRankList(List<JunTuanRankData> list, int weekDay)
		{
			MySqlDataReader sdr = null;
			bool result;
			try
			{
				string strSql = string.Format("SELECT j.juntuanid,juntuanname,SUM(IF(t.`taskstate`=1,t.`taskpoint`,0)) AS `point` ,MAX(lasttime) AS lasttime FROM t_juntuan j LEFT JOIN t_juntuan_task t ON j.juntuanid=t.juntuanid WHERE `weekday`={0} GROUP BY juntuanid ORDER BY SUM(t.`taskpoint`) DESC,MAX(lasttime) ASC,juntuanid ASC;", weekDay);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanRankData
					{
						JunTuanId = Convert.ToInt32(sdr[0].ToString()),
						JunTuanName = sdr[1].ToString(),
						Point = (int)Convert.ToInt64(sdr[2].ToString()),
						LastTime = Convert.ToDateTime(sdr[3].ToString())
					});
					index++;
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
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

		// Token: 0x06000137 RID: 311 RVA: 0x000110B8 File Offset: 0x0000F2B8
		public bool LoadJunTuanDataList(List<JunTuanData> list)
		{
			MySqlDataReader sdr = null;
			bool result;
			try
			{
				string strSql = string.Format("SELECT juntuanid,juntuanname,bulletin,zoneid,rname,`point`,pointcostday,lingdi,voice FROM t_juntuan where isdel=0", new object[0]);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanData
					{
						JunTuanId = Convert.ToInt32(sdr[0].ToString()),
						JunTuanName = sdr[1].ToString(),
						Bulletin = sdr[2].ToString(),
						LeaderZoneId = Convert.ToInt32(sdr[3].ToString()),
						LeaderName = sdr[4].ToString(),
						Point = (int)Convert.ToInt64(sdr[5].ToString()),
						PointCostDay = Convert.ToInt32(sdr[6].ToString()),
						LingDi = Convert.ToInt32(sdr[7].ToString()),
						GVoicePrioritys = sdr[7].ToString()
					});
					index++;
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
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

		// Token: 0x06000138 RID: 312 RVA: 0x0001123C File Offset: 0x0000F43C
		private bool LoadJunTuanTaskList(List<JunTuanTaskData> list, int weekDay, int junTuanId)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT taskid,taskv,taskstate,lasttime FROM t_juntuan_task WHERE `weekday`={0} AND juntuanid={1};", weekDay, junTuanId);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanTaskData
					{
						TaskId = Convert.ToInt32(sdr[0]),
						TaskValue = Convert.ToInt32(sdr[1]),
						TaskState = (long)Convert.ToInt32(sdr[2]),
						WeekDay = weekDay
					});
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

		// Token: 0x06000139 RID: 313 RVA: 0x0001132C File Offset: 0x0000F52C
		private bool LoadJunTuanBangHuiList(List<JunTuanBangHuiData> list, List<int> bhIdList, int junTuanId)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT bhid,bhzoneid,bhname,rolenum,zhanli,zhiwu FROM t_juntuan_banghui WHERE juntuanid={0} order by zhiwu desc;", junTuanId);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					JunTuanBangHuiData data = new JunTuanBangHuiData();
					data.BhId = Convert.ToInt32(sdr[0]);
					data.BhZoneId = Convert.ToInt32(sdr[1]);
					data.BhName = sdr[2].ToString();
					data.RoleNum = Convert.ToInt32(sdr[3]);
					data.ZhanLi = Global.SafeConvertToInt64(sdr[4].ToString());
					data.JuTuanZhiWu = Convert.ToInt32(sdr[5]);
					if (data.JuTuanZhiWu > 0)
					{
						bhIdList.Insert(0, data.BhId);
						list.Insert(0, data);
					}
					else
					{
						data.JuTuanZhiWu = 0;
						list.Add(data);
						bhIdList.Add(data.BhId);
					}
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

		// Token: 0x0600013A RID: 314 RVA: 0x000114B0 File Offset: 0x0000F6B0
		public bool LoadJunTuanRoleList(List<JunTuanRoleData> list, List<int> bhidList, int junTuanId)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT rid,rname,zoneid,b.`bhname`,b.`bhzoneid`,r.zhanli,r.zhiwu,zhuansheng,`level`,r.bhid,r.occu FROM t_juntuan_roles r LEFT JOIN t_juntuan_banghui b ON r.bhid=b.bhid WHERE r.bhid in ({0});", string.Join<int>(",", bhidList));
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanRoleData
					{
						RoleId = Convert.ToInt32(sdr[0]),
						RoleName = sdr[1].ToString(),
						ZoneId = Convert.ToInt32(sdr[2]),
						BhName = sdr[3].ToString(),
						BhZoneId = Convert.ToInt32(sdr[4]),
						ZhanLi = (int)Math.Min(Convert.ToInt64(sdr[5]), 2147483647L),
						JuTuanZhiWu = Convert.ToInt32(sdr[6]),
						ChangeLifeCount = Convert.ToInt32(sdr[7]),
						Level = Convert.ToInt32(sdr[8]),
						BhId = Convert.ToInt32(sdr[9]),
						Occu = Convert.ToInt32(sdr[10])
					});
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

		// Token: 0x0600013B RID: 315 RVA: 0x0001165C File Offset: 0x0000F85C
		private bool LoadJunTuanRequestList(List<JunTuanRequestData> list, int junTuanId)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT `bhid`,`juntuanid`,`zoneid`,`bhname`,`zhanli`,`rolenum`,`leadezoneid`,`leadername` FROM `t_juntuan_request` WHERE juntuanid={0} AND `time`>='{1}' ORDER BY `time` DESC;", junTuanId, TimeUtil.NowDateTime().AddDays(-1.0).ToString("yyyy-MM-dd HH:mm:ss"));
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanRequestData
					{
						BhId = Convert.ToInt32(sdr[0]),
						JunTuanId = Convert.ToInt32(sdr[1]),
						BhZoneId = Convert.ToInt32(sdr[2]),
						BhName = sdr[3].ToString(),
						ZhanLi = (long)((int)Convert.ToInt64(sdr[4])),
						RoleNum = Convert.ToInt32(sdr[5]),
						LeaderZoneId = Convert.ToInt32(sdr[6]),
						LeaderName = sdr[7].ToString()
					});
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

		// Token: 0x0600013C RID: 316 RVA: 0x000117E0 File Offset: 0x0000F9E0
		private bool LoadJunTuanLogList(List<JunTuanEventLog> list, int junTuanId)
		{
			bool result = false;
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT `eventtype`,`time`,`message` FROM t_juntuan_log WHERE juntuanid={0} limit {1};", junTuanId, this.MaxLogCount);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					list.Add(new JunTuanEventLog
					{
						EventType = Convert.ToInt32(sdr[0]),
						Time = Convert.ToDateTime(sdr[1].ToString()),
						Message = sdr[2].ToString()
					});
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

		// Token: 0x0600013D RID: 317 RVA: 0x000118D0 File Offset: 0x0000FAD0
		public void AddDelayWriteSql(string sql)
		{
			lock (this.Mutex)
			{
				this.DelayWriteSqlQueue.Enqueue(sql);
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00011924 File Offset: 0x0000FB24
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

		// Token: 0x0600013F RID: 319 RVA: 0x00011974 File Offset: 0x0000FB74
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

		// Token: 0x06000140 RID: 320 RVA: 0x00011A38 File Offset: 0x0000FC38
		public long ExecuteSqlGetIncrement(string sqlCmd)
		{
			long result;
			try
			{
				LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSqlGetIncrement(sqlCmd, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1L;
			}
			return result;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00011A88 File Offset: 0x0000FC88
		public int ExecuteSqlNoQuery(string sqlCmd)
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

		// Token: 0x06000142 RID: 322 RVA: 0x00011AD4 File Offset: 0x0000FCD4
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00011AF4 File Offset: 0x0000FCF4
		public long CreateJunTuan(string junTuanName, string bulletin, int zoneid, string rname, int initPoint, int pointcostday)
		{
			string sql = string.Format("insert  into `t_juntuan`(`juntuanname`,`bulletin`,`zoneid`,`rname`,`point`,`pointcostday`,`lingdi`) values ('{1}','{2}',{3},'{4}',{5},{6},{7});", new object[]
			{
				0,
				junTuanName,
				bulletin,
				zoneid,
				rname,
				initPoint,
				pointcostday,
				0
			});
			return this.ExecuteSqlGetIncrement(sql);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00011B5C File Offset: 0x0000FD5C
		public long AddJunTuanJoinData(JunTuanRequestData data)
		{
			try
			{
				string sql = string.Format("REPLACE INTO `t_juntuan_request` (`bhid`, `juntuanid`, `time`, `state`, `zoneid`, `bhname`, `zhanli`, `rolenum`, `leadezoneid`, `leadername`) VALUES ({0},{1},now(),{2},{3},'{4}',{5},{6},{7},'{8}');", new object[]
				{
					data.BhId,
					data.JunTuanId,
					0,
					data.BhZoneId,
					data.BhName,
					data.ZhanLi,
					data.RoleNum,
					data.LeaderZoneId,
					data.LeaderName
				});
				return (long)this.ExecuteSqlNoQuery(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -1L;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00011C24 File Offset: 0x0000FE24
		public int UpdateJunTuanBangHuiData(JunTuanBangHuiData data, int junTuanId)
		{
			string sql = string.Format("REPLACE INTO `t_juntuan_banghui` (`bhid`, `juntuanid`, `bhzoneid`, `bhname`, `rolenum`, `zhanli`, `zhiwu`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", new object[]
			{
				data.BhId,
				junTuanId,
				data.BhZoneId,
				data.BhName,
				data.RoleNum,
				data.ZhanLi,
				data.JuTuanZhiWu
			});
			return this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00011CA8 File Offset: 0x0000FEA8
		public void UpdateJunTuanPointData(int junTuanId, int point)
		{
			string sql = string.Format("update `t_juntuan` set `point`={0} where juntuanid={1};", point, junTuanId);
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00011CD8 File Offset: 0x0000FED8
		public void AddJunTuanEventLog(int junTuanId, JunTuanEventLog data)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_log` (`juntuanid`,`eventtype`, `time`, `message`) VALUES ({0},{1},'{2}','{3}');", new object[]
			{
				junTuanId,
				data.EventType,
				TimeUtil.DataTimeToString(data.Time, "yyyy-MM-dd HH:mm:ss"),
				data.Message
			});
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00011D34 File Offset: 0x0000FF34
		public void UpdateJunTuanTaskData(int junTuanId, JunTuanTaskData data, DateTime time, int taskPoint)
		{
			string sql = string.Format("INSERT INTO `t_juntuan_task` (`weekday`,`juntuanid`,`taskid`, `taskv`, `taskpoint`, `taskstate`, `lasttime`) VALUES ({0},{1},{2},{3},{4},{5},'{6}') on duplicate key update `taskv`={3}, `taskpoint`={4}, `taskstate`={5}, `lasttime`='{6}';", new object[]
			{
				data.WeekDay,
				junTuanId,
				data.TaskId,
				data.TaskValue,
				taskPoint,
				data.TaskState,
				TimeUtil.DataTimeToString(time, "yyyy-MM-dd HH:mm:ss")
			});
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00011DB8 File Offset: 0x0000FFB8
		public void UpdateJunTuanTaskData(int junTuanId, JunTuanTaskData data)
		{
			string sql = string.Format("update `t_juntuan_task` set taskstate={3},taskv={4} where weekday={0} and juntuanid={1} and taskid={2};", new object[]
			{
				data.WeekDay,
				junTuanId,
				data.TaskId,
				data.TaskState,
				data.TaskValue
			});
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00011E24 File Offset: 0x00010024
		public void DeleteJunTuanRequestData(JunTuanDetailData detailData, JunTuanRequestData data)
		{
			lock (this.Mutex)
			{
				detailData.JoinDataList.V.Remove(data);
				TimeUtil.AgeByNow(ref detailData.RequestDataListCmdData.Age);
				detailData.RequestDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRequestData>>(detailData.JoinDataList.V);
			}
			string sql = string.Format("DELETE FROM t_juntuan_request WHERE bhid={0} AND juntuanid={1};", data.BhId, data.JunTuanId);
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00011ED0 File Offset: 0x000100D0
		public void UpdateJunTuanLingDi(int junTuanId, int lingDi)
		{
			int mask = 3 ^ lingDi;
			string sql = string.Format("UPDATE t_juntuaN SET lingdi=lingdi|{1} WHERE juntuanid={0};", junTuanId, lingDi, mask);
			this.AddDelayWriteSql(sql);
			sql = string.Format("UPDATE t_juntuaN SET lingdi=lingdi&{2} WHERE juntuanid<>{0};", junTuanId, lingDi, mask);
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00011F2C File Offset: 0x0001012C
		public void DestroyJunTuan(JunTuanDetailData detailData)
		{
			foreach (JunTuanBangHuiData data in detailData.JunTuanBangHuiList.V)
			{
				data.JuTuanZhiWu = 0;
				this.UpdateJunTuanBangHuiData(data, 0);
				this.BangHuiJunTuanIdDict[data.BhId] = 0;
			}
			this.JunTuanAllDataDict.Remove(detailData.JunTuanId);
			this.DeleteJunTuan(detailData.JunTuanId);
			this.UpdateJunTuanMiniDataList();
			this.UpdateJunTuanBaseDataList();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00011FD8 File Offset: 0x000101D8
		public void DeleteJunTuan(int junTuanId)
		{
			string sql = string.Format("update t_juntuan set isdel=1 WHERE juntuanid={0};", junTuanId);
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00012000 File Offset: 0x00010200
		public void UpdateJuntuanRoleDataList(int bhid, Dictionary<int, JunTuanRoleData> dict)
		{
			try
			{
				string sql = string.Format("DELETE FROM t_juntuan_roles WHERE bhid={1} and rid NOT IN ({0});", string.Join<int>(",", dict.Keys), bhid);
				this.AddDelayWriteSql(sql);
				foreach (JunTuanRoleData data in dict.Values)
				{
					sql = string.Format("REPLACE INTO t_juntuan_roles(bhid,rid,rname,zoneid,zhanli,zhiwu,zhuansheng,`level`,occu) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8});", new object[]
					{
						data.BhId,
						data.RoleId,
						data.RoleName,
						data.ZoneId,
						data.ZhanLi,
						data.JuTuanZhiWu,
						data.ChangeLifeCount,
						data.Level,
						data.Occu
					});
					this.AddDelayWriteSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00012160 File Offset: 0x00010360
		public void UpdateJuntuanRoleData(JunTuanRoleData data)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_juntuan_roles(bhid,rid,rname,zoneid,zhanli,zhiwu,zhuansheng,`level`,occu) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8});", new object[]
				{
					data.BhId,
					data.RoleId,
					data.RoleName,
					data.ZoneId,
					data.ZhanLi,
					data.JuTuanZhiWu,
					data.ChangeLifeCount,
					data.Level,
					data.Occu
				});
				this.AddDelayWriteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0001222C File Offset: 0x0001042C
		public bool GetHongBaoHuoDongData(ref long huoDongStartTicks, ref int nextSendId, ref long leftCharge, ref long totalCharge)
		{
			try
			{
				object obj = DbHelperMySQL.GetSingle("select value from t_async where id = " + 40);
				int unixtime;
				if (obj == null || !int.TryParse(obj.ToString(), out unixtime))
				{
					huoDongStartTicks = DateTime.MinValue.Ticks;
				}
				else
				{
					huoDongStartTicks = TimeUtil.UnixSecondsToTicks(unixtime);
				}
				obj = DbHelperMySQL.GetSingle("select value from t_async where id = " + 43);
				if (obj == null || !int.TryParse(obj.ToString(), out nextSendId))
				{
					huoDongStartTicks = 0L;
				}
				obj = DbHelperMySQL.GetSingle("select value from t_async where id = " + 41);
				if (obj == null || !long.TryParse(obj.ToString(), out leftCharge))
				{
					huoDongStartTicks = 0L;
				}
				obj = DbHelperMySQL.GetSingle("select value from t_async where id = " + 42);
				if (obj == null || !long.TryParse(obj.ToString(), out totalCharge))
				{
					huoDongStartTicks = 0L;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0001235C File Offset: 0x0001055C
		public void UpdateHongBaoHuoDongData(long huoDongStartTicks, int nextSendId, long leftCharge, long totalCharge)
		{
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 40, TimeUtil.SysTicksToUnixSeconds(huoDongStartTicks)));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 43, nextSendId));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 41, leftCharge));
			DbHelperMySQL.ExecuteSql(string.Format("replace INTO t_async(`id`,`value`) VALUES({0},{1});", 42, totalCharge));
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000123E4 File Offset: 0x000105E4
		public long CreateHongBao(string keystr, int senderid, DateTime startTime, DateTime endTime, int zuanshi, int state)
		{
			string sql = string.Format("INSERT INTO `t_hongbao_chongzhi_send` (`keystr`,`senderid`,`sendtime`,`endtime`,`zuanshi`,`leftzuanshi`,`state`) VALUES ('{0}','{1}','{2}','{3}','{4}','{4}','{5}');", new object[]
			{
				keystr,
				senderid,
				startTime.ToString("yyyy-MM-dd HH:mm:ss"),
				endTime.ToString("yyyy-MM-dd HH:mm:ss"),
				zuanshi,
				state
			});
			return this.ExecuteSqlGetIncrement(sql);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00012450 File Offset: 0x00010650
		public void UpdateHongBao(int hongbaoid, int leftzuanshi, int state)
		{
			string sql = string.Format("UPDATE `t_hongbao_chongzhi_send` SET leftzuanshi={1},state={2} WHERE id={0};", hongbaoid, leftzuanshi, state);
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00012484 File Offset: 0x00010684
		public bool LoadHongBaoDataList(string keyStr, Dictionary<int, SystemHongBaoData> hongBaoDict, Dictionary<long, int> recvDict)
		{
			MySqlDataReader sdr = null;
			try
			{
				string strSql = string.Format("SELECT id,senderid,sendtime,endtime,leftzuanshi,state FROM `t_hongbao_chongzhi_send` where keystr='{0}';", keyStr);
				sdr = DbHelperMySQL.ExecuteReader(strSql, false);
				int index = 1;
				while (sdr.Read())
				{
					SystemHongBaoData data = new SystemHongBaoData();
					data.HongBaoId = Convert.ToInt32(sdr[0].ToString());
					data.ID = (int)Convert.ToInt64(sdr[1].ToString());
					data.StartTime = Convert.ToDateTime(sdr[2].ToString()).Ticks / 10000L;
					data.DurationTime = (int)(Convert.ToDateTime(sdr[3].ToString()).Ticks / 10000L - data.StartTime);
					data.LeftZuanShi = Convert.ToInt32(sdr[4].ToString());
					data.State = Convert.ToInt32(sdr[5].ToString());
					hongBaoDict[data.HongBaoId] = data;
					index++;
				}
				sdr.Close();
				for (int i = 0; i < 100000000; i += 10000)
				{
					bool load = false;
					strSql = string.Format("SELECT hongbaoid,rid FROM `t_hongbao_chongzhi_recv` where keystr='{0}' limit {1},10000;", keyStr, i);
					sdr = DbHelperMySQL.ExecuteReader(strSql, false);
					index = 1;
					while (sdr.Read())
					{
						load = true;
						int hongBaoId = Convert.ToInt32(sdr[0].ToString());
						int rid = Convert.ToInt32(sdr[1].ToString());
						long kvp = ((long)hongBaoId << 36) + (long)rid;
						recvDict[kvp] = 1;
						index++;
					}
					sdr.Close();
					if (!load)
					{
						break;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			finally
			{
				if (null != sdr)
				{
					sdr.Close();
				}
			}
			return true;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000126B8 File Offset: 0x000108B8
		public void WriteHongBaoRecv(string keystr, int hongBaoId, int rid, int zoneid, string userid, string rname, int zuanshi)
		{
			string sql = string.Format("INSERT INTO `t_hongbao_chongzhi_recv` (`keystr`, `hongbaoid`, `rid`, `lasttime`, `zoneid`, `userid`, `rname`, `zuanshi`) VALUES ('{0}', '{1}', '{2}', now(), '{3}', '{4}', '{5}', '{6}');", new object[]
			{
				keystr,
				hongBaoId,
				rid,
				zoneid,
				userid,
				rname,
				zuanshi
			});
			this.AddDelayWriteSql(sql);
		}

		// Token: 0x040000D1 RID: 209
		public static readonly JunTuanPersistence Instance = new JunTuanPersistence();

		// Token: 0x040000D2 RID: 210
		public object Mutex = new object();

		// Token: 0x040000D3 RID: 211
		public bool Initialized = false;

		// Token: 0x040000D4 RID: 212
		public JunTuanRuntimeData RuntimeData = new JunTuanRuntimeData();

		// Token: 0x040000D5 RID: 213
		public KuaFuCmdData JunTuanBaseDataListCmdData = new KuaFuCmdData();

		// Token: 0x040000D6 RID: 214
		public KuaFuCmdData JunTuanMiniDataListCmdData = new KuaFuCmdData();

		// Token: 0x040000D7 RID: 215
		public KuaFuCmdData JunTuanRankDataListCmdData = new KuaFuCmdData();

		// Token: 0x040000D8 RID: 216
		public Dictionary<int, JunTuanDetailData> JunTuanAllDataDict = new Dictionary<int, JunTuanDetailData>();

		// Token: 0x040000D9 RID: 217
		public Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		// Token: 0x040000DA RID: 218
		public KuaFuData<List<JunTuanRankData>> JunTuanRankDataList = new KuaFuData<List<JunTuanRankData>>();

		// Token: 0x040000DB RID: 219
		public Dictionary<int, JunTuanBangHuiData> JunTuanBangHuiDataDict = new Dictionary<int, JunTuanBangHuiData>();

		// Token: 0x040000DC RID: 220
		private int MaxPaiMingRank = 100;

		// Token: 0x040000DD RID: 221
		private int MaxLogCount = 100;

		// Token: 0x040000DE RID: 222
		private int CurrGameId = Global.UninitGameId;

		// Token: 0x040000DF RID: 223
		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
