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
using Remoting;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, UseSynchronizationContext = true)]
	public class TianTiService : MarshalByRefObject, ITianTiService, IExecCommand
	{
		
		public override object InitializeLifetimeService()
		{
			TianTiService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		
		public TianTiService()
		{
			TianTiService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		
		~TianTiService()
		{
			this.BackgroundThread.Abort();
		}

		
		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					if (now > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = now.AddSeconds(1.428);
						int signUpCnt;
						int startCnt;
						this.CheckRoleTimerProc(now, out signUpCnt, out startCnt);
						ClientAgentManager.Instance().SetGameTypeLoad(this.GameType, signUpCnt, startCnt);
					}
					if (now > this.SaveServerStateProcTime)
					{
						this.SaveServerStateProcTime = now.AddSeconds(30.0);
						if (now.Hour >= 3 && now.Hour < 4)
						{
							this.ClearRolePairFightCount();
							this.Persistence.UpdateTianTiRankData(now, false, false);
						}
					}
					if (now > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = now.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(now);
					}
					AsyncDataItem[] asyncEvArray = ZhengBaManagerK.Instance().Update();
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, asyncEvArray);
					this.Persistence.WriteRoleInfoDataProc();
					CoupleArenaService.getInstance().Update();
					CoupleWishService.getInstance().Update();
					zhengDuoService.Instance().Update(now);
					BangHuiMatchService.Instance().Update(now);
					CompService.Instance().Update(now);
					TianTi5v5Service.ThreadProc(null);
					Zork5v5Service.Instance().Update(now);
					int sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
					this.Persistence.SaveCostTime(sleepMS);
					sleepMS = 1000 - sleepMS;
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

		
		public RangeKey GetAssignRange(int baseValue, long startTicks, long waitTicks1, long waitTicks3, long waitTicksAll)
		{
			int rangeIndex;
			if (startTicks > waitTicks3)
			{
				if (startTicks > waitTicks1)
				{
					rangeIndex = 0;
				}
				else
				{
					rangeIndex = 1;
				}
			}
			else if (startTicks > waitTicksAll)
			{
				rangeIndex = 2;
			}
			else
			{
				rangeIndex = 3;
			}
			int expend = this.AssignRangeArray[rangeIndex];
			return new RangeKey(baseValue - expend, baseValue + expend, null);
		}

		
		private void CheckRoleTimerProc(DateTime now, out int signUpCnt, out int startCount)
		{
			signUpCnt = 0;
			startCount = 0;
			bool assgionGameFuBen = true;
			long maxRemoveRoleTicks = now.AddHours(-2.0).Ticks;
			long waitTicks = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs1)).Ticks;
			long waitTicks2 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs3)).Ticks;
			long waitTicksAll = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecsAll)).Ticks;
			long waitTicksMax = now.AddSeconds((double)(-(double)this.Persistence.WaitForJoinMaxSecs)).Ticks;
			this.ProcessTianTiFuBenDataDict.Clear();
			foreach (KuaFuRoleData kuaFuRoleData in this.RoleIdKuaFuRoleDataDict.Values)
			{
				int oldGameId = 0;
				lock (kuaFuRoleData)
				{
					if (kuaFuRoleData.State == KuaFuRoleStates.None || kuaFuRoleData.State > KuaFuRoleStates.StartGame)
					{
						if (kuaFuRoleData.StateEndTicks < maxRemoveRoleTicks)
						{
							KuaFuRoleData kuaFuRoleDataTemp;
							this.RoleIdKuaFuRoleDataDict.TryRemove(KuaFuRoleKey.Get(kuaFuRoleData.ServerId, kuaFuRoleData.RoleId), out kuaFuRoleDataTemp);
							continue;
						}
					}
					else if (kuaFuRoleData.State == KuaFuRoleStates.NotifyEnterGame || kuaFuRoleData.State == KuaFuRoleStates.EnterGame)
					{
						if (kuaFuRoleData.StateEndTicks < now.Ticks)
						{
							kuaFuRoleData.Age++;
							kuaFuRoleData.State = KuaFuRoleStates.None;
							oldGameId = kuaFuRoleData.GameId;
						}
					}
					else if (kuaFuRoleData.State == KuaFuRoleStates.SignUp)
					{
						if (kuaFuRoleData.StateEndTicks < waitTicksMax)
						{
							kuaFuRoleData.Age++;
							kuaFuRoleData.State = KuaFuRoleStates.None;
						}
					}
				}
				if (kuaFuRoleData.State == KuaFuRoleStates.SignUp)
				{
					signUpCnt++;
					if (assgionGameFuBen)
					{
						RangeKey range = this.GetAssignRange(kuaFuRoleData.GroupIndex, kuaFuRoleData.StateEndTicks, waitTicks, waitTicks2, waitTicksAll);
						assgionGameFuBen = this.AssignGameFuben(kuaFuRoleData, range, now);
					}
				}
				else if (kuaFuRoleData.State == KuaFuRoleStates.SignUpWaiting)
				{
					signUpCnt++;
				}
				else if (kuaFuRoleData.State == KuaFuRoleStates.StartGame)
				{
					startCount++;
				}
				if (oldGameId > 0)
				{
					this.RemoveRoleFromFuBen(oldGameId, kuaFuRoleData.RoleId);
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.RoleStateChange, new object[]
					{
						kuaFuRoleData
					});
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
				}
			}
		}

		
		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.TianTiFuBenDataDict.Count > 0)
			{
				DateTime canRemoveTime = now.AddMinutes(-8.0);
				foreach (TianTiFuBenData fuBenData in this.TianTiFuBenDataDict.Values)
				{
					lock (fuBenData)
					{
						if (fuBenData.CanRemove())
						{
							this.RemoveGameFuBen(fuBenData);
						}
						else if (fuBenData.EndTime < canRemoveTime)
						{
							this.RemoveGameFuBen(fuBenData);
						}
					}
				}
			}
		}

		
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 2 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1}", clientInfo.ServerId, clientInfo.ClientId));
				result = -11003;
			}
			return result;
		}

		
		public int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData)
		{
			int result = 1;
			int rid;
			KuaFuRoleData kuaFuRoleData;
			if (!this.UserId2RoleIdActiveDict.TryGetValue(userId, out rid))
			{
				this.UserId2RoleIdActiveDict[userId] = roleId;
			}
			else if (rid > 0 && rid != roleId)
			{
				if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, rid), out kuaFuRoleData))
				{
					if (this.ChangeRoleState(kuaFuRoleData, KuaFuRoleStates.None) < 0)
					{
					}
				}
				this.UserId2RoleIdActiveDict[userId] = roleId;
			}
			Lazy<KuaFuRoleData> lazy = new Lazy<KuaFuRoleData>(() => new KuaFuRoleData
			{
				RoleId = roleId,
				UserId = userId,
				GameType = gameType
			});
			KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
			kuaFuRoleData = this.RoleIdKuaFuRoleDataDict.GetOrAdd(key, (KuaFuRoleKey x) => lazy.Value);
			int oldGameId = 0;
			lock (kuaFuRoleData)
			{
				oldGameId = kuaFuRoleData.GameId;
				kuaFuRoleData.GameId = 0;
				kuaFuRoleData.Age++;
				kuaFuRoleData.State = KuaFuRoleStates.SignUp;
				kuaFuRoleData.ServerId = serverId;
				kuaFuRoleData.ZoneId = zoneId;
				kuaFuRoleData.GameData = gameData;
				kuaFuRoleData.GroupIndex = groupIndex;
				kuaFuRoleData.StateEndTicks = Global.NowTime.Ticks;
			}
			if (oldGameId > 0)
			{
				this.RemoveRoleFromFuBen(oldGameId, roleId);
			}
			return result;
		}

		
		public int RoleChangeState(int serverId, int roleId, int state)
		{
			KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
			KuaFuRoleData kuaFuRoleData;
			int result;
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
						if (kuaFuRoleData.GameId > 0)
						{
							TianTiFuBenData fuBenData;
							if (this.TianTiFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out fuBenData))
							{
								AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.CopyCanceled, new object[]
								{
									kuaFuRoleData.GameId
								});
								ClientAgentManager.Instance().PostAsyncEvent(fuBenData.ServerId, this.GameType, evItem);
							}
						}
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
			return result;
		}

		
		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state)
		{
			TianTiFuBenData tianTiFuBenData;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				lock (tianTiFuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (tianTiFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
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

		
		public int GetRoleExtendData(int serverId, int roleId, int dataType)
		{
			KuaFuRoleData kuaFuRoleData = null;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData))
			{
				result = 0;
			}
			else if (dataType == 0)
			{
				int roleCount = 0;
				if (kuaFuRoleData.State == KuaFuRoleStates.SignUp)
				{
					roleCount = 1;
				}
				if (kuaFuRoleData.GameId > 0)
				{
					TianTiFuBenData tianTiFuBenData;
					if (this.TianTiFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out tianTiFuBenData))
					{
						if (tianTiFuBenData.State < GameFuBenState.Start)
						{
							roleCount = tianTiFuBenData.GetFuBenRoleCount();
						}
						else
						{
							this.RemoveRoleFromFuBen(kuaFuRoleData.GameId, roleId);
							this.RoleChangeState(serverId, roleId, 0);
							roleCount = 0;
						}
					}
				}
				result = roleCount;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		
		public TianTiFuBenData GetFuBenData(int gameId)
		{
			TianTiFuBenData kuaFuFuBenData = null;
			TianTiFuBenData result;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out kuaFuFuBenData) && kuaFuFuBenData.State < GameFuBenState.End)
			{
				result = kuaFuFuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11;
			TianTiFuBenData tianTiFuBenData = null;
			int result2;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				this.AddRolePairFightCount(tianTiFuBenData);
				lock (tianTiFuBenData)
				{
					tianTiFuBenData.State = state;
					if (state == GameFuBenState.End)
					{
						this.RemoveGameFuBen(tianTiFuBenData);
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

		
		public TianTiRankData GetRankingData(DateTime modifyTime)
		{
			return this.Persistence.GetTianTiRankData(modifyTime);
		}

		
		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			this.Persistence.UpdateRoleInfoData(data);
		}

		
		public ZhengBaSyncData SyncZhengBaData(ZhengBaSyncData lastSyncData)
		{
			return ZhengBaManagerK.Instance().SyncZhengBaData(lastSyncData);
		}

		
		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			return ZhengBaManagerK.Instance().ZhengBaSupport(data);
		}

		
		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			return ZhengBaManagerK.Instance().ZhengBaRequestEnter(roleId, gameId, enter);
		}

		
		public int ZhengBaKuaFuLogin(int roleid, int gameId)
		{
			return ZhengBaManagerK.Instance().ZhengBaKuaFuLogin(roleid, gameId);
		}

		
		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner, int FirstLeaveRoleId)
		{
			return ZhengBaManagerK.Instance().ZhengBaPkResult(gameId, winner, FirstLeaveRoleId);
		}

		
		private int ChangeRoleState(KuaFuRoleData kuaFuRoleData, KuaFuRoleStates state)
		{
			int result = -1;
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
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
				}
			}
			catch (Exception ex)
			{
				return -1;
			}
			return result;
		}

		
		private void NotifyFuBenRoleCount(TianTiFuBenData fuBenData)
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
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
						}
					}
				}
			}
			catch
			{
			}
		}

		
		private void NotifyFuBenRoleEnterGame(TianTiFuBenData fuBenData)
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
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		private long MakeRolePairKey(int roleId1, int roleId2)
		{
			long rolePairKey;
			if (roleId1 < roleId2)
			{
				rolePairKey = ((long)roleId1 << 32) + (long)roleId2;
			}
			else
			{
				rolePairKey = ((long)roleId2 << 32) + (long)roleId1;
			}
			return rolePairKey;
		}

		
		private void ClearRolePairFightCount()
		{
			lock (this.RolePairFightCountDict)
			{
				this.RolePairFightCountDict.Clear();
			}
		}

		
		private void AddRolePairFightCount(TianTiFuBenData tianTiFuBenData)
		{
			int roleId = 0;
			int roleId2 = 0;
			if (tianTiFuBenData.RoleDict.Count >= 2)
			{
				foreach (KuaFuFuBenRoleData role in tianTiFuBenData.RoleDict.Values)
				{
					if (roleId == 0)
					{
						roleId = role.RoleId;
					}
					else
					{
						roleId2 = role.RoleId;
					}
				}
				long rolePairKey = this.MakeRolePairKey(roleId, roleId2);
				lock (this.RolePairFightCountDict)
				{
					int fightCount;
					if (!this.RolePairFightCountDict.TryGetValue(rolePairKey, out fightCount))
					{
						this.RolePairFightCountDict[rolePairKey] = 1;
					}
					else
					{
						this.RolePairFightCountDict[rolePairKey] = fightCount + 1;
					}
				}
			}
		}

		
		private bool CanAddFuBenRole(TianTiFuBenData tianTiFuBenData, KuaFuRoleData kuaFuRoleData)
		{
			bool result;
			if (tianTiFuBenData.RoleDict.Count == 0)
			{
				result = true;
			}
			else
			{
				KuaFuFuBenRoleData existRole = tianTiFuBenData.RoleDict.Values.FirstOrDefault<KuaFuFuBenRoleData>();
				long rolePairKey = this.MakeRolePairKey(kuaFuRoleData.RoleId, existRole.RoleId);
				lock (this.RolePairFightCountDict)
				{
					int fightCount;
					if (!this.RolePairFightCountDict.TryGetValue(rolePairKey, out fightCount) || fightCount < this.Persistence.MaxRolePairFightCount)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		
		private bool AssignGameFuben(KuaFuRoleData kuaFuRoleData, RangeKey range, DateTime now)
		{
			DateTime stateEndTime = now.AddSeconds((double)this.EnterGameSecs);
			TianTiFuBenData tianTiFuBenData = null;
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId
			};
			List<KuaFuRoleData> updateRoleDataList = new List<KuaFuRoleData>();
			if (!this.ProcessTianTiFuBenDataDict.TryGetValue(range, out tianTiFuBenData))
			{
				tianTiFuBenData = new TianTiFuBenData();
				this.ProcessTianTiFuBenDataDict.Add(range, tianTiFuBenData);
			}
			else if (!this.CanAddFuBenRole(tianTiFuBenData, kuaFuRoleData))
			{
				return true;
			}
			int roleCount = tianTiFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData);
			bool result;
			if (roleCount < Consts.TianTiRoleCountTotal)
			{
				result = true;
			}
			else
			{
				try
				{
					int kfSrvId = 0;
					int gameId = this.Persistence.GetNextGameId();
					bool createSuccess = ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)gameId, roleCount, out kfSrvId);
					if (createSuccess)
					{
						tianTiFuBenData.ServerId = kfSrvId;
						tianTiFuBenData.GameId = gameId;
						tianTiFuBenData.EndTime = Global.NowTime.AddMinutes(8.0);
						this.AddGameFuBen(tianTiFuBenData);
						this.Persistence.LogCreateTianTiFuBen(tianTiFuBenData.GameId, tianTiFuBenData.ServerId, 0, roleCount);
						foreach (KuaFuFuBenRoleData role in tianTiFuBenData.RoleDict.Values)
						{
							KuaFuRoleKey key = KuaFuRoleKey.Get(role.ServerId, role.RoleId);
							KuaFuRoleData kuaFuRoleDataTemp;
							if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleDataTemp))
							{
								kuaFuRoleDataTemp.UpdateStateTime(tianTiFuBenData.GameId, KuaFuRoleStates.NotifyEnterGame, stateEndTime.Ticks);
							}
						}
						tianTiFuBenData.State = GameFuBenState.Start;
						this.NotifyFuBenRoleEnterGame(tianTiFuBenData);
						this.ProcessTianTiFuBenDataDict.Remove(range);
						return true;
					}
					return false;
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				result = false;
			}
			return result;
		}

		
		private void AddGameFuBen(TianTiFuBenData tianTiFuBenData)
		{
			this.TianTiFuBenDataDict[tianTiFuBenData.GameId] = tianTiFuBenData;
		}

		
		private void RemoveGameFuBen(TianTiFuBenData tianTiFuBenData)
		{
			int gameId = tianTiFuBenData.GameId;
			TianTiFuBenData tianTiFuBenDataTemp;
			if (this.TianTiFuBenDataDict.TryRemove(gameId, out tianTiFuBenDataTemp))
			{
				tianTiFuBenDataTemp.State = GameFuBenState.End;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, tianTiFuBenData.ServerId, (long)tianTiFuBenData.GameId);
			lock (tianTiFuBenData)
			{
				foreach (KuaFuFuBenRoleData fuBenRoleData in tianTiFuBenData.RoleDict.Values)
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

		
		private void RemoveRoleFromFuBen(int gameId, int roleId)
		{
			if (gameId > 0)
			{
				TianTiFuBenData tianTiFuBenData;
				if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
				{
					lock (tianTiFuBenData)
					{
						int count = tianTiFuBenData.RemoveKuaFuFuBenRoleData(roleId);
						if (tianTiFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(tianTiFuBenData);
						}
						else if (tianTiFuBenData.State < GameFuBenState.Start)
						{
							this.NotifyFuBenRoleCount(tianTiFuBenData);
						}
					}
				}
			}
		}

		
		public int ExecCommand(string[] args)
		{
			int result = -1;
			try
			{
				if (string.Compare(args[0], "reload") == 0 && 0 == string.Compare(args[1], "paihang"))
				{
					bool monthRank = false;
					if (args.Length >= 3)
					{
						monthRank = true;
					}
					this.Persistence.UpdateTianTiRankData(TimeUtil.NowDateTime(), monthRank, true);
				}
				else if (0 == string.Compare(args[0], "load"))
				{
					this.Persistence.LoadTianTiRankData(TimeUtil.NowDateTime());
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			return CoupleArenaService.getInstance().CoupleArenaJoin(roleId1, roleId2, serverId);
		}

		
		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaQuit(roleId1, roleId2);
		}

		
		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			return CoupleArenaService.getInstance().CoupleArenaSync(lastSyncTime);
		}

		
		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaPreDivorce(roleId1, roleId2);
		}

		
		public CoupleArenaFuBenData GetCoupleFuBenData(long gameId)
		{
			return CoupleArenaService.getInstance().GetCoupleFuBenData(gameId);
		}

		
		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			return CoupleArenaService.getInstance().CoupleArenaPkResult(req);
		}

		
		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			return CoupleWishService.getInstance().CoupleWishWishRole(req);
		}

		
		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			return CoupleWishService.getInstance().CoupleWishGetWishRecord(roleId);
		}

		
		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			return CoupleWishService.getInstance().CoupleWishSyncCenterData(oldThisWeek, oldLastWeek, oldStatue);
		}

		
		public int CoupleWishPreDivorce(int man, int wife)
		{
			return CoupleWishService.getInstance().CoupleWishPreDivorce(man, wife);
		}

		
		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			CoupleWishService.getInstance().CoupleWishReportCoupleStatue(req);
		}

		
		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishAdmire(fromRole, fromZone, admireType, toCoupleId);
		}

		
		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishJoinParty(fromRole, fromZone, toCoupleId);
		}

		
		public ZhengDuoSyncData ZhengDuoSync(int serverID, long version)
		{
			return zhengDuoService.Instance().ZhengDuoSync(serverID, version);
		}

		
		public int ZhengDuoSign(int serverID, int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			return zhengDuoService.Instance().ZhengDuoSign(serverID, bhid, usedTime, zoneId, bhName, bhLevel, bhZhanLi);
		}

		
		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			return zhengDuoService.Instance().ZhengDuoResult(bhidSuccess, bhids);
		}

		
		public int GmCommand(string[] args, byte[] data)
		{
			return zhengDuoService.Instance().GmCommand(args, data);
		}

		
		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenDataByBhid(bhid);
		}

		
		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenData(gameId);
		}

		
		public KuaFuCmdData GetBHDataByBhid_BHMatch(int type, int bhid, long age)
		{
			return BangHuiMatchService.Instance().GetBHDataByBhid_BHMatch(type, bhid, age);
		}

		
		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			return BangHuiMatchService.Instance().SyncData_BHMatch(ageRank, agePKInfo, ageChampion);
		}

		
		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetKuaFuGameState_BHMatch(bhid);
		}

		
		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().CheckRookieJoinLast_BHMatch(bhid);
		}

		
		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			return BangHuiMatchService.Instance().RookieSignUp_BHMatch(bhid, zoneid_bh, bhname, rid, rname, zoneid_r);
		}

		
		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByBhid_BHMatch(bhid);
		}

		
		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int gameid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByGameId_BHMatch(gameid);
		}

		
		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			return BangHuiMatchService.Instance().GameFuBenComplete_BHMatch(data);
		}

		
		public int RemoveBangHui_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().RemoveBangHui_BHMatch(bhid);
		}

		
		public void SwitchLastGoldBH_GM()
		{
			BangHuiMatchService.Instance().SwitchLastGoldBH_GM();
		}

		
		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			return CompService.Instance().Comp_SyncData(ageComp, ageRankJX, ageRankJXL, ageRankBD, ageRankBJF, ageRankMJF);
		}

		
		public KuaFuCmdData Comp_GetCompRoleData(int roleId, long dataAge)
		{
			return CompService.Instance().GetCompRoleData(roleId, dataAge);
		}

		
		public void Comp_ChangeName(int roleId, string roleName)
		{
			CompService.Instance().ChangeName(roleId, roleName);
		}

		
		public int Comp_JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			return CompService.Instance().JoinComp(roleId, zoneId, roleName, compType);
		}

		
		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			return CompService.Instance().Comp_JoinComp_Repair(roleId, zoneId, roleName, compType, battleJiFen);
		}

		
		public void Comp_CompOpt(int compType, int optType, int param1, int param2)
		{
			CompService.Instance().CompOpt(compType, optType, param1, param2);
		}

		
		public void Comp_SetBulletin(int compType, string bulletin)
		{
			CompService.Instance().SetBulletin(compType, bulletin);
		}

		
		public void Comp_BroadCastCompNotice(int serverId, byte[] bytes)
		{
			CompService.Instance().BroadCastCompNotice(serverId, bytes);
		}

		
		public void Comp_CompChat(int serverId, byte[] bytes)
		{
			CompService.Instance().CompChat(serverId, bytes);
		}

		
		public void Comp_SetRoleData4Selector(int roleId, byte[] bytes)
		{
			CompService.Instance().SetRoleData4Selector(roleId, bytes);
		}

		
		public void Comp_UpdateMapRoleNum(int mapCode, int roleNum)
		{
			CompService.Instance().UpdateMapRoleNum(mapCode, roleNum);
		}

		
		public void Comp_UpdateFuBenMapRoleNum(int gameType, CompFuBenData fubenItem)
		{
			CompService.Instance().UpdateFuBenMapRoleNum(gameType, fubenItem);
		}

		
		public void Comp_UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			CompService.Instance().UpdateStrongholdData(cityID, shDataList);
		}

		
		public int Comp_GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			return CompService.Instance().GameFuBenRoleChangeState(gameType, serverId, cityID, roleId, zhiwu, state);
		}

		
		public KuaFuCmdData Comp_GetKuaFuFuBenData(int gameType, int cityID, long dataAge)
		{
			return CompService.Instance().GetKuaFuFuBenData(gameType, cityID, dataAge);
		}

		
		public int CreateZhanDui(int serverID, TianTi5v5ZhanDuiData pData)
		{
			return TianTi5v5Service.CreateZhanDui(serverID, pData);
		}

		
		public int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			return TianTi5v5Service.UpdateZhanDuiXuanYan(teamID, xuanYan);
		}

		
		public int DeleteZhanDui(int serverID, int roleid, int teamID)
		{
			return TianTi5v5Service.DeleteZhanDui(serverID, roleid, teamID);
		}

		
		public int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			return TianTi5v5Service.UpdateZhanDuiData(data, modeType);
		}

		
		public int ZhanDuiRoleSignUp(int serverId, int gameType, int teamID, long zhanLi, int groupIndex)
		{
			return TianTi5v5Service.ZhanDuiRoleSignUp(serverId, gameType, teamID, zhanLi, groupIndex);
		}

		
		public int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID)
		{
			return TianTi5v5Service.ZhanDuiRoleChangeState(serverId, zhanDuiID, roleId, state, gameID);
		}

		
		public KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			return TianTi5v5Service.ZhanDuiGetFuBenData(gameId);
		}

		
		public int ZhanDuiGameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11;
			TianTiFuBenData tianTiFuBenData = null;
			int result2;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				this.AddRolePairFightCount(tianTiFuBenData);
				lock (tianTiFuBenData)
				{
					tianTiFuBenData.State = state;
					if (state == GameFuBenState.End)
					{
						this.RemoveGameFuBen(tianTiFuBenData);
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

		
		public TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			return TianTi5v5Service.ZhanDuiGetRankingData(modifyTime);
		}

		
		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int gameid)
		{
			return Zork5v5Service.Instance().GetFuBenDataByGameId_ZorkBattle(gameid);
		}

		
		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			return Zork5v5Service.Instance().GetFuBenDataByZhanDuiId_ZorkBattle(ZhanDuiId);
		}

		
		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			return Zork5v5Service.Instance().GetKuaFuGameState_ZorkBattle(zhanduiID);
		}

		
		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			return Zork5v5Service.Instance().SyncData_ZorkBattle(gsTicks, ageRank);
		}

		
		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			return Zork5v5Service.Instance().SignUp_ZorkBattle(zhanduiID, serverID);
		}

		
		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			return Zork5v5Service.Instance().GameFuBenComplete_ZorkBattle(data);
		}

		
		public int ExcuteGMCmd(int serverID, int rid, string[] cmd)
		{
			return 0;
		}

		
		private const double CheckGameFuBenInterval = 1000.0;

		
		private const double CheckRoleTimerProcInterval = 1.428;

		
		private const double SaveServerStateProcInterval = 30.0;

		
		public static TianTiService Instance = null;

		
		private object Mutex = new object();

		
		public readonly GameTypes GameType = GameTypes.TianTi;

		
		private DateTime CheckGameFuBenTime;

		
		private DateTime CheckRoleTimerProcTime;

		
		private DateTime SaveServerStateProcTime;

		
		public TianTiPersistence Persistence = TianTiPersistence.Instance;

		
		public int ExistKuaFuFuBenCount = 0;

		
		private int EnterGameSecs = 20;

		
		public int DisconnectionProtectSecs = 15;

		
		public int GameEndProtectSecs = 15;

		
		private int MaxServerLoad = 400;

		
		private int MaxTianTiDayCount = 1000;

		
		private int TianTiDayCount = 0;

		
		private int TianTiDayId = 0;

		
		public int[] AssignRangeArray = new int[]
		{
			0,
			1,
			2,
			100
		};

		
		public ConcurrentDictionary<int, TianTiFuBenData> TianTiFuBenDataDict = new ConcurrentDictionary<int, TianTiFuBenData>(1, 4096);

		
		private SortedDictionary<RangeKey, TianTiFuBenData> ProcessTianTiFuBenDataDict = new SortedDictionary<RangeKey, TianTiFuBenData>(RangeKey.Comparer);

		
		private SortedList<long, int> RolePairFightCountDict = new SortedList<long, int>();

		
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		
		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		
		public Thread BackgroundThread;
	}
}
