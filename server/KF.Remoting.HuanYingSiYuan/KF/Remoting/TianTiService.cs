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
	// Token: 0x02000083 RID: 131
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, UseSynchronizationContext = true)]
	public class TianTiService : MarshalByRefObject, ITianTiService, IExecCommand
	{
		// Token: 0x06000680 RID: 1664 RVA: 0x000593E0 File Offset: 0x000575E0
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

		// Token: 0x06000681 RID: 1665 RVA: 0x00059440 File Offset: 0x00057640
		public TianTiService()
		{
			TianTiService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0005954C File Offset: 0x0005774C
		~TianTiService()
		{
			this.BackgroundThread.Abort();
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00059584 File Offset: 0x00057784
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

		// Token: 0x06000684 RID: 1668 RVA: 0x000597A8 File Offset: 0x000579A8
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

		// Token: 0x06000685 RID: 1669 RVA: 0x0005980C File Offset: 0x00057A0C
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

		// Token: 0x06000686 RID: 1670 RVA: 0x00059B50 File Offset: 0x00057D50
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

		// Token: 0x06000687 RID: 1671 RVA: 0x00059C4C File Offset: 0x00057E4C
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00059C70 File Offset: 0x00057E70
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

		// Token: 0x06000689 RID: 1673 RVA: 0x00059D84 File Offset: 0x00057F84
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

		// Token: 0x0600068A RID: 1674 RVA: 0x00059F5C File Offset: 0x0005815C
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

		// Token: 0x0600068B RID: 1675 RVA: 0x0005A090 File Offset: 0x00058290
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

		// Token: 0x0600068C RID: 1676 RVA: 0x0005A170 File Offset: 0x00058370
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

		// Token: 0x0600068D RID: 1677 RVA: 0x0005A1B4 File Offset: 0x000583B4
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

		// Token: 0x0600068E RID: 1678 RVA: 0x0005A284 File Offset: 0x00058484
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0005A29C File Offset: 0x0005849C
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

		// Token: 0x06000690 RID: 1680 RVA: 0x0005A2DC File Offset: 0x000584DC
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

		// Token: 0x06000691 RID: 1681 RVA: 0x0005A378 File Offset: 0x00058578
		public TianTiRankData GetRankingData(DateTime modifyTime)
		{
			return this.Persistence.GetTianTiRankData(modifyTime);
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0005A396 File Offset: 0x00058596
		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			this.Persistence.UpdateRoleInfoData(data);
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0005A3A8 File Offset: 0x000585A8
		public ZhengBaSyncData SyncZhengBaData(ZhengBaSyncData lastSyncData)
		{
			return ZhengBaManagerK.Instance().SyncZhengBaData(lastSyncData);
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0005A3C8 File Offset: 0x000585C8
		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			return ZhengBaManagerK.Instance().ZhengBaSupport(data);
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0005A3E8 File Offset: 0x000585E8
		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			return ZhengBaManagerK.Instance().ZhengBaRequestEnter(roleId, gameId, enter);
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0005A408 File Offset: 0x00058608
		public int ZhengBaKuaFuLogin(int roleid, int gameId)
		{
			return ZhengBaManagerK.Instance().ZhengBaKuaFuLogin(roleid, gameId);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0005A428 File Offset: 0x00058628
		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner, int FirstLeaveRoleId)
		{
			return ZhengBaManagerK.Instance().ZhengBaPkResult(gameId, winner, FirstLeaveRoleId);
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0005A448 File Offset: 0x00058648
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

		// Token: 0x06000699 RID: 1689 RVA: 0x0005A54C File Offset: 0x0005874C
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

		// Token: 0x0600069A RID: 1690 RVA: 0x0005A66C File Offset: 0x0005886C
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

		// Token: 0x0600069B RID: 1691 RVA: 0x0005A778 File Offset: 0x00058978
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

		// Token: 0x0600069C RID: 1692 RVA: 0x0005A7B0 File Offset: 0x000589B0
		private void ClearRolePairFightCount()
		{
			lock (this.RolePairFightCountDict)
			{
				this.RolePairFightCountDict.Clear();
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0005A804 File Offset: 0x00058A04
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

		// Token: 0x0600069E RID: 1694 RVA: 0x0005A91C File Offset: 0x00058B1C
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

		// Token: 0x0600069F RID: 1695 RVA: 0x0005A9E4 File Offset: 0x00058BE4
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

		// Token: 0x060006A0 RID: 1696 RVA: 0x0005AC20 File Offset: 0x00058E20
		private void AddGameFuBen(TianTiFuBenData tianTiFuBenData)
		{
			this.TianTiFuBenDataDict[tianTiFuBenData.GameId] = tianTiFuBenData;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0005AC38 File Offset: 0x00058E38
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

		// Token: 0x060006A2 RID: 1698 RVA: 0x0005AD54 File Offset: 0x00058F54
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

		// Token: 0x060006A3 RID: 1699 RVA: 0x0005ADFC File Offset: 0x00058FFC
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

		// Token: 0x060006A4 RID: 1700 RVA: 0x0005AEB8 File Offset: 0x000590B8
		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			return CoupleArenaService.getInstance().CoupleArenaJoin(roleId1, roleId2, serverId);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0005AED8 File Offset: 0x000590D8
		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaQuit(roleId1, roleId2);
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0005AEF8 File Offset: 0x000590F8
		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			return CoupleArenaService.getInstance().CoupleArenaSync(lastSyncTime);
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0005AF18 File Offset: 0x00059118
		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaPreDivorce(roleId1, roleId2);
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0005AF38 File Offset: 0x00059138
		public CoupleArenaFuBenData GetCoupleFuBenData(long gameId)
		{
			return CoupleArenaService.getInstance().GetCoupleFuBenData(gameId);
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0005AF58 File Offset: 0x00059158
		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			return CoupleArenaService.getInstance().CoupleArenaPkResult(req);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0005AF78 File Offset: 0x00059178
		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			return CoupleWishService.getInstance().CoupleWishWishRole(req);
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0005AF98 File Offset: 0x00059198
		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			return CoupleWishService.getInstance().CoupleWishGetWishRecord(roleId);
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0005AFB8 File Offset: 0x000591B8
		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			return CoupleWishService.getInstance().CoupleWishSyncCenterData(oldThisWeek, oldLastWeek, oldStatue);
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0005AFD8 File Offset: 0x000591D8
		public int CoupleWishPreDivorce(int man, int wife)
		{
			return CoupleWishService.getInstance().CoupleWishPreDivorce(man, wife);
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0005AFF6 File Offset: 0x000591F6
		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			CoupleWishService.getInstance().CoupleWishReportCoupleStatue(req);
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0005B008 File Offset: 0x00059208
		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishAdmire(fromRole, fromZone, admireType, toCoupleId);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0005B02C File Offset: 0x0005922C
		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishJoinParty(fromRole, fromZone, toCoupleId);
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0005B04C File Offset: 0x0005924C
		public ZhengDuoSyncData ZhengDuoSync(int serverID, long version)
		{
			return zhengDuoService.Instance().ZhengDuoSync(serverID, version);
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0005B06C File Offset: 0x0005926C
		public int ZhengDuoSign(int serverID, int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			return zhengDuoService.Instance().ZhengDuoSign(serverID, bhid, usedTime, zoneId, bhName, bhLevel, bhZhanLi);
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0005B094 File Offset: 0x00059294
		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			return zhengDuoService.Instance().ZhengDuoResult(bhidSuccess, bhids);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0005B0B4 File Offset: 0x000592B4
		public int GmCommand(string[] args, byte[] data)
		{
			return zhengDuoService.Instance().GmCommand(args, data);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0005B0D4 File Offset: 0x000592D4
		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenDataByBhid(bhid);
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0005B0F4 File Offset: 0x000592F4
		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenData(gameId);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0005B114 File Offset: 0x00059314
		public KuaFuCmdData GetBHDataByBhid_BHMatch(int type, int bhid, long age)
		{
			return BangHuiMatchService.Instance().GetBHDataByBhid_BHMatch(type, bhid, age);
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0005B134 File Offset: 0x00059334
		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			return BangHuiMatchService.Instance().SyncData_BHMatch(ageRank, agePKInfo, ageChampion);
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0005B154 File Offset: 0x00059354
		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetKuaFuGameState_BHMatch(bhid);
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0005B174 File Offset: 0x00059374
		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().CheckRookieJoinLast_BHMatch(bhid);
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0005B194 File Offset: 0x00059394
		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			return BangHuiMatchService.Instance().RookieSignUp_BHMatch(bhid, zoneid_bh, bhname, rid, rname, zoneid_r);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0005B1BC File Offset: 0x000593BC
		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByBhid_BHMatch(bhid);
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0005B1DC File Offset: 0x000593DC
		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int gameid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByGameId_BHMatch(gameid);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0005B1FC File Offset: 0x000593FC
		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			return BangHuiMatchService.Instance().GameFuBenComplete_BHMatch(data);
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0005B21C File Offset: 0x0005941C
		public int RemoveBangHui_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().RemoveBangHui_BHMatch(bhid);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0005B239 File Offset: 0x00059439
		public void SwitchLastGoldBH_GM()
		{
			BangHuiMatchService.Instance().SwitchLastGoldBH_GM();
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0005B248 File Offset: 0x00059448
		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			return CompService.Instance().Comp_SyncData(ageComp, ageRankJX, ageRankJXL, ageRankBD, ageRankBJF, ageRankMJF);
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0005B270 File Offset: 0x00059470
		public KuaFuCmdData Comp_GetCompRoleData(int roleId, long dataAge)
		{
			return CompService.Instance().GetCompRoleData(roleId, dataAge);
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0005B28E File Offset: 0x0005948E
		public void Comp_ChangeName(int roleId, string roleName)
		{
			CompService.Instance().ChangeName(roleId, roleName);
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0005B2A0 File Offset: 0x000594A0
		public int Comp_JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			return CompService.Instance().JoinComp(roleId, zoneId, roleName, compType);
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0005B2C4 File Offset: 0x000594C4
		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			return CompService.Instance().Comp_JoinComp_Repair(roleId, zoneId, roleName, compType, battleJiFen);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0005B2E7 File Offset: 0x000594E7
		public void Comp_CompOpt(int compType, int optType, int param1, int param2)
		{
			CompService.Instance().CompOpt(compType, optType, param1, param2);
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0005B2FA File Offset: 0x000594FA
		public void Comp_SetBulletin(int compType, string bulletin)
		{
			CompService.Instance().SetBulletin(compType, bulletin);
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0005B30A File Offset: 0x0005950A
		public void Comp_BroadCastCompNotice(int serverId, byte[] bytes)
		{
			CompService.Instance().BroadCastCompNotice(serverId, bytes);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0005B31A File Offset: 0x0005951A
		public void Comp_CompChat(int serverId, byte[] bytes)
		{
			CompService.Instance().CompChat(serverId, bytes);
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0005B32A File Offset: 0x0005952A
		public void Comp_SetRoleData4Selector(int roleId, byte[] bytes)
		{
			CompService.Instance().SetRoleData4Selector(roleId, bytes);
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0005B33A File Offset: 0x0005953A
		public void Comp_UpdateMapRoleNum(int mapCode, int roleNum)
		{
			CompService.Instance().UpdateMapRoleNum(mapCode, roleNum);
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x0005B34A File Offset: 0x0005954A
		public void Comp_UpdateFuBenMapRoleNum(int gameType, CompFuBenData fubenItem)
		{
			CompService.Instance().UpdateFuBenMapRoleNum(gameType, fubenItem);
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0005B35A File Offset: 0x0005955A
		public void Comp_UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			CompService.Instance().UpdateStrongholdData(cityID, shDataList);
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0005B36C File Offset: 0x0005956C
		public int Comp_GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			return CompService.Instance().GameFuBenRoleChangeState(gameType, serverId, cityID, roleId, zhiwu, state);
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x0005B394 File Offset: 0x00059594
		public KuaFuCmdData Comp_GetKuaFuFuBenData(int gameType, int cityID, long dataAge)
		{
			return CompService.Instance().GetKuaFuFuBenData(gameType, cityID, dataAge);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0005B3B4 File Offset: 0x000595B4
		public int CreateZhanDui(int serverID, TianTi5v5ZhanDuiData pData)
		{
			return TianTi5v5Service.CreateZhanDui(serverID, pData);
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0005B3D0 File Offset: 0x000595D0
		public int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			return TianTi5v5Service.UpdateZhanDuiXuanYan(teamID, xuanYan);
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x0005B3EC File Offset: 0x000595EC
		public int DeleteZhanDui(int serverID, int roleid, int teamID)
		{
			return TianTi5v5Service.DeleteZhanDui(serverID, roleid, teamID);
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0005B408 File Offset: 0x00059608
		public int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			return TianTi5v5Service.UpdateZhanDuiData(data, modeType);
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x0005B424 File Offset: 0x00059624
		public int ZhanDuiRoleSignUp(int serverId, int gameType, int teamID, long zhanLi, int groupIndex)
		{
			return TianTi5v5Service.ZhanDuiRoleSignUp(serverId, gameType, teamID, zhanLi, groupIndex);
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0005B444 File Offset: 0x00059644
		public int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID)
		{
			return TianTi5v5Service.ZhanDuiRoleChangeState(serverId, zhanDuiID, roleId, state, gameID);
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0005B464 File Offset: 0x00059664
		public KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			return TianTi5v5Service.ZhanDuiGetFuBenData(gameId);
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0005B47C File Offset: 0x0005967C
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

		// Token: 0x060006D8 RID: 1752 RVA: 0x0005B518 File Offset: 0x00059718
		public TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			return TianTi5v5Service.ZhanDuiGetRankingData(modifyTime);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0005B530 File Offset: 0x00059730
		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int gameid)
		{
			return Zork5v5Service.Instance().GetFuBenDataByGameId_ZorkBattle(gameid);
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x0005B550 File Offset: 0x00059750
		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			return Zork5v5Service.Instance().GetFuBenDataByZhanDuiId_ZorkBattle(ZhanDuiId);
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x0005B570 File Offset: 0x00059770
		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			return Zork5v5Service.Instance().GetKuaFuGameState_ZorkBattle(zhanduiID);
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0005B590 File Offset: 0x00059790
		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			return Zork5v5Service.Instance().SyncData_ZorkBattle(gsTicks, ageRank);
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x0005B5B0 File Offset: 0x000597B0
		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			return Zork5v5Service.Instance().SignUp_ZorkBattle(zhanduiID, serverID);
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x0005B5D0 File Offset: 0x000597D0
		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			return Zork5v5Service.Instance().GameFuBenComplete_ZorkBattle(data);
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0005B5F0 File Offset: 0x000597F0
		public int ExcuteGMCmd(int serverID, int rid, string[] cmd)
		{
			return 0;
		}

		// Token: 0x0400039E RID: 926
		private const double CheckGameFuBenInterval = 1000.0;

		// Token: 0x0400039F RID: 927
		private const double CheckRoleTimerProcInterval = 1.428;

		// Token: 0x040003A0 RID: 928
		private const double SaveServerStateProcInterval = 30.0;

		// Token: 0x040003A1 RID: 929
		public static TianTiService Instance = null;

		// Token: 0x040003A2 RID: 930
		private object Mutex = new object();

		// Token: 0x040003A3 RID: 931
		public readonly GameTypes GameType = GameTypes.TianTi;

		// Token: 0x040003A4 RID: 932
		private DateTime CheckGameFuBenTime;

		// Token: 0x040003A5 RID: 933
		private DateTime CheckRoleTimerProcTime;

		// Token: 0x040003A6 RID: 934
		private DateTime SaveServerStateProcTime;

		// Token: 0x040003A7 RID: 935
		public TianTiPersistence Persistence = TianTiPersistence.Instance;

		// Token: 0x040003A8 RID: 936
		public int ExistKuaFuFuBenCount = 0;

		// Token: 0x040003A9 RID: 937
		private int EnterGameSecs = 20;

		// Token: 0x040003AA RID: 938
		public int DisconnectionProtectSecs = 15;

		// Token: 0x040003AB RID: 939
		public int GameEndProtectSecs = 15;

		// Token: 0x040003AC RID: 940
		private int MaxServerLoad = 400;

		// Token: 0x040003AD RID: 941
		private int MaxTianTiDayCount = 1000;

		// Token: 0x040003AE RID: 942
		private int TianTiDayCount = 0;

		// Token: 0x040003AF RID: 943
		private int TianTiDayId = 0;

		// Token: 0x040003B0 RID: 944
		public int[] AssignRangeArray = new int[]
		{
			0,
			1,
			2,
			100
		};

		// Token: 0x040003B1 RID: 945
		public ConcurrentDictionary<int, TianTiFuBenData> TianTiFuBenDataDict = new ConcurrentDictionary<int, TianTiFuBenData>(1, 4096);

		// Token: 0x040003B2 RID: 946
		private SortedDictionary<RangeKey, TianTiFuBenData> ProcessTianTiFuBenDataDict = new SortedDictionary<RangeKey, TianTiFuBenData>(RangeKey.Comparer);

		// Token: 0x040003B3 RID: 947
		private SortedList<long, int> RolePairFightCountDict = new SortedList<long, int>();

		// Token: 0x040003B4 RID: 948
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		// Token: 0x040003B5 RID: 949
		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		// Token: 0x040003B6 RID: 950
		public Thread BackgroundThread;
	}
}
