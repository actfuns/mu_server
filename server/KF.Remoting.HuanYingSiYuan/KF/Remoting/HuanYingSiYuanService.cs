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
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class HuanYingSiYuanService : MarshalByRefObject, IKuaFuService
	{
		
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

		
		public HuanYingSiYuanService()
		{
			HuanYingSiYuanService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		
		~HuanYingSiYuanService()
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

		
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		
		public AsyncData GetClientCacheItems2(int serverId, long requestTicks)
		{
			return new AsyncData
			{
				RequestTicks = requestTicks,
				ServerTicks = TimeUtil.NOW(),
				ItemList = ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType)
			};
		}

		
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

		
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		
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

		
		private void AddGameFuBen(HuanYingSiYuanFuBenData huanYingSiYuanFuBenData)
		{
			this.HuanYingSiYuanFuBenDataDict[huanYingSiYuanFuBenData.GameId] = huanYingSiYuanFuBenData;
		}

		
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

		
		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			return KuaFuLueDuoService.Instance().JingJia_KuaFuLueDuo(bhid, zoneid_bh, bhname, ziJin, serverId, oldZiJin);
		}

		
		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByServerId_KuaFuLueDuo(serverId);
		}

		
		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByGameId_KuaFuLueDuo(gameId);
		}

		
		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			return KuaFuLueDuoService.Instance().GetRoleData_KuaFuLueDuo(rid);
		}

		
		public byte[] SyncData_KuaFuLueDuo(byte[] bytes)
		{
			return DataHelper2.ObjectToBytes<KuaFuLueDuoSyncData>(KuaFuLueDuoService.Instance().SyncData_KuaFuLueDuo(bytes));
		}

		
		public KuaFuCmdData GetBHDataByBhid_KuaFuLueDuo(int bhid, long age)
		{
			return KuaFuLueDuoService.Instance().GetBHDataByBhid_KuaFuLueDuo(bhid, age);
		}

		
		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			return KuaFuLueDuoService.Instance().GameFuBenComplete_KuaFuLueDuo(data);
		}

		
		private const double CheckGameFuBenInterval = 1000.0;

		
		private const double CheckRoleTimerProcInterval = 1.428;

		
		public static HuanYingSiYuanService Instance = null;

		
		private object Mutex = new object();

		
		public readonly GameTypes GameType = GameTypes.HuanYingSiYuan;

		
		private DateTime CheckGameFuBenTime;

		
		private DateTime CheckRoleTimerProcTime;

		
		public HuanYingSiYuanPersistence Persistence = HuanYingSiYuanPersistence.Instance;

		
		public int ExistKuaFuFuBenCount = 0;

		
		private int EnterGameSecs = 20;

		
		public int DisconnectionProtectSecs = 15;

		
		public int GameEndProtectSecs = 15;

		
		private int MaxServerLoad = 400;

		
		private int MaxHuanYingSiYuanDayCount = 1000;

		
		private int HuanYingSiYuanDayCount = 0;

		
		private int HuanYingSiYuanDayId = 0;

		
		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> HuanYingSiYuanFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		
		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		
		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> WaitingKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		
		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> ShotOfRolesFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		
		public Thread BackgroundThread;
	}
}
