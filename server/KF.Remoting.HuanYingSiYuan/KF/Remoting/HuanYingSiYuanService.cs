using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x02000081 RID: 129
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class HuanYingSiYuanService : MarshalByRefObject, IKuaFuService
	{
		// Token: 0x06000631 RID: 1585 RVA: 0x000552FC File Offset: 0x000534FC
		public override object InitializeLifetimeService()
		{
			HuanYingSiYuanService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00055348 File Offset: 0x00053548
		public HuanYingSiYuanService()
		{
			HuanYingSiYuanService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00055440 File Offset: 0x00053640
		~HuanYingSiYuanService()
		{
			this.BackgroundThread.Abort();
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00055478 File Offset: 0x00053678
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
					if (now > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = now.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						this.CheckRoleTimerProc(now, out signUpCount, out startCount);
						ClientAgentManager.Instance().SetGameTypeLoad(this.GameType, signUpCount, startCount);
					}
					if (now > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = now.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(now);
					}
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

		// Token: 0x06000635 RID: 1589 RVA: 0x000555B8 File Offset: 0x000537B8
		private void CheckRoleTimerProc(DateTime now, out int signUpCount, out int startCount)
		{
			signUpCount = 0;
			startCount = 0;
			bool assgionGameFuBen = true;
			long maxRemoveRoleTicks = now.AddHours(-2.0).Ticks;
			long waitTicks = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs1)).Ticks;
			long waitTicks2 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs2)).Ticks;
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
				}
				if (kuaFuRoleData.State == KuaFuRoleStates.SignUp)
				{
					signUpCount++;
					if (assgionGameFuBen)
					{
						assgionGameFuBen = this.AssignGameFuben(kuaFuRoleData, waitTicks, waitTicks2, now);
					}
				}
				else if (kuaFuRoleData.State == KuaFuRoleStates.SignUpWaiting)
				{
					signUpCount++;
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

		// Token: 0x06000636 RID: 1590 RVA: 0x0005584C File Offset: 0x00053A4C
		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.HuanYingSiYuanFuBenDataDict.Count > 0)
			{
				DateTime canRemoveTime = now.AddMinutes(-15.0);
				foreach (HuanYingSiYuanFuBenData fuBenData in this.HuanYingSiYuanFuBenDataDict.Values)
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

		// Token: 0x06000637 RID: 1591 RVA: 0x0005593C File Offset: 0x00053B3C
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00055960 File Offset: 0x00053B60
		public AsyncData GetClientCacheItems2(int serverId, long requestTicks)
		{
			return new AsyncData
			{
				RequestTicks = requestTicks,
				ServerTicks = TimeUtil.NOW(),
				ItemList = ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType)
			};
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x000559A4 File Offset: 0x00053BA4
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 1 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
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

		// Token: 0x0600063A RID: 1594 RVA: 0x00055AB8 File Offset: 0x00053CB8
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

		// Token: 0x0600063B RID: 1595 RVA: 0x00055C90 File Offset: 0x00053E90
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

		// Token: 0x0600063C RID: 1596 RVA: 0x00055D50 File Offset: 0x00053F50
		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state)
		{
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
			{
				lock (huanYingSiYuanFuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (huanYingSiYuanFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
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

		// Token: 0x0600063D RID: 1597 RVA: 0x00055E30 File Offset: 0x00054030
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

		// Token: 0x0600063E RID: 1598 RVA: 0x00055E74 File Offset: 0x00054074
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
					HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
					if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out huanYingSiYuanFuBenData))
					{
						if (huanYingSiYuanFuBenData.State < GameFuBenState.Start)
						{
							roleCount = huanYingSiYuanFuBenData.GetFuBenRoleCount();
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

		// Token: 0x0600063F RID: 1599 RVA: 0x00055F44 File Offset: 0x00054144
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00055F5C File Offset: 0x0005415C
		public HuanYingSiYuanFuBenData GetFuBenData(int gameId)
		{
			HuanYingSiYuanFuBenData kuaFuFuBenData = null;
			HuanYingSiYuanFuBenData result;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out kuaFuFuBenData) && kuaFuFuBenData.State < GameFuBenState.End)
			{
				result = kuaFuFuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00055F9C File Offset: 0x0005419C
		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11;
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			int result2;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
			{
				lock (huanYingSiYuanFuBenData)
				{
					huanYingSiYuanFuBenData.State = state;
					if (state == GameFuBenState.End)
					{
						this.RemoveGameFuBen(huanYingSiYuanFuBenData);
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

		// Token: 0x06000642 RID: 1602 RVA: 0x00056030 File Offset: 0x00054230
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

		// Token: 0x06000643 RID: 1603 RVA: 0x00056130 File Offset: 0x00054330
		private void NotifyFuBenRoleCount(HuanYingSiYuanFuBenData fuBenData)
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

		// Token: 0x06000644 RID: 1604 RVA: 0x00056250 File Offset: 0x00054450
		private void NotifyFuBenRoleEnterGame(HuanYingSiYuanFuBenData fuBenData)
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

		// Token: 0x06000645 RID: 1605 RVA: 0x0005635C File Offset: 0x0005455C
		private bool AssignGameFuben(KuaFuRoleData kuaFuRoleData, long waitSecs1, long waitSecs2, DateTime now)
		{
			int roleCount = 0;
			DateTime stateEndTime = now.AddSeconds((double)this.EnterGameSecs);
			HuanYingSiYuanFuBenData selectFuben = null;
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId,
				ZhanDouLi = kuaFuRoleData.GameData.ZhanDouLi
			};
			try
			{
				foreach (HuanYingSiYuanFuBenData tmpFuben in this.ShotOfRolesFuBenDataDict.Values)
				{
					if (tmpFuben.CanRemove())
					{
						this.RemoveGameFuBen(tmpFuben);
					}
					else if (tmpFuben.CanEnter(kuaFuRoleData.GroupIndex, waitSecs1, waitSecs2))
					{
						if (ClientAgentManager.Instance().IsAgentAlive(tmpFuben.ServerId))
						{
							roleCount = tmpFuben.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
							if (roleCount > 0)
							{
								selectFuben = tmpFuben;
								break;
							}
						}
					}
				}
				if (null == selectFuben)
				{
					int gameId = this.Persistence.GetNextGameId();
					int kfSrvId = 0;
					if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)gameId, 1, out kfSrvId))
					{
						selectFuben = new HuanYingSiYuanFuBenData();
						selectFuben.ServerId = kfSrvId;
						selectFuben.GameId = gameId;
						selectFuben.GroupIndex = kuaFuRoleData.GroupIndex;
						selectFuben.EndTime = Global.NowTime.AddMinutes(15.0);
						this.AddGameFuBen(selectFuben);
						roleCount = selectFuben.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
						this.Persistence.LogCreateHysyFuben(gameId, kfSrvId, 0, 1);
					}
				}
				if (selectFuben != null && roleCount > 0)
				{
					if (roleCount == 1)
					{
						selectFuben.EndTime = now;
					}
					if (selectFuben.State == GameFuBenState.Wait)
					{
						if (roleCount == Consts.HuanYingSiYuanRoleCountTotal)
						{
							List<KuaFuFuBenRoleData> roleList = selectFuben.SortFuBenRoleList();
							foreach (KuaFuFuBenRoleData role in roleList)
							{
								KuaFuRoleKey key = KuaFuRoleKey.Get(role.ServerId, role.RoleId);
								KuaFuRoleData kuaFuRoleDataTemp;
								if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleDataTemp))
								{
									kuaFuRoleDataTemp.UpdateStateTime(selectFuben.GameId, KuaFuRoleStates.NotifyEnterGame, stateEndTime.Ticks);
								}
							}
							selectFuben.State = GameFuBenState.Start;
							this.NotifyFuBenRoleEnterGame(selectFuben);
						}
						else
						{
							kuaFuRoleData.UpdateStateTime(selectFuben.GameId, KuaFuRoleStates.SignUpWaiting, kuaFuRoleData.StateEndTicks);
							this.NotifyFuBenRoleCount(selectFuben);
						}
					}
					else if (selectFuben.State == GameFuBenState.Start)
					{
						kuaFuRoleData.UpdateStateTime(selectFuben.GameId, KuaFuRoleStates.NotifyEnterGame, stateEndTime.Ticks);
						this.NotifyFuBenRoleEnterGame(selectFuben);
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000566EC File Offset: 0x000548EC
		private void GameFuBenRoleCountChangedHandler(HuanYingSiYuanFuBenData huanYingSiYuanFuBenData, int roleCount)
		{
			if (roleCount == Consts.HuanYingSiYuanRoleCountTotal)
			{
				HuanYingSiYuanFuBenData huanYingSiYuanFuBenDataTemp;
				this.ShotOfRolesFuBenDataDict.TryRemove(huanYingSiYuanFuBenData.GameId, out huanYingSiYuanFuBenDataTemp);
			}
			else if (!huanYingSiYuanFuBenData.CanRemove())
			{
				this.ShotOfRolesFuBenDataDict[huanYingSiYuanFuBenData.GameId] = huanYingSiYuanFuBenData;
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00056740 File Offset: 0x00054940
		private void AddGameFuBen(HuanYingSiYuanFuBenData huanYingSiYuanFuBenData)
		{
			this.HuanYingSiYuanFuBenDataDict[huanYingSiYuanFuBenData.GameId] = huanYingSiYuanFuBenData;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00056758 File Offset: 0x00054958
		private void RemoveGameFuBen(HuanYingSiYuanFuBenData fubenData)
		{
			int gameId = fubenData.GameId;
			HuanYingSiYuanFuBenData tmpFuben;
			this.ShotOfRolesFuBenDataDict.TryRemove(gameId, out tmpFuben);
			if (this.HuanYingSiYuanFuBenDataDict.TryRemove(gameId, out tmpFuben))
			{
				tmpFuben.State = GameFuBenState.End;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, tmpFuben.ServerId, (long)tmpFuben.GameId);
			lock (fubenData)
			{
				foreach (KuaFuFuBenRoleData fuBenRoleData in fubenData.RoleDict.Values)
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

		// Token: 0x06000649 RID: 1609 RVA: 0x00056884 File Offset: 0x00054A84
		private void RemoveRoleFromFuBen(int gameId, int roleId)
		{
			if (gameId > 0)
			{
				HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
				if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
				{
					lock (huanYingSiYuanFuBenData)
					{
						int count = huanYingSiYuanFuBenData.RemoveKuaFuFuBenRoleData(roleId, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
						if (huanYingSiYuanFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(huanYingSiYuanFuBenData);
						}
						else if (huanYingSiYuanFuBenData.State < GameFuBenState.Start)
						{
							this.NotifyFuBenRoleCount(huanYingSiYuanFuBenData);
						}
					}
				}
			}
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00056938 File Offset: 0x00054B38
		public int UseGiftCode(string ptid, string uid, string rid, string channel, string codeno, string appid, int zoneid, ref string giftid)
		{
			MySqlDataReader myTable = null;
			int result;
			try
			{
				string existCodeSql = string.Format("select count(codeno) from  t_uselog  where codeno='{0}'; ", codeno);
				long existTable = (long)DbHelperMySQL.GetSingle(existCodeSql);
				if (existTable != 0L)
				{
					result = -14;
				}
				else
				{
					string cSql = string.Format("select * from t_giftcode where codeno ='{0}'", codeno, appid);
					myTable = DbHelperMySQL.ExecuteReader(cSql, false);
					if (myTable == null || !myTable.HasRows || !myTable.Read())
					{
						result = -11;
					}
					else
					{
						MySqlDataReader dr = myTable;
						if (!string.IsNullOrEmpty(dr["usetime"].ToString()))
						{
							result = -14;
						}
						else
						{
							giftid = dr["giftid"].ToString();
							string codePtid = dr["ptid"].ToString();
							string codeChannel = dr["channel"].ToString();
							List<string> channelArray = new List<string>();
							string[] channelStrs = codeChannel.Split(new char[]
							{
								'|'
							}, StringSplitOptions.RemoveEmptyEntries);
							foreach (string channelStr in channelStrs)
							{
								channelArray.Add(channelStr.ToLower());
							}
							string codeZone = dr["zoneid"].ToString();
							DateTime startdate = Convert.ToDateTime(dr["startdate"]);
							DateTime enddate = Convert.ToDateTime(dr["enddate"].ToString());
							int times = Convert.ToInt32(dr["times"]);
							int character = Convert.ToInt32(dr["character"]);
							List<string> zoneArray = new List<string>();
							zoneArray.AddRange(codeZone.Split(new char[]
							{
								'|'
							}, StringSplitOptions.RemoveEmptyEntries));
							if (channel == "HMN")
							{
								channel = "HM";
							}
							if (codeChannel != "ALL" && !channelArray.Contains(channel.ToLower()) && !channelArray.Contains("ALL"))
							{
								result = -16;
							}
							else if (codeZone != "0" && !zoneArray.Contains(zoneid.ToString()))
							{
								result = -18;
							}
							else if (startdate > DateTime.Now.Date || enddate < DateTime.Now.Date)
							{
								result = -17;
							}
							else
							{
								string useCount = string.Format("select count(giftid) from t_uselog where giftid ='{0}'", giftid);
								if (character == 0)
								{
									useCount += string.Format(" and rid={0}", rid);
								}
								else
								{
									useCount += string.Format(" and userid='{0}'", uid);
								}
								int useCountNum = Convert.ToInt32(DbHelperMySQL.GetSingle(useCount));
								if (useCountNum >= times)
								{
									result = -12;
								}
								else
								{
									string sql = string.Format("update t_giftcode set usetime=now() where codeno ='{0}';", codeno);
									DbHelperMySQL.GetSingle(sql);
									sql = string.Format("insert into t_uselog(userid,rid,giftid,codeno,usetime,serverid,ptid,channel,status) values('{0}','{1}','{2}','{3}',now(),'{4}','{5}','{6}','{7}');", new object[]
									{
										uid,
										rid,
										giftid,
										codeno,
										zoneid,
										ptid,
										channel,
										1
									});
									DbHelperMySQL.GetSingle(sql);
									result = 0;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -11003;
			}
			finally
			{
				if (null != myTable)
				{
					myTable.Close();
				}
			}
			return result;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00056D0C File Offset: 0x00054F0C
		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			lock (this.Mutex)
			{
				if (serverFlag == 1)
				{
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(GameTypes.HuanYingSiYuan, new AsyncDataItem(KuaFuEventTypes.GMCmd, new object[]
					{
						data
					}));
				}
				else
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.HuanYingSiYuan, new AsyncDataItem(KuaFuEventTypes.GMCmd, new object[]
					{
						data
					}), 0);
				}
			}
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00056DA8 File Offset: 0x00054FA8
		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			return KuaFuLueDuoService.Instance().JingJia_KuaFuLueDuo(bhid, zoneid_bh, bhname, ziJin, serverId, oldZiJin);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00056DD0 File Offset: 0x00054FD0
		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByServerId_KuaFuLueDuo(serverId);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00056DF0 File Offset: 0x00054FF0
		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByGameId_KuaFuLueDuo(gameId);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00056E10 File Offset: 0x00055010
		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			return KuaFuLueDuoService.Instance().GetRoleData_KuaFuLueDuo(rid);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00056E30 File Offset: 0x00055030
		public byte[] SyncData_KuaFuLueDuo(byte[] bytes)
		{
			return DataHelper2.ObjectToBytes<KuaFuLueDuoSyncData>(KuaFuLueDuoService.Instance().SyncData_KuaFuLueDuo(bytes));
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00056E54 File Offset: 0x00055054
		public KuaFuCmdData GetBHDataByBhid_KuaFuLueDuo(int bhid, long age)
		{
			return KuaFuLueDuoService.Instance().GetBHDataByBhid_KuaFuLueDuo(bhid, age);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00056E74 File Offset: 0x00055074
		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			return KuaFuLueDuoService.Instance().GameFuBenComplete_KuaFuLueDuo(data);
		}

		// Token: 0x04000378 RID: 888
		private const double CheckGameFuBenInterval = 1000.0;

		// Token: 0x04000379 RID: 889
		private const double CheckRoleTimerProcInterval = 1.428;

		// Token: 0x0400037A RID: 890
		public static HuanYingSiYuanService Instance = null;

		// Token: 0x0400037B RID: 891
		private object Mutex = new object();

		// Token: 0x0400037C RID: 892
		public readonly GameTypes GameType = GameTypes.HuanYingSiYuan;

		// Token: 0x0400037D RID: 893
		private DateTime CheckGameFuBenTime;

		// Token: 0x0400037E RID: 894
		private DateTime CheckRoleTimerProcTime;

		// Token: 0x0400037F RID: 895
		public HuanYingSiYuanPersistence Persistence = HuanYingSiYuanPersistence.Instance;

		// Token: 0x04000380 RID: 896
		public int ExistKuaFuFuBenCount = 0;

		// Token: 0x04000381 RID: 897
		private int EnterGameSecs = 20;

		// Token: 0x04000382 RID: 898
		public int DisconnectionProtectSecs = 15;

		// Token: 0x04000383 RID: 899
		public int GameEndProtectSecs = 15;

		// Token: 0x04000384 RID: 900
		private int MaxServerLoad = 400;

		// Token: 0x04000385 RID: 901
		private int MaxHuanYingSiYuanDayCount = 1000;

		// Token: 0x04000386 RID: 902
		private int HuanYingSiYuanDayCount = 0;

		// Token: 0x04000387 RID: 903
		private int HuanYingSiYuanDayId = 0;

		// Token: 0x04000388 RID: 904
		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> HuanYingSiYuanFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		// Token: 0x04000389 RID: 905
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		// Token: 0x0400038A RID: 906
		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		// Token: 0x0400038B RID: 907
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> WaitingKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		// Token: 0x0400038C RID: 908
		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> ShotOfRolesFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		// Token: 0x0400038D RID: 909
		public Thread BackgroundThread;
	}
}
