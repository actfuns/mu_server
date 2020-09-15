using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x0200008B RID: 139
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class YongZheZhanChangService : MarshalByRefObject, IYongZheZhanChangService, IExecCommand
	{
		// Token: 0x06000740 RID: 1856 RVA: 0x00060240 File Offset: 0x0005E440
		public override object InitializeLifetimeService()
		{
			YongZheZhanChangService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x000602BC File Offset: 0x0005E4BC
		public YongZheZhanChangService()
		{
			YongZheZhanChangService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x000603FC File Offset: 0x0005E5FC
		~YongZheZhanChangService()
		{
			this.BackgroundThread.Abort();
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00060434 File Offset: 0x0005E634
		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.Persistence.Initialized);
			DateTime lastRunTime = TimeUtil.NowDateTime();
			for (;;)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					this.RunLangHunLingYuTimerProc();
					if (now > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = now.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						lock (this.Mutex)
						{
							this.CheckRoleTimerProc(now, out signUpCount, out startCount);
						}
						ClientAgentManager.Instance().SetGameTypeLoad((GameTypes)this.RunTimeGameType, signUpCount, startCount);
					}
					if (now > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = now.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(now);
						this.CheckOverTimeLangHunLingYuGameFuBen(now);
					}
					this.Persistence.WriteRoleInfoDataProc();
					int sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
					this.Persistence.SaveCostTime(sleepMS);
					sleepMS = 1600 - sleepMS;
					if (sleepMS < 50)
					{
						sleepMS = 50;
					}
					Thread.Sleep(sleepMS);
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x000605E0 File Offset: 0x0005E7E0
		private void CheckRoleTimerProc(DateTime now, out int signUpCount, out int startCount)
		{
			signUpCount = 0;
			startCount = 0;
			int gameState;
			lock (this.Mutex)
			{
				gameState = this.GameState;
			}
			if (gameState == 2)
			{
				LogManager.WriteLog(LogTypes.Info, "清除上场遗留的活动副本信息,开始统计报名玩家列表", null, true);
				this.FuBenDataDict.Clear();
				this.PreAssignGameFuBenDataDict.Clear();
			}
			DateTime stateEndTime = now.AddSeconds((double)this.EnterGameSecs);
			DateTime removeRoleTime = now.AddSeconds((double)(-(double)this.EnterGameSecs));
			foreach (KuaFuRoleData kuaFuRoleData in this.RoleIdKuaFuRoleDataDict.Values)
			{
				if (kuaFuRoleData.StateEndTicks < removeRoleTime.Ticks)
				{
					KuaFuRoleData kuaFuRoleDataTemp;
					this.RoleIdKuaFuRoleDataDict.TryRemove(KuaFuRoleKey.Get(kuaFuRoleData.ServerId, kuaFuRoleData.RoleId), out kuaFuRoleDataTemp);
				}
				else if (kuaFuRoleData.State >= KuaFuRoleStates.SignUp && kuaFuRoleData.State < KuaFuRoleStates.StartGame)
				{
					signUpCount++;
					if (kuaFuRoleData.State == KuaFuRoleStates.SignUp)
					{
						if (gameState == 2)
						{
							this.AssignGameFubenStep1(kuaFuRoleData, stateEndTime.Ticks);
						}
					}
				}
				else if (kuaFuRoleData.State == KuaFuRoleStates.StartGame)
				{
					startCount++;
				}
			}
			if (gameState == 2)
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("对玩家进行场次分组:SignUpRoleCount={0},StartGameRoleCount={1}", signUpCount, startCount), null, true);
				this.AssignGameFubenStep2();
				this.AssginGameFuBenComplete = false;
				lock (this.Mutex)
				{
					this.GameState = 3;
				}
			}
			else if (gameState == 3)
			{
				if (!this.AssginGameFuBenComplete)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("尝试为场次创建活动副本", new object[0]), null, true);
					this.AssginGameFuBenComplete = this.AssignGameFubenStep3(stateEndTime);
				}
				else
				{
					lock (this.Mutex)
					{
						this.GameState = 1;
						GameLogItem gameLogItem = new GameLogItem();
						gameLogItem.SignUpCount = signUpCount;
						gameLogItem.EnterCount = startCount;
						gameLogItem.GameType = this.RunTimeGameType;
						this.Persistence.UpdateRoleInfoData(gameLogItem);
					}
				}
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x000608DC File Offset: 0x0005EADC
		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.FuBenDataDict.Count > 0)
			{
				foreach (YongZheZhanChangFuBenData fuBenData in this.FuBenDataDict.Values)
				{
					lock (fuBenData)
					{
						if (fuBenData.CanRemove() || fuBenData.EndTime < now)
						{
							this.RemoveGameFuBen(fuBenData);
						}
					}
				}
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x000609B8 File Offset: 0x0005EBB8
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			bool broadcast = false;
			lock (this.Mutex)
			{
				if (this.Persistence.LangHunLingYuInitialized)
				{
					ClientAgent agent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (agent != null && agent.ClientInfo.ClientId > 0)
					{
						int clientId;
						if (!this.Persistence.LangHunLingYuBroadcastServerIdHashSet.TryGetValue(serverId, out clientId) || clientId != agent.ClientInfo.ClientId)
						{
							this.Persistence.LangHunLingYuBroadcastServerIdHashSet[serverId] = agent.ClientInfo.ClientId;
							broadcast = true;
						}
					}
				}
			}
			if (broadcast)
			{
				this.PostAllLangHunLingYuData(serverId);
			}
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.YzzcGameType);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00060AC0 File Offset: 0x0005ECC0
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool bFirstInit = false;
					int ret = ClientAgentManager.Instance().InitializeClient(clientInfo, out bFirstInit);
					if (ret > 0)
					{
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
					result = ret;
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00060BDC File Offset: 0x0005EDDC
		public void PostAllLangHunLingYuData(int serverId)
		{
			lock (this.Mutex)
			{
				foreach (LangHunLingYuBangHuiDataEx item in this.LangHunLingYuBangHuiDataExDict.Values)
				{
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.UpdateLhlyBhData, new object[]
					{
						item
					});
					ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, evItem);
				}
				foreach (LangHunLingYuCityDataEx item2 in this.LangHunLingYuCityDataExDict)
				{
					if (null != item2)
					{
						AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.UpdateLhlyCityData, new object[]
						{
							item2
						});
						ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, evItem);
					}
				}
				ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, new AsyncDataItem(KuaFuEventTypes.UpdateLhlyOtherCityList, new object[]
				{
					new Dictionary<int, List<int>>(this.OtherCityList)
				}));
				ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, new AsyncDataItem(KuaFuEventTypes.UpdateLhlyCityOwnerList, new object[]
				{
					this.GetLangHunLingYuCityOwnerHist()
				}));
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00060D70 File Offset: 0x0005EF70
		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				ClientAgent agent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
				if (null != agent)
				{
					KuaFuServerManager.UpdateKuaFuLineData(agent.ClientInfo.ServerId, mapClientCountDict);
					ClientAgentManager.Instance().SetMainlinePayload(agent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
				}
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00060DE8 File Offset: 0x0005EFE8
		public int ExecuteCommand(string cmd)
		{
			int result;
			if (string.IsNullOrEmpty(cmd))
			{
				result = -18;
			}
			else
			{
				string[] args = cmd.Split(YongZheZhanChangService.WriteSpaceChars, StringSplitOptions.RemoveEmptyEntries);
				result = this.ExecCommand(args);
			}
			return result;
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00060E21 File Offset: 0x0005F021
		public void UpdateStatisticalData(AsyncDataItem data)
		{
			this.Persistence.UpdateRoleInfoData(data.Args[0]);
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00060EA0 File Offset: 0x0005F0A0
		public int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData)
		{
			int result = 1;
			int result2;
			if (this.GameState != 1)
			{
				result2 = -2001;
			}
			else if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("RoleSignUp时ServerId错误.ServerId:{0},roleId:{1}", serverId, roleId), null, true);
				result2 = -500;
			}
			else
			{
				Lazy<KuaFuRoleData> lazy = new Lazy<KuaFuRoleData>(() => new KuaFuRoleData
				{
					RoleId = roleId,
					UserId = userId,
					GameType = gameType
				});
				KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
				KuaFuRoleData kuaFuRoleData = this.RoleIdKuaFuRoleDataDict.GetOrAdd(key, (KuaFuRoleKey x) => lazy.Value);
				lock (kuaFuRoleData)
				{
					kuaFuRoleData.Age++;
					kuaFuRoleData.State = KuaFuRoleStates.SignUp;
					kuaFuRoleData.ServerId = serverId;
					kuaFuRoleData.ZoneId = zoneId;
					kuaFuRoleData.GameData = gameData;
					kuaFuRoleData.GroupIndex = groupIndex;
					kuaFuRoleData.StateEndTicks = Global.NowTime.Ticks;
				}
				LogManager.WriteLog(LogTypes.Trace, string.Format("YongZheZhanChang.RoleSignUp,{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					serverId,
					userId,
					zoneId,
					roleId,
					gameType,
					groupIndex,
					gameData.ZhanDouLi
				}), null, true);
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0006107C File Offset: 0x0005F27C
		public int RoleChangeState(int serverId, int roleId, int state)
		{
			int result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				result = -11003;
			}
			else
			{
				KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
				KuaFuRoleData kuaFuRoleData;
				if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleData))
				{
					result = -11003;
				}
				else
				{
					int oldGameId = 0;
					lock (kuaFuRoleData)
					{
						if (state == 0)
						{
							oldGameId = kuaFuRoleData.GameId;
							kuaFuRoleData.GameId = 0;
						}
						kuaFuRoleData.Age++;
						kuaFuRoleData.State = (KuaFuRoleStates)state;
					}
					if (oldGameId > 0)
					{
						this.RemoveRoleFromFuBen(oldGameId, roleId);
					}
					result = state;
				}
			}
			return result;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0006115C File Offset: 0x0005F35C
		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state)
		{
			YongZheZhanChangFuBenData fuBenData;
			if (this.FuBenDataDict.TryGetValue(gameId, out fuBenData))
			{
				lock (fuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (fuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
					{
						if (state == 7 || state == 0)
						{
							this.RemoveRoleFromFuBen(gameId, roleId);
						}
					}
				}
			}
			KuaFuRoleData kuaFuRoleData;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData))
			{
				result = -20;
			}
			else
			{
				if (kuaFuRoleData.GameId == gameId)
				{
					this.ChangeRoleState(kuaFuRoleData, (KuaFuRoleStates)state);
				}
				result = state;
			}
			return result;
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0006123C File Offset: 0x0005F43C
		public KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			KuaFuRoleData result;
			if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData) && kuaFuRoleData.State != KuaFuRoleStates.None)
			{
				result = kuaFuRoleData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00061280 File Offset: 0x0005F480
		public int GetRoleExtendData(int serverId, int roleId, int dataType)
		{
			KuaFuRoleData kuaFuRoleData = null;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData))
			{
				result = 0;
			}
			else if (dataType == 2)
			{
				result = (int)kuaFuRoleData.State;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000612C8 File Offset: 0x0005F4C8
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x000612E0 File Offset: 0x0005F4E0
		public YongZheZhanChangFuBenData GetFuBenData(int gameId)
		{
			YongZheZhanChangFuBenData kuaFuFuBenData = null;
			YongZheZhanChangFuBenData result;
			if (this.FuBenDataDict.TryGetValue(gameId, out kuaFuFuBenData) && kuaFuFuBenData.State < GameFuBenState.End)
			{
				result = kuaFuFuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00061320 File Offset: 0x0005F520
		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11;
			YongZheZhanChangFuBenData fubenData = null;
			int result2;
			if (this.FuBenDataDict.TryGetValue(gameId, out fubenData))
			{
				lock (fubenData)
				{
					fubenData.State = state;
					if (state == GameFuBenState.End)
					{
						this.RemoveGameFuBen(fubenData);
					}
				}
				result2 = result;
			}
			else
			{
				result2 = -20;
			}
			return result2;
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x000613B4 File Offset: 0x0005F5B4
		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return new AsyncDataItem(KuaFuEventTypes.Other, new object[]
			{
				KuaFuServerManager.GetKuaFuLineDataList(mapCode)
			});
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x000613E4 File Offset: 0x0005F5E4
		public int EnterKuaFuMap(int serverId, int roleId, int mapCode, int kuaFuLine)
		{
			int kuaFuServerId = KuaFuServerManager.EnterKuaFuMapLine(kuaFuLine, mapCode);
			int result;
			if (kuaFuServerId > 0)
			{
				KuaFuMapRoleData kuaFuMapRoleData = new KuaFuMapRoleData();
				kuaFuMapRoleData = this.RoleId2KuaFuMapIdDict.GetOrAdd(roleId, kuaFuMapRoleData);
				kuaFuMapRoleData.ServerId = serverId;
				kuaFuMapRoleData.RoleId = roleId;
				kuaFuMapRoleData.KuaFuMapCode = mapCode;
				kuaFuMapRoleData.KuaFuServerId = kuaFuServerId;
				result = kuaFuServerId;
			}
			else
			{
				result = -100;
			}
			return result;
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00061444 File Offset: 0x0005F644
		public KuaFuMapRoleData GetKuaFuMapRoleData(int roleId)
		{
			KuaFuMapRoleData kuaFuMapRoleData;
			this.RoleId2KuaFuMapIdDict.TryGetValue(roleId, out kuaFuMapRoleData);
			return kuaFuMapRoleData;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00061468 File Offset: 0x0005F668
		private void RunLangHunLingYuTimerProc()
		{
			DateTime now = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				if (!this.Persistence.LangHunLingYuInitialized)
				{
					List<LangHunLingYuBangHuiDataEx> bangHuilist = new List<LangHunLingYuBangHuiDataEx>();
					List<LangHunLingYuCityDataEx> cityList = new List<LangHunLingYuCityDataEx>();
					List<LangHunLingYuKingHist> cityOwnerHistList = new List<LangHunLingYuKingHist>();
					if (this.Persistence.LoadBangHuiDataExList(bangHuilist) && this.Persistence.LoadCityDataExList(cityList) && this.Persistence.LoadCityOwnerHistory(cityOwnerHistList))
					{
						HashSet<long> existBhids = new HashSet<long>();
						foreach (LangHunLingYuCityDataEx c in cityList)
						{
							foreach (long bhid in c.Site)
							{
								existBhids.Add(bhid);
							}
							if (this.LangHunLingYuCityDataExDict[c.CityId] == null)
							{
								this.LangHunLingYuCityDataExDict[c.CityId] = c;
							}
							else
							{
								this.LangHunLingYuCityDataExDict[c.CityId].CityId = c.CityId;
								this.LangHunLingYuCityDataExDict[c.CityId].CityLevel = c.CityLevel;
								Array.Copy(c.Site, this.LangHunLingYuCityDataExDict[c.CityId].Site, this.LangHunLingYuCityDataExDict[c.CityId].Site.Length);
							}
						}
						foreach (LangHunLingYuKingHist d in cityOwnerHistList)
						{
							int admire_count = 0;
							if (!this.LangHunLingYuAdmireDict.TryGetValue(d.rid, out admire_count))
							{
								this.LangHunLingYuAdmireDict[d.rid] = d.AdmireCount;
							}
						}
						foreach (LangHunLingYuBangHuiDataEx b in bangHuilist)
						{
							LangHunLingYuBangHuiDataEx bangHuiDataEx;
							if (!this.LangHunLingYuBangHuiDataExDict.TryGetValue((long)b.Bhid, out bangHuiDataEx))
							{
								if (existBhids.Contains((long)b.Bhid))
								{
									this.LangHunLingYuBangHuiDataExDict[(long)b.Bhid] = b;
								}
							}
							else
							{
								bangHuiDataEx.Bhid = b.Bhid;
								bangHuiDataEx.BhName = b.BhName;
								bangHuiDataEx.ZoneId = b.ZoneId;
							}
						}
						this.LangHunLingYuCityHistList = cityOwnerHistList;
						this.CalcBangHuiCityLevel(null, false);
						this.Persistence.LangHunLingYuInitialized = true;
					}
				}
				double secs = (now.TimeOfDay - this.Persistence.LangHunLingYuResetCityTime).TotalSeconds;
				if (secs >= 0.0 && secs < 300.0 && (now - this.Persistence.LastLangHunLingYuResetCityTime).TotalHours >= 1.0)
				{
					this.Persistence.LastLangHunLingYuResetCityTime = now;
					LangHunLingYuCityDataEx[] langHunLingYuCityDataExDict = this.LangHunLingYuCityDataExDict;
					int j = 0;
					while (j < langHunLingYuCityDataExDict.Length)
					{
						LangHunLingYuCityDataEx c = langHunLingYuCityDataExDict[j];
						bool reset = false;
						if (c != null)
						{
							bool clear = false;
							lock (this.Mutex)
							{
								CityLevelInfo ci;
								if (this.Persistence.CityLevelInfoDict.TryGetValue(c.CityLevel, out ci))
								{
									foreach (int day in ci.AttackWeekDay)
									{
										if (day == (int)now.DayOfWeek)
										{
											clear = true;
											break;
										}
									}
								}
							}
							if (clear)
							{
								c.GameId = 0;
								for (int i = 1; i < 4; i++)
								{
									if (c.Site[i] > 0L)
									{
										c.Site[i] = 0L;
										reset = true;
									}
								}
								if (reset)
								{
									LogManager.WriteLog(LogTypes.Info, string.Format("清空城池{0}进攻者状态", c.CityId), null, true);
									this.Persistence.SaveCityData(c);
									this.NotifyUpdateCityDataEx(c);
								}
							}
						}
						IL_4AA:
						j++;
						continue;
						goto IL_4AA;
					}
					this.FilterOwnerHistListData();
				}
				secs %= 3600.0;
				secs = (secs + 3600.0) % 3600.0;
				if (secs >= 600.0 && secs <= 660.0 && (now - this.Persistence.LastLangHunLingYuBroadcastTime).TotalHours >= 2.0)
				{
					this.Persistence.LastLangHunLingYuBroadcastTime = now;
					this.Persistence.LangHunLingYuBroadcastServerIdHashSet.Clear();
				}
			}
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00061A60 File Offset: 0x0005FC60
		private int GetCityLevelById(int cityId)
		{
			return 10 - (int)Math.Log((double)cityId, 2.0);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00061A88 File Offset: 0x0005FC88
		private void NotifyUpdatBangHuiDataEx(LangHunLingYuBangHuiDataEx bangHuiDataEx)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyBhData, new object[]
			{
				bangHuiDataEx
			}));
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00061AB0 File Offset: 0x0005FCB0
		private void NotifyUpdateCityDataEx(LangHunLingYuCityDataEx cityDataEx)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyCityData, new object[]
			{
				cityDataEx
			}));
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00061AD8 File Offset: 0x0005FCD8
		private void NotifyUpdateOtherCityList(Dictionary<int, List<int>> list)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyOtherCityList, new object[]
			{
				list
			}));
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00061B00 File Offset: 0x0005FD00
		private void NotifyUpdateCityOwnerHist(List<LangHunLingYuKingHist> list)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyCityOwnerList, new object[]
			{
				list
			}));
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00061B28 File Offset: 0x0005FD28
		private void NotifyUpdateCityOwnerAdmire(int rid, int admirecount)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyCityOwnerAdmire, new object[]
			{
				rid,
				admirecount
			}));
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00061B60 File Offset: 0x0005FD60
		public int CalcNeedNextLevelCityCount(int cityLevel, int maxAttackerPerCity)
		{
			int result;
			if (cityLevel <= 0)
			{
				result = 1023;
			}
			else
			{
				int cityWithOwnerCount = 0;
				int minCityId = 1 << 10 - cityLevel;
				int maxCityId = minCityId * 2;
				for (int i = minCityId; i < maxCityId; i++)
				{
					if (this.LangHunLingYuCityDataExDict[i] != null)
					{
						if (this.LangHunLingYuCityDataExDict[i].Site[0] > 0L)
						{
							cityWithOwnerCount++;
						}
					}
				}
				result = (int)Math.Ceiling((double)cityWithOwnerCount / (double)maxAttackerPerCity);
			}
			return result;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00061C04 File Offset: 0x0005FE04
		public List<int> GetRandomCityListByLevel(int cityLevel, int reserveCount)
		{
			List<int> list = new List<int>();
			int cityWithOwnerCount = 0;
			int minCityId = 1 << 10 - cityLevel;
			int maxCityId = minCityId * 2;
			HashSet<int> cityHashSet = new HashSet<int>();
			for (int i = minCityId; i < maxCityId; i++)
			{
				if (this.LangHunLingYuCityDataExDict[i] != null)
				{
					if (this.LangHunLingYuCityDataExDict[i].Site.Any((long x) => x > 0L))
					{
						cityHashSet.Add(i);
						cityWithOwnerCount++;
					}
				}
			}
			int[] array = cityHashSet.ToArray<int>();
			foreach (int cityId in array)
			{
				int rnd = Global.GetRandomNumber(0, list.Count + 1);
				list.Insert(rnd, cityId);
			}
			if (cityWithOwnerCount < reserveCount)
			{
				for (int i = minCityId; i < maxCityId; i++)
				{
					if (!cityHashSet.Contains(i))
					{
						list.Add(i);
						cityWithOwnerCount++;
						if (cityWithOwnerCount >= reserveCount)
						{
							break;
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00061D4C File Offset: 0x0005FF4C
		private void FilterOwnerHistListData()
		{
			lock (this.Mutex)
			{
				if (this.LangHunLingYuCityHistList != null && this.LangHunLingYuCityHistList.Count != 0)
				{
					CityLevelInfo sceneItem = null;
					if (this.Persistence.CityLevelInfoDict.TryGetValue(10, out sceneItem))
					{
						bool NeedCheckData = false;
						DateTime now = TimeUtil.NowDateTime();
						for (int loop = 0; loop < sceneItem.AttackWeekDay.Length; loop++)
						{
							if (now.DayOfWeek == (DayOfWeek)sceneItem.AttackWeekDay[loop])
							{
								NeedCheckData = true;
								break;
							}
						}
						if (NeedCheckData)
						{
							LangHunLingYuKingHist CurKingHist = this.LangHunLingYuCityHistList[this.LangHunLingYuCityHistList.Count - 1];
							DateTime NowDate = new DateTime(now.Year, now.Month, now.Day);
							DateTime ComDate = new DateTime(CurKingHist.CompleteTime.Year, CurKingHist.CompleteTime.Month, CurKingHist.CompleteTime.Day);
							if (DateTime.Compare(NowDate, ComDate) != 0)
							{
								CurKingHist.CompleteTime = now;
								this.LangHunLingYuCityHistList.Add(CurKingHist);
								this.Persistence.InsertCityOwnerHistory(CurKingHist);
								this.NotifyUpdateCityOwnerHist(this.GetLangHunLingYuCityOwnerHist());
							}
						}
					}
				}
			}
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00061EE8 File Offset: 0x000600E8
		private List<LangHunLingYuKingHist> GetLangHunLingYuCityOwnerHist()
		{
			List<LangHunLingYuKingHist> CityHistList = null;
			lock (this.Mutex)
			{
				if (this.LangHunLingYuCityHistList.Count != 0 && this.LangHunLingYuCityHistList.Count > 10)
				{
					int index = this.LangHunLingYuCityHistList.Count - 10;
					CityHistList = this.LangHunLingYuCityHistList.GetRange(index, 10);
				}
				else
				{
					CityHistList = new List<LangHunLingYuKingHist>(this.LangHunLingYuCityHistList);
				}
			}
			return CityHistList;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00061F94 File Offset: 0x00060194
		public bool LangHunLingYuAdmaire(int rid)
		{
			lock (this.Mutex)
			{
				if (!this.Persistence.LangHunLingYuInitialized)
				{
					return false;
				}
				int admire_count = 0;
				if (!this.LangHunLingYuAdmireDict.TryGetValue(rid, out admire_count))
				{
					return false;
				}
				admire_count = (this.LangHunLingYuAdmireDict[rid] = admire_count + 1);
				foreach (LangHunLingYuKingHist d in this.LangHunLingYuCityHistList)
				{
					if (d.rid == rid)
					{
						d.AdmireCount++;
					}
				}
				this.Persistence.AdmireCityOwner(rid);
				this.NotifyUpdateCityOwnerAdmire(rid, admire_count);
			}
			return true;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x000620A8 File Offset: 0x000602A8
		public int LangHunLingYuSignUp(string bhName, int bhid, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					if (!this.Persistence.LangHunLingYuInitialized)
					{
						result = -11000;
						return result;
					}
					LangHunLingYuBangHuiDataEx banghuiDataEx;
					if (!this.LangHunLingYuBangHuiDataExDict.TryGetValue((long)bhid, out banghuiDataEx))
					{
						banghuiDataEx = new LangHunLingYuBangHuiDataEx
						{
							Bhid = bhid,
							BhName = bhName,
							ZoneId = zoneId
						};
						this.LangHunLingYuBangHuiDataExDict[(long)bhid] = banghuiDataEx;
						this.NotifyUpdatBangHuiDataEx(banghuiDataEx);
						this.Persistence.SaveBangHuiData(banghuiDataEx);
					}
					else if (banghuiDataEx.BhName != bhName)
					{
						banghuiDataEx.BhName = bhName;
						this.NotifyUpdatBangHuiDataEx(banghuiDataEx);
						this.Persistence.SaveBangHuiData(banghuiDataEx);
					}
					int cityLevel = banghuiDataEx.Level + 1;
					if (cityLevel > 10)
					{
						result = -4004;
						return result;
					}
					int attackerCountLimit = 3;
					CityLevelInfo ci;
					if (this.Persistence.CityLevelInfoDict.TryGetValue(cityLevel, out ci))
					{
						attackerCountLimit = ci.MaxNum;
					}
					result = -2001;
					DateTime now = TimeUtil.NowDateTime();
					for (int i = 0; i < ci.BaoMingTime.Count - 1; i += 2)
					{
						TimeSpan ts = now.TimeOfDay.Add(TimeSpan.FromDays((double)now.DayOfWeek));
						if (ts >= ci.BaoMingTime[i] && ts <= ci.BaoMingTime[i + 1])
						{
							result = 1;
							break;
						}
					}
					if (result < 0)
					{
						return result;
					}
					int toCityId = -1;
					int toCitySite = 0;
					int reserveCount = this.CalcNeedNextLevelCityCount(cityLevel - 1, attackerCountLimit);
					List<int> cityList = this.GetRandomCityListByLevel(cityLevel, reserveCount);
					foreach (int cityId in cityList)
					{
						if (this.LangHunLingYuCityDataExDict[cityId] == null)
						{
							if (toCityId < 0)
							{
								toCityId = cityId;
								toCitySite = 1;
							}
						}
						else
						{
							for (int j = 1; j <= attackerCountLimit; j++)
							{
								if (this.LangHunLingYuCityDataExDict[cityId].Site[j] == (long)bhid)
								{
									result = -4005;
									return result;
								}
								if (toCityId < 0)
								{
									if (this.LangHunLingYuCityDataExDict[cityId].Site[j] == 0L)
									{
										toCityId = cityId;
										toCitySite = j;
									}
								}
							}
						}
					}
					if (toCityId >= 0)
					{
						if (this.LangHunLingYuCityDataExDict[toCityId] == null)
						{
							this.LangHunLingYuCityDataExDict[toCityId] = new LangHunLingYuCityDataEx
							{
								CityId = toCityId,
								CityLevel = cityLevel
							};
						}
						this.LangHunLingYuCityDataExDict[toCityId].Site[toCitySite] = (long)bhid;
						LangHunLingYuCityDataEx data = this.LangHunLingYuCityDataExDict[toCityId].Clone() as LangHunLingYuCityDataEx;
						this.Persistence.SaveCityData(data);
						this.NotifyUpdateCityDataEx(data);
					}
					else
					{
						result = -22;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -11003;
			}
			return result;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00062498 File Offset: 0x00060698
		private void CalcBangHuiCityLevel(HashSet<long> reCalcBangHuiLevelHashSet = null, bool broadcast = false)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dict = new Dictionary<long, int>();
				if (null != reCalcBangHuiLevelHashSet)
				{
					foreach (long bhid in reCalcBangHuiLevelHashSet)
					{
						dict[bhid] = 0;
					}
				}
				else
				{
					foreach (long bhid in this.LangHunLingYuBangHuiDataExDict.Keys)
					{
						dict[bhid] = 0;
					}
				}
				foreach (LangHunLingYuCityDataEx cityDataEx in this.LangHunLingYuCityDataExDict)
				{
					if (null != cityDataEx)
					{
						long bhid = cityDataEx.Site[0];
						int level;
						if (bhid > 0L && dict.TryGetValue(bhid, out level))
						{
							if (cityDataEx.CityLevel > level)
							{
								dict[bhid] = cityDataEx.CityLevel;
							}
						}
					}
				}
				foreach (KeyValuePair<long, int> kv in dict)
				{
					LangHunLingYuBangHuiDataEx bangHuiDataEx;
					if (this.LangHunLingYuBangHuiDataExDict.TryGetValue(kv.Key, out bangHuiDataEx))
					{
						if (bangHuiDataEx.Level != kv.Value)
						{
							bangHuiDataEx.Level = kv.Value;
							this.Persistence.SaveBangHuiData(bangHuiDataEx);
							if (broadcast)
							{
								this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.UpdateLhlyBhData, new object[]
								{
									bangHuiDataEx.Clone() as LangHunLingYuBangHuiDataEx
								}));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00062728 File Offset: 0x00060928
		private void UpdateOtherCityList(bool broadcast = false)
		{
			lock (this.Mutex)
			{
				Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
				foreach (LangHunLingYuCityDataEx cityDataEx in this.LangHunLingYuCityDataExDict)
				{
					if (cityDataEx != null && this.OtherCityLevelList[cityDataEx.CityLevel] >= 0)
					{
						int c0 = cityDataEx.Site.Count((long x) => x > 0L);
						if (c0 > 0)
						{
							List<int> list;
							if (!dict.TryGetValue(cityDataEx.CityLevel, out list))
							{
								list = new List<int>();
								dict[cityDataEx.CityLevel] = list;
							}
							list.Add(cityDataEx.CityId);
						}
					}
				}
				string log = "";
				foreach (KeyValuePair<int, List<int>> kv in dict)
				{
					int index = this.OtherCityLevelList[kv.Key];
					int rnd = Global.GetRandomNumber(0, kv.Value.Count);
					int cityId = kv.Value[rnd];
					List<int> list;
					if (!this.OtherCityList.TryGetValue(kv.Key, out list))
					{
						list = new List<int>();
						this.OtherCityList[kv.Key] = list;
					}
					list.Clear();
					list.Add(cityId);
					if (kv.Value.Count >= 2)
					{
						if (rnd > 0)
						{
							list.Add(kv.Value[rnd - 1]);
						}
						else
						{
							list.Add(kv.Value[rnd + 1]);
						}
					}
					log += string.Format("Level={0}:{1},{2};", kv.Key, (kv.Value.Count >= 1) ? kv.Value[0] : 0, (kv.Value.Count >= 2) ? kv.Value[1] : 0);
				}
				LogManager.WriteLog(LogTypes.Info, string.Format("重新计算他人城池展示列表{0}", log), null, true);
				if (broadcast)
				{
					this.NotifyUpdateOtherCityList(new Dictionary<int, List<int>>(this.OtherCityList));
				}
			}
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00062A10 File Offset: 0x00060C10
		public int GameFuBenComplete(LangHunLingYuStatisticalData data)
		{
			int result = 0;
			try
			{
				int cityId = data.CityId;
				int[] siteBhid = data.SiteBhids;
				lock (this.Mutex)
				{
					if (!this.Persistence.LangHunLingYuInitialized)
					{
						result = -11000;
						return result;
					}
					int hours = (int)(TimeUtil.NowDateTime() - data.CompliteTime).TotalHours;
					bool setAttacker = true;
					if (data.GameId > 0 && Math.Abs(hours) >= 20)
					{
						setAttacker = false;
						LogManager.WriteLog(LogTypes.Error, string.Format("更新城池占领者,CityID={0},占领者={1},但时间已超过预期时间{2}小时,将不重置进攻者", data.CityId, data.SiteBhids[0], hours), null, true);
					}
					HashSet<long> reCalcBangHuiLevelHashSet = new HashSet<long>();
					LangHunLingYuCityDataEx cityDataEx = this.LangHunLingYuCityDataExDict[cityId];
					if (null == cityDataEx)
					{
						cityDataEx = (this.LangHunLingYuCityDataExDict[cityId] = new LangHunLingYuCityDataEx
						{
							CityId = cityId,
							CityLevel = this.GetCityLevelById(cityId)
						});
					}
					for (int i = 0; i < cityDataEx.Site.Length; i++)
					{
						if (setAttacker || i == 0)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("更新城池信息,CityID={0},Site={1},old bhid={2},new bhid={3}", new object[]
							{
								cityDataEx.CityId,
								i,
								cityDataEx.Site[i],
								siteBhid[i]
							}), null, true);
							if (cityDataEx.Site[i] > 0L && !reCalcBangHuiLevelHashSet.Contains(cityDataEx.Site[i]))
							{
								reCalcBangHuiLevelHashSet.Add(cityDataEx.Site[i]);
							}
							cityDataEx.Site[i] = (long)siteBhid[i];
							if (cityDataEx.Site[i] > 0L && !reCalcBangHuiLevelHashSet.Contains(cityDataEx.Site[i]))
							{
								reCalcBangHuiLevelHashSet.Add(cityDataEx.Site[i]);
							}
						}
					}
					this.CalcBangHuiCityLevel(reCalcBangHuiLevelHashSet, true);
					cityDataEx = (cityDataEx.Clone() as LangHunLingYuCityDataEx);
					this.Persistence.SaveCityData(cityDataEx);
					this.NotifyUpdateCityDataEx(cityDataEx);
					if (cityDataEx.CityLevel == 10 && data.CityOwnerRoleData != null)
					{
						int admire_count = 0;
						if (!this.LangHunLingYuAdmireDict.TryGetValue(data.rid, out admire_count))
						{
							this.LangHunLingYuAdmireDict[data.rid] = admire_count;
						}
						LangHunLingYuKingHist CityOwnerData = new LangHunLingYuKingHist
						{
							rid = data.rid,
							AdmireCount = admire_count,
							CompleteTime = data.CompliteTime,
							CityOwnerRoleData = data.CityOwnerRoleData
						};
						this.LangHunLingYuCityHistList.Add(CityOwnerData);
						this.Persistence.InsertCityOwnerHistory(CityOwnerData);
						this.NotifyUpdateCityOwnerHist(this.GetLangHunLingYuCityOwnerHist());
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -11003;
			}
			return result;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00062D9C File Offset: 0x00060F9C
		private bool CreateLangHunLingYuGameFuBen(LangHunLingYuFuBenData fuBenData, DateTime stateEndTime)
		{
			try
			{
				int magicRoleCount = 10;
				int gameId = this.Persistence.GetNextGameId();
				int kfSrvId = 0;
				bool createSuccess = ClientAgentManager.Instance().AssginKfFuben(this.LhlyGameType, (long)gameId, magicRoleCount, out kfSrvId);
				if (createSuccess)
				{
					fuBenData.ServerId = kfSrvId;
					fuBenData.GameId = gameId;
					fuBenData.EndTime = Global.NowTime.AddMinutes(65.0);
					this.AddLangHunLingYuGameFuBen(fuBenData);
					LogManager.WriteLog(LogTypes.Info, string.Format("创建圣域争霸副本GameID={0},CityId={1},ServerID={2}", fuBenData.GameId, fuBenData.CityId, fuBenData.ServerId), null, true);
					this.Persistence.LogCreateYongZheFuBen(kfSrvId, gameId, 0, magicRoleCount);
					return true;
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("暂时没有可用的服务器可以给活动副本分配,稍后重试", new object[0]), null, true);
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00062EA8 File Offset: 0x000610A8
		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenDataByCityId(int cityId)
		{
			LangHunLingYuFuBenData result = null;
			try
			{
				int cityLevel = this.GetCityLevelById(cityId);
				lock (this.Mutex)
				{
					LangHunLingYuCityDataEx cityDataEx = this.LangHunLingYuCityDataExDict[cityId];
					if (null == cityDataEx)
					{
						return null;
					}
					if (!this.LangHunLingYuFuBenDataDict.TryGetValue(cityDataEx.GameId, out result))
					{
						result = new LangHunLingYuFuBenData();
						result.CityId = cityId;
						if (!this.CreateLangHunLingYuGameFuBen(result, Global.NowTime.AddHours(1.0)))
						{
							return null;
						}
						cityDataEx.GameId = result.GameId;
						this.Persistence.SaveCityData(cityDataEx);
						this.NotifyUpdateCityDataEx(cityDataEx);
					}
					result.CityDataEx = (cityDataEx.Clone() as LangHunLingYuCityDataEx);
					return result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00062FCC File Offset: 0x000611CC
		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenData(int gameId)
		{
			LangHunLingYuFuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					if (!this.LangHunLingYuFuBenDataDict.TryGetValue(gameId, out result))
					{
						return null;
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00063058 File Offset: 0x00061258
		private void AddLangHunLingYuGameFuBen(LangHunLingYuFuBenData fuBenData)
		{
			lock (this.Mutex)
			{
				this.LangHunLingYuFuBenDataDict[fuBenData.GameId] = fuBenData;
			}
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x000630B0 File Offset: 0x000612B0
		private void CheckOverTimeLangHunLingYuGameFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<LangHunLingYuFuBenData> removeList = new List<LangHunLingYuFuBenData>();
				foreach (LangHunLingYuFuBenData fubenData in this.LangHunLingYuFuBenDataDict.Values)
				{
					if (now > fubenData.EndTime)
					{
						removeList.Add(fubenData);
					}
				}
				foreach (LangHunLingYuFuBenData fuBenData in removeList)
				{
					int gameId = fuBenData.GameId;
					if (this.LangHunLingYuFuBenDataDict.Remove(gameId))
					{
						fuBenData.State = GameFuBenState.End;
						ClientAgentManager.Instance().RemoveKfFuben(this.LhlyGameType, fuBenData.ServerId, (long)fuBenData.GameId);
					}
				}
			}
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000631F0 File Offset: 0x000613F0
		private void Broadcast2GsAgent(AsyncDataItem item)
		{
			ClientAgentManager.Instance().BroadCastAsyncEvent(this.YzzcGameType, item, 0);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00063208 File Offset: 0x00061408
		private void ChangeRoleState(KuaFuRoleData kuaFuRoleData, KuaFuRoleStates state)
		{
			try
			{
				KuaFuRoleData roleData = null;
				int oldGameId = 0;
				lock (kuaFuRoleData)
				{
					kuaFuRoleData.Age++;
					kuaFuRoleData.State = state;
					if (state == KuaFuRoleStates.None && kuaFuRoleData.GameId > 0)
					{
						oldGameId = kuaFuRoleData.GameId;
					}
					roleData = kuaFuRoleData;
				}
				if (oldGameId > 0)
				{
					this.RemoveRoleFromFuBen(oldGameId, kuaFuRoleData.RoleId);
				}
				if (null != roleData)
				{
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.RoleStateChange, new object[]
					{
						kuaFuRoleData
					});
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x000632FC File Offset: 0x000614FC
		private void NotifyFuBenRoleCount(YongZheZhanChangFuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					int roleCount = fuBenData.RoleDict.Count;
					foreach (KuaFuFuBenRoleData role in fuBenData.RoleDict.Values)
					{
						KuaFuRoleData kuaFuRoleData;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(role.ServerId, role.RoleId), out kuaFuRoleData))
						{
							AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.NotifyWaitingRoleCount, new object[]
							{
								kuaFuRoleData.RoleId,
								roleCount
							});
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0006341C File Offset: 0x0006161C
		private void NotifyFuBenRoleEnterGame(YongZheZhanChangFuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					foreach (KuaFuFuBenRoleData role in fuBenData.RoleDict.Values)
					{
						KuaFuRoleData kuaFuRoleData;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(role.ServerId, role.RoleId), out kuaFuRoleData) && kuaFuRoleData.State == KuaFuRoleStates.NotifyEnterGame)
						{
							AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.UpdateAndNotifyEnterGame, new object[]
							{
								kuaFuRoleData
							});
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00063528 File Offset: 0x00061728
		private void AssignGameFubenStep1(KuaFuRoleData kuaFuRoleData, long endStateTicks)
		{
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId,
				ZhanDouLi = kuaFuRoleData.ZhanDouLi
			};
			YongZheZhanChangGameFuBenPreAssignData PreAssignData;
			if (!this.PreAssignGameFuBenDataDict.TryGetValue(kuaFuRoleData.GroupIndex, out PreAssignData))
			{
				PreAssignData = new YongZheZhanChangGameFuBenPreAssignData();
				this.PreAssignGameFuBenDataDict.Add(kuaFuRoleData.GroupIndex, PreAssignData);
			}
			if (null == PreAssignData.RemainFuBenData)
			{
				PreAssignData.RemainFuBenData = new YongZheZhanChangFuBenData
				{
					GroupIndex = kuaFuRoleData.GroupIndex
				};
			}
			int roleCount = PreAssignData.RemainFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, false);
			if (roleCount >= this.Persistence.MinEnterCount)
			{
				PreAssignData.FullFuBenDataList.Add(PreAssignData.RemainFuBenData);
				PreAssignData.RemainFuBenData = null;
			}
			kuaFuRoleData.UpdateStateTime(0, KuaFuRoleStates.SignUpWaiting, endStateTicks);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00063614 File Offset: 0x00061814
		private void AssignGameFubenStep2()
		{
			List<int> keyList = this.PreAssignGameFuBenDataDict.Keys.Reverse<int>().ToList<int>();
			int count = keyList.Count;
			for (int i = 0; i < count; i++)
			{
				YongZheZhanChangGameFuBenPreAssignData previous = null;
				YongZheZhanChangGameFuBenPreAssignData current = null;
				current = this.PreAssignGameFuBenDataDict[keyList[i]];
				if (i > 0)
				{
					previous = this.PreAssignGameFuBenDataDict[keyList[i - 1]];
				}
				if (previous != null && previous.RemainFuBenData != null)
				{
					int fc = current.FullFuBenDataList.Count;
					int rc = previous.RemainFuBenData.RoleDict.Count;
					List<int> roleList = previous.RemainFuBenData.RoleDict.Keys.ToList<int>();
					if (fc > 0)
					{
						for (int j = 0; j < fc; j++)
						{
							YongZheZhanChangFuBenData fuBenData = current.FullFuBenDataList[j];
							for (int k = j; k < rc; k += fc)
							{
								fuBenData.AddKuaFuFuBenRoleData(previous.RemainFuBenData.RoleDict[roleList[k]], true);
							}
						}
					}
					else if (null != current.RemainFuBenData)
					{
						foreach (KuaFuFuBenRoleData r in previous.RemainFuBenData.RoleDict.Values)
						{
							if (current.RemainFuBenData.AddKuaFuFuBenRoleData(r, false) >= this.Persistence.MinEnterCount)
							{
								YongZheZhanChangFuBenData tmp = new YongZheZhanChangFuBenData
								{
									GroupIndex = current.RemainFuBenData.GroupIndex
								};
								current.FullFuBenDataList.Add(current.RemainFuBenData);
								current.RemainFuBenData = tmp;
							}
						}
					}
					else
					{
						current.RemainFuBenData = previous.RemainFuBenData;
						current.RemainFuBenData.GroupIndex = keyList[i];
					}
					previous.RemainFuBenData = null;
				}
				if (current.RemainFuBenData != null && i == count - 1 && current.RemainFuBenData.GetFuBenRoleCount() > 0)
				{
					if (current.RemainFuBenData.GetFuBenRoleCount() >= this.Persistence.MinEnterCount)
					{
						current.FullFuBenDataList.Add(current.RemainFuBenData);
					}
					else if (current.FullFuBenDataList.Count <= 0)
					{
						current.FullFuBenDataList.Add(current.RemainFuBenData);
					}
					else
					{
						int fc = current.FullFuBenDataList.Count;
						int rc = current.RemainFuBenData.RoleDict.Count;
						List<int> roleList = current.RemainFuBenData.RoleDict.Keys.ToList<int>();
						for (int j = 0; j < fc; j++)
						{
							YongZheZhanChangFuBenData fuBenData = current.FullFuBenDataList[j];
							for (int k = j; k < rc; k += fc)
							{
								fuBenData.AddKuaFuFuBenRoleData(current.RemainFuBenData.RoleDict[roleList[k]], true);
							}
						}
					}
					current.RemainFuBenData = null;
				}
			}
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x0006399C File Offset: 0x00061B9C
		private bool AssignGameFubenStep3(DateTime stateEndTime)
		{
			DateTime nextTime = TimeUtil.NowDateTime();
			int count = 0;
			foreach (YongZheZhanChangGameFuBenPreAssignData preAssignData in this.PreAssignGameFuBenDataDict.Values)
			{
				foreach (YongZheZhanChangFuBenData fubenData in preAssignData.FullFuBenDataList)
				{
					if (fubenData.State < GameFuBenState.Start)
					{
						if (fubenData.GetFuBenRoleCount() > 0)
						{
							if (!this.CreateGameFuBenOnServer(fubenData, stateEndTime))
							{
								return false;
							}
							count++;
							if (count >= 15)
							{
								count = 0;
								DateTime now = TimeUtil.NowDateTime();
								if (now < nextTime)
								{
									Thread.Sleep((int)(nextTime - now).TotalMilliseconds);
								}
								else
								{
									nextTime = now.AddSeconds(1.0);
								}
							}
						}
					}
				}
				preAssignData.FullFuBenDataList.Clear();
				if (preAssignData.RemainFuBenData != null && preAssignData.RemainFuBenData.GetFuBenRoleCount() > 0)
				{
					if (preAssignData.RemainFuBenData.State < GameFuBenState.Start)
					{
						if (!this.CreateGameFuBenOnServer(preAssignData.RemainFuBenData, stateEndTime))
						{
							return false;
						}
					}
					preAssignData.RemainFuBenData = null;
				}
			}
			return true;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00063B78 File Offset: 0x00061D78
		private bool CreateGameFuBenOnServer(YongZheZhanChangFuBenData fuBenData, DateTime stateEndTime)
		{
			try
			{
				int gameId = this.Persistence.GetNextGameId();
				int kfSrvId = 0;
				bool createSuccess = ClientAgentManager.Instance().AssginKfFuben(this.YzzcGameType, (long)gameId, fuBenData.GetFuBenRoleCount(), out kfSrvId);
				if (createSuccess)
				{
					fuBenData.ServerId = kfSrvId;
					fuBenData.GameId = gameId;
					fuBenData.EndTime = Global.NowTime.AddMinutes(65.0);
					this.AddGameFuBen(fuBenData);
					fuBenData.SortFuBenRoleList();
					foreach (KuaFuFuBenRoleData role in fuBenData.RoleDict.Values)
					{
						KuaFuRoleKey key = KuaFuRoleKey.Get(role.ServerId, role.RoleId);
						KuaFuRoleData kuaFuRoleDataTemp;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleDataTemp))
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("通知活动副本GameID={0}的角色{1}准备进入ServerID={2}开始游戏", fuBenData.GameId, kuaFuRoleDataTemp.RoleId, fuBenData.ServerId), null, true);
							kuaFuRoleDataTemp.UpdateStateTime(fuBenData.GameId, KuaFuRoleStates.NotifyEnterGame, stateEndTime.Ticks);
						}
					}
					fuBenData.State = GameFuBenState.Start;
					this.NotifyFuBenRoleEnterGame(fuBenData);
					this.Persistence.LogCreateYongZheFuBen(kfSrvId, gameId, 0, fuBenData.GetFuBenRoleCount());
					return true;
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("暂时没有可用的服务器可以给活动副本分配,稍后重试", new object[0]), null, true);
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00063D54 File Offset: 0x00061F54
		private void AddGameFuBen(YongZheZhanChangFuBenData fubenData)
		{
			this.FuBenDataDict[fubenData.GameId] = fubenData;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00063D6C File Offset: 0x00061F6C
		private void RemoveGameFuBen(YongZheZhanChangFuBenData fuBenData)
		{
			int gameId = fuBenData.GameId;
			YongZheZhanChangFuBenData tmpFuben;
			if (this.FuBenDataDict.TryRemove(gameId, out tmpFuben))
			{
				tmpFuben.State = GameFuBenState.End;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.YzzcGameType, tmpFuben.ServerId, (long)tmpFuben.GameId);
			lock (fuBenData)
			{
				foreach (KuaFuFuBenRoleData fuBenRoleData in fuBenData.RoleDict.Values)
				{
					KuaFuRoleData kuaFuRoleData;
					if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(fuBenRoleData.ServerId, fuBenRoleData.RoleId), out kuaFuRoleData))
					{
						if (kuaFuRoleData.GameId == gameId)
						{
							kuaFuRoleData.State = KuaFuRoleStates.None;
						}
					}
				}
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00063E88 File Offset: 0x00062088
		private void RemoveRoleFromFuBen(int gameId, int roleId)
		{
			if (gameId > 0)
			{
				YongZheZhanChangFuBenData fuBenData;
				if (this.FuBenDataDict.TryGetValue(gameId, out fuBenData))
				{
					lock (fuBenData)
					{
						fuBenData.RemoveKuaFuFuBenRoleData(roleId);
						if (fuBenData.CanRemove())
						{
							this.RemoveGameFuBen(fuBenData);
						}
					}
				}
			}
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00063F68 File Offset: 0x00062168
		public int ExecCommand(string[] args)
		{
			int result = 0;
			try
			{
				if (0 == string.Compare(args[0], "GameState", true))
				{
					if (args.Length >= 2)
					{
						int gameType = 5;
						int gameState = int.Parse(args[1]);
						if (args.Length >= 3)
						{
							int.TryParse(args[2], out gameType);
						}
						DateTime now = TimeUtil.NowDateTime();
						lock (this.Mutex)
						{
							if (gameState == 2 && (now - this.PrepareStartGameTime).TotalHours >= 1.0)
							{
								this.PrepareStartGameTime = now;
								if (this.GameState > gameState)
								{
									LogManager.WriteLog(LogTypes.Info, "ExecCommand Set GameState to ignore" + gameState, null, true);
									return -4;
								}
								this.GameState = gameState;
								this.RunTimeGameType = gameType;
								int num = gameType;
								switch (num)
								{
								case 5:
									this.Persistence.MinEnterCount = this.Persistence.YongZheZhanChangRoleCount;
									this.Persistence.ServerCapacityRate = 1;
									break;
								case 6:
									this.Persistence.MinEnterCount = this.Persistence.KuaFuBossRoleCount;
									this.Persistence.ServerCapacityRate = 2;
									break;
								default:
									if (num == 15)
									{
										this.Persistence.MinEnterCount = this.Persistence.KingOfBattleRoleCount;
										this.Persistence.ServerCapacityRate = 2;
									}
									break;
								}
								LogManager.WriteLog(LogTypes.Info, "ExecCommand Set GameState to " + gameState, null, true);
							}
						}
					}
				}
				else if (0 == string.Compare(args[0], "-settime", true))
				{
					if (KuaFuServerManager.EnableGMSetAllServerTime && args.Length >= 3)
					{
						string datetimeStr = args[2];
						if (args.Length > 3)
						{
							datetimeStr = datetimeStr + " " + args[3];
						}
						ThreadPool.QueueUserWorkItem(delegate(object x)
						{
							Thread.Sleep(10000);
							string timeStr = x as string;
							if (!string.IsNullOrEmpty(timeStr))
							{
								TimeUtil.SetTime(timeStr);
								LogManager.WriteLog(LogTypes.Error, string.Format("GM命令修改时间#from={0},to={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), timeStr), null, true);
							}
						}, datetimeStr);
						this.Broadcast2GsAgent(new AsyncDataItem(KuaFuEventTypes.GMSetTime, args));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		// Token: 0x040003ED RID: 1005
		private const double CheckGameFuBenInterval = 1000.0;

		// Token: 0x040003EE RID: 1006
		private const double CheckRoleTimerProcInterval = 1.428;

		// Token: 0x040003EF RID: 1007
		public static YongZheZhanChangService Instance = null;

		// Token: 0x040003F0 RID: 1008
		private object Mutex = new object();

		// Token: 0x040003F1 RID: 1009
		public readonly GameTypes YzzcGameType = GameTypes.YongZheZhanChang;

		// Token: 0x040003F2 RID: 1010
		public readonly GameTypes LhlyGameType = GameTypes.LangHunLingYu;

		// Token: 0x040003F3 RID: 1011
		private DateTime CheckGameFuBenTime;

		// Token: 0x040003F4 RID: 1012
		private DateTime CheckRoleTimerProcTime;

		// Token: 0x040003F5 RID: 1013
		public YongZheZhanChangPersistence Persistence = YongZheZhanChangPersistence.Instance;

		// Token: 0x040003F6 RID: 1014
		public int[] OtherCityLevelList = new int[]
		{
			-1,
			0,
			-1,
			1,
			-1,
			2,
			3,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x040003F7 RID: 1015
		public Dictionary<int, List<int>> OtherCityList = new Dictionary<int, List<int>>();

		// Token: 0x040003F8 RID: 1016
		public int ExistKuaFuFuBenCount = 0;

		// Token: 0x040003F9 RID: 1017
		private int EnterGameSecs = 3600;

		// Token: 0x040003FA RID: 1018
		private int GameState = 1;

		// Token: 0x040003FB RID: 1019
		private bool AssginGameFuBenComplete = false;

		// Token: 0x040003FC RID: 1020
		private int RunTimeGameType;

		// Token: 0x040003FD RID: 1021
		private DateTime PrepareStartGameTime = DateTime.MinValue;

		// Token: 0x040003FE RID: 1022
		public ConcurrentDictionary<int, YongZheZhanChangFuBenData> FuBenDataDict = new ConcurrentDictionary<int, YongZheZhanChangFuBenData>(1, 4096);

		// Token: 0x040003FF RID: 1023
		private SortedDictionary<int, YongZheZhanChangGameFuBenPreAssignData> PreAssignGameFuBenDataDict = new SortedDictionary<int, YongZheZhanChangGameFuBenPreAssignData>();

		// Token: 0x04000400 RID: 1024
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		// Token: 0x04000401 RID: 1025
		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		// Token: 0x04000402 RID: 1026
		private ConcurrentDictionary<int, KuaFuMapRoleData> RoleId2KuaFuMapIdDict = new ConcurrentDictionary<int, KuaFuMapRoleData>();

		// Token: 0x04000403 RID: 1027
		public Thread BackgroundThread;

		// Token: 0x04000404 RID: 1028
		public static char[] WriteSpaceChars = new char[]
		{
			' '
		};

		// Token: 0x04000405 RID: 1029
		public Dictionary<long, LangHunLingYuBangHuiDataEx> LangHunLingYuBangHuiDataExDict = new Dictionary<long, LangHunLingYuBangHuiDataEx>();

		// Token: 0x04000406 RID: 1030
		public LangHunLingYuCityDataEx[] LangHunLingYuCityDataExDict = new LangHunLingYuCityDataEx[1024];

		// Token: 0x04000407 RID: 1031
		public List<LangHunLingYuKingHist> LangHunLingYuCityHistList = new List<LangHunLingYuKingHist>();

		// Token: 0x04000408 RID: 1032
		public Dictionary<int, int> LangHunLingYuAdmireDict = new Dictionary<int, int>();

		// Token: 0x04000409 RID: 1033
		public Dictionary<int, LangHunLingYuFuBenData> LangHunLingYuFuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();
	}
}
