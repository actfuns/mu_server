using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	// Token: 0x0200041B RID: 1051
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class TianTiClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x060012B1 RID: 4785 RVA: 0x00128E48 File Offset: 0x00127048
		public static TianTiClient getInstance()
		{
			return TianTiClient.instance;
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x00128E60 File Offset: 0x00127060
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00128E74 File Offset: 0x00127074
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 2;
			return true;
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x00128EBC File Offset: 0x001270BC
		public bool startup()
		{
			return true;
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x00128ED0 File Offset: 0x001270D0
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x00128EE4 File Offset: 0x001270E4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x00128EF8 File Offset: 0x001270F8
		public void ExecuteEventCallBackAsync(object state)
		{
			AsyncDataItem[] items = state as AsyncDataItem[];
			if (items != null && items.Length > 0)
			{
				foreach (AsyncDataItem item in items)
				{
					this.EventCallBackHandler(item);
				}
			}
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x00128F4C File Offset: 0x0012714C
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				if (this.NextClearFuBenTime < now)
				{
					this.NextClearFuBenTime = now.AddHours(1.0);
					this.ClearOverTimeFuBen(now);
				}
				string tianTiUri = this.CoreInterface.GetRuntimeVariable("TianTiUri", null);
				if (this.RemoteServiceUri != tianTiUri)
				{
					this.RemoteServiceUri = tianTiUri;
				}
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] items = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId);
						if (items != null && items.Length > 0)
						{
							this.ExecuteEventCallBackAsync(items);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x00129048 File Offset: 0x00127248
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("TianTiUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
				this.RomoteServiceConnect = false;
			}
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x001290C0 File Offset: 0x001272C0
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x001290CA File Offset: 0x001272CA
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x001290D4 File Offset: 0x001272D4
		public bool IsKuaFuServerOK()
		{
			return this.RomoteServiceConnect;
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x001290EC File Offset: 0x001272EC
		private ITianTiService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this.Mutex)
				{
					if (string.IsNullOrEmpty(this.RemoteServiceUri))
					{
						return null;
					}
					if (this.KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this.RemotingMutex)
				{
					ITianTiService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (ITianTiService)Activator.GetObject(typeof(ITianTiService), this.RemoteServiceUri);
						if (null == kuaFuService)
						{
							return null;
						}
					}
					else
					{
						kuaFuService = this.KuaFuService;
					}
					int clientId = this.ClientInfo.ClientId;
					long nowTicks = TimeUtil.NOW();
					if (clientId <= 0 || Math.Abs(nowTicks - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = nowTicks;
						clientId = kuaFuService.InitializeClient(this.ClientInfo);
					}
					if (!this.RomoteServiceConnect)
					{
						this.RomoteServiceConnect = true;
						LogManager.WriteLog(LogTypes.Fatal, "KuaFu5v5.InitializeClient Connected", null, false);
					}
					if (kuaFuService != null && (clientId != this.ClientInfo.ClientId || this.KuaFuService != kuaFuService))
					{
						lock (this.Mutex)
						{
							if (clientId > 0)
							{
								this.KuaFuService = kuaFuService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = clientId;
							return kuaFuService;
						}
					}
					return this.KuaFuService;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0012938C File Offset: 0x0012758C
		public int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0)
		{
			int result = 0;
			int result2;
			if (kuaFuRoleData == null)
			{
				result2 = result;
			}
			else
			{
				roleId = kuaFuRoleData.RoleId;
				lock (this.Mutex)
				{
					if (kuaFuRoleData.State == KuaFuRoleStates.None)
					{
						this.RemoveRoleData(kuaFuRoleData.RoleId);
						return 0;
					}
					this.RoleId2RoleDataDict[roleId] = kuaFuRoleData;
					this.RoleId2KuaFuStateDict[roleId] = (int)kuaFuRoleData.State;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x00129438 File Offset: 0x00127638
		public int RoleChangeState(int serverId, int rid, int state)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RoleChangeState(serverId, rid, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00129490 File Offset: 0x00127690
		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenChangeState(gameId, state, time);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x001294F0 File Offset: 0x001276F0
		public int GetNewFuBenSeqId()
		{
			int result;
			if (null != this.CoreInterface)
			{
				result = this.CoreInterface.GetNewFuBenSeqId();
			}
			else
			{
				result = -11;
			}
			return result;
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x00129520 File Offset: 0x00127720
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x00129534 File Offset: 0x00127734
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				int num = eventType;
				if (num <= 38)
				{
					switch (num)
					{
					case 0:
					case 1:
						if (args.Length == 1)
						{
							KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
							if (null != kuaFuRoleData)
							{
								this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
							}
						}
						break;
					case 2:
						if (args.Length == 2)
						{
							int rid = (int)args[0];
							int count = (int)args[1];
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuFuBenRoleCountEvent(rid, count), this.SceneType);
						}
						break;
					case 3:
						if (args.Length == 1)
						{
							KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
							if (null != kuaFuRoleData)
							{
								this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
								TianTiFuBenData TianTiFuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
								if (TianTiFuBenData != null && TianTiFuBenData.State == GameFuBenState.Start)
								{
									KuaFuServerLoginData kuaFuServerLoginData = new KuaFuServerLoginData
									{
										RoleId = kuaFuRoleData.RoleId,
										GameType = kuaFuRoleData.GameType,
										GameId = (long)kuaFuRoleData.GameId,
										EndTicks = kuaFuRoleData.StateEndTicks
									};
									kuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
									KuaFuServerInfo kuaFuServerInfo;
									if (KuaFuManager.getInstance().TryGetValue(TianTiFuBenData.ServerId, out kuaFuServerInfo))
									{
										kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
										kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
									}
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), this.SceneType);
								}
							}
						}
						break;
					case 4:
						if (args.Length == 1)
						{
							int gameId = (int)args[0];
							TianTiManager.getInstance().CancleTianTiScene(gameId);
						}
						break;
					default:
						if (num != 20)
						{
							switch (num)
							{
							case 34:
								CompManager.getInstance().OnNoticeListData(args[0] as byte[]);
								break;
							case 35:
								CompManager.getInstance().OnChatListData(args[0] as byte[]);
								break;
							case 36:
								CompManager.getInstance().OnRefreshAllCompNpc((int)args[0]);
								break;
							case 37:
								CompBattleManager.getInstance().OnCompBattleReset();
								break;
							case 38:
								CompMineManager.getInstance().OnCompMineReset();
								break;
							}
						}
						else
						{
							ZhengDuoSyncData zhengDuoSyncData = args[0] as ZhengDuoSyncData;
							if (null != zhengDuoSyncData)
							{
								this._ZhengDuoSyncData = zhengDuoSyncData;
							}
						}
						break;
					}
				}
				else if (num <= 10028)
				{
					switch (num)
					{
					case 10009:
					{
						ZhengBaSupportLogData data = args[0] as ZhengBaSupportLogData;
						if (data != null && data.FromServerId != this.ClientInfo.ServerId)
						{
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaSupportEvent(data), 36);
						}
						break;
					}
					case 10010:
						if (args.Length == 1)
						{
							ZhengBaPkLogData log = args[0] as ZhengBaPkLogData;
							if (log != null)
							{
								this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaPkLogEvent(log), 36);
							}
						}
						break;
					case 10011:
					{
						ZhengBaNtfEnterData data2 = args[0] as ZhengBaNtfEnterData;
						KuaFuServerInfo kuaFuServerInfo;
						if (KuaFuManager.getInstance().TryGetValue(data2.ToServerId, out kuaFuServerInfo))
						{
							data2.ToServerIp = kuaFuServerInfo.Ip;
							data2.ToServerPort = kuaFuServerInfo.Port;
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("KuaFuEventTypes.ZhengBaNtfEnter not find kfserver={0}", data2.ToServerId), null, true);
						}
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaNtfEnterEvent(data2), 36);
						break;
					}
					case 10012:
					{
						ZhengBaMirrorFightData data3 = args[0] as ZhengBaMirrorFightData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaMirrorFightEvent(data3), 36);
						break;
					}
					case 10013:
					{
						ZhengBaBulletinJoinData data4 = args[0] as ZhengBaBulletinJoinData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaBulletinJoinEvent(data4), 36);
						break;
					}
					case 10014:
						this.CoreInterface.GetEventSourceInterface().fireEvent(new CoupleArenaCanEnterEvent(args[0] as CoupleArenaCanEnterData), 38);
						break;
					default:
						if (num == 10028)
						{
							BHMatchNtfEnterData data5 = args[0] as BHMatchNtfEnterData;
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KFBHMatchNtfEnterData(data5), 45);
						}
						break;
					}
				}
				else if (num != 10033)
				{
					if (num == 10036)
					{
						KuaFu5v5FuBenData data6 = args[0] as KuaFu5v5FuBenData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZorkBattleNtfEnterData(data6), 57);
					}
				}
				else if (args.Length >= 1)
				{
					KuaFu5v5FuBenData kuaFu5v5FuBenData = args[0] as KuaFu5v5FuBenData;
					if (kuaFu5v5FuBenData != null && kuaFu5v5FuBenData.State == GameFuBenState.Start)
					{
						foreach (KuaFuFuBenRoleData kuaFuRoleData2 in kuaFu5v5FuBenData.RoleDict.Values)
						{
							GameClient client = GameManager.ClientMgr.FindClient(kuaFuRoleData2.RoleId);
							if (null != client)
							{
								KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
								if (null != clientKuaFuServerLoginData)
								{
									clientKuaFuServerLoginData.RoleId = kuaFuRoleData2.RoleId;
									clientKuaFuServerLoginData.GameId = (long)kuaFu5v5FuBenData.GameId;
									clientKuaFuServerLoginData.GameType = kuaFu5v5FuBenData.GameType;
									clientKuaFuServerLoginData.EndTicks = kuaFu5v5FuBenData.EndTime.Ticks;
									clientKuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
									clientKuaFuServerLoginData.ServerIp = kuaFu5v5FuBenData.LoginInfo.KuaFuIP;
									clientKuaFuServerLoginData.ServerPort = kuaFu5v5FuBenData.LoginInfo.KuaFuPort;
									clientKuaFuServerLoginData.ips = kuaFu5v5FuBenData.LoginInfo.LocalIPs;
									clientKuaFuServerLoginData.ports = kuaFu5v5FuBenData.LoginInfo.LocalPorts;
									KuaFuManager.getInstance().KuaFuSwitchServer(client);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x00129C1C File Offset: 0x00127E1C
		public int OnRoleChangeState(int roleId, int state, int age)
		{
			lock (this.Mutex)
			{
				KuaFuRoleData kuaFuRoleData;
				if (!this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					return -1;
				}
				if (age > kuaFuRoleData.Age)
				{
					kuaFuRoleData.State = (KuaFuRoleStates)state;
				}
			}
			return 0;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x00129CA0 File Offset: 0x00127EA0
		public int TianTiSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int result2;
			if (string.IsNullOrEmpty(userId) || roleId <= 0)
			{
				result2 = -20;
			}
			else
			{
				userId = userId.ToUpper();
				int count = Interlocked.Increment(ref this.CurrentRequestCount);
				try
				{
					if (count < this.MaxRequestCount)
					{
						lock (this.Mutex)
						{
							KuaFuRoleData kuaFuRoleData;
							if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
							{
								if (kuaFuRoleData.ServerId != this.ClientInfo.ServerId)
								{
									return -11;
								}
							}
						}
						ITianTiService kuaFuService = this.GetKuaFuService(false);
						if (null == kuaFuService)
						{
							return -11001;
						}
						try
						{
							IGameData TianTiGameData = new IGameData
							{
								ZhanDouLi = zhanDouLi
							};
							int result = kuaFuService.RoleSignUp(this.ClientInfo.ServerId, userId, zoneId, roleId, gameType, groupIndex, TianTiGameData);
						}
						catch (Exception ex)
						{
							this.ResetKuaFuService();
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				finally
				{
					Interlocked.Decrement(ref this.CurrentRequestCount);
				}
				result2 = 1;
			}
			return result2;
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00129E2C File Offset: 0x0012802C
		public int ChangeRoleState(int roleId, KuaFuRoleStates state, bool noWait = false)
		{
			int result = -11;
			KuaFuRoleData kuaFuRoleData = null;
			int serverId = this.ClientInfo.ServerId;
			lock (this.Mutex)
			{
				if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					serverId = kuaFuRoleData.ServerId;
				}
			}
			ITianTiService kuaFuService = this.GetKuaFuService(noWait);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RoleChangeState(serverId, roleId, (int)state);
					if (result >= 0)
					{
						lock (this.Mutex)
						{
							if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
							{
								kuaFuRoleData.State = (KuaFuRoleStates)result;
							}
						}
						if (null != kuaFuRoleData)
						{
							this.UpdateRoleData(kuaFuRoleData, 0);
						}
					}
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x00129F6C File Offset: 0x0012816C
		private TianTiFuBenData GetKuaFuFuBenData(int gameId)
		{
			TianTiFuBenData TianTiFuBenData = null;
			if (TianTiFuBenData == null)
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						TianTiFuBenData = kuaFuService.GetFuBenData(gameId);
					}
					catch (Exception ex)
					{
						TianTiFuBenData = null;
					}
				}
			}
			return TianTiFuBenData;
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x00129FC8 File Offset: 0x001281C8
		public int GetRoleKuaFuFuBenRoleCount(int roleId)
		{
			int roleCount = 0;
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					object result = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 0);
					if (null != result)
					{
						roleCount = (int)result;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return roleCount;
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x0012A048 File Offset: 0x00128248
		public int GameFuBenRoleChangeState(int roleId, int state, int serverId = 0, int gameId = 0)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (serverId <= 0 || gameId <= 0)
					{
						KuaFuRoleData kuaFuRoleData;
						if (!this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
						{
							return 0;
						}
						serverId = kuaFuRoleData.ServerId;
						gameId = kuaFuRoleData.GameId;
					}
					return this.KuaFuService.GameFuBenRoleChangeState(serverId, roleId, gameId, state);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x0012A0E8 File Offset: 0x001282E8
		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0012A148 File Offset: 0x00128348
		public KuaFuRoleData GetKuaFuRoleDataFromServer(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuRoleData = kuaFuService.GetKuaFuRoleData(serverId, roleId);
					this.UpdateRoleData(kuaFuRoleData, 0);
				}
				catch (Exception ex)
				{
					kuaFuRoleData = null;
				}
			}
			return kuaFuRoleData;
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x0012A1A0 File Offset: 0x001283A0
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			TianTiFuBenData TianTiFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
			if (TianTiFuBenData != null && TianTiFuBenData.State < GameFuBenState.End)
			{
				if (TianTiFuBenData.ServerId == GameManager.ServerId)
				{
					if (TianTiFuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId))
					{
						KuaFuRoleData kuaFuRoleData = this.GetKuaFuRoleDataFromServer(kuaFuServerLoginData.ServerId, kuaFuServerLoginData.RoleId);
						if (kuaFuRoleData.GameId == TianTiFuBenData.GameId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x0012A238 File Offset: 0x00128438
		public int GetRoleBattleWhichSide(int gameId, int roleId)
		{
			TianTiFuBenData TianTiFuBenData = this.GetKuaFuFuBenData(gameId);
			if (TianTiFuBenData != null && TianTiFuBenData.State < GameFuBenState.End)
			{
				if (TianTiFuBenData.ServerId == this.ClientInfo.ServerId)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (TianTiFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
					{
						return kuaFuFuBenRoleData.Side;
					}
				}
			}
			return 0;
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0012A2A8 File Offset: 0x001284A8
		public TianTiRankData GetRankingData()
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					DateTime modifyTime;
					lock (this.Mutex)
					{
						modifyTime = this.RankData.ModifyTime;
					}
					TianTiRankData tianTiRankData = kuaFuService.GetRankingData(modifyTime);
					lock (this.Mutex)
					{
						if (tianTiRankData != null && tianTiRankData.ModifyTime > this.RankData.ModifyTime)
						{
							this.RankData = tianTiRankData;
						}
						tianTiRankData = new TianTiRankData();
						tianTiRankData.ModifyTime = this.RankData.ModifyTime;
						tianTiRankData.MaxPaiMingRank = this.RankData.MaxPaiMingRank;
						if (this.RankData.TianTiRoleInfoDataList != null && this.RankData.TianTiRoleInfoDataList.Count > 0)
						{
							tianTiRankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiRoleInfoDataList);
						}
						if (this.RankData.TianTiMonthRoleInfoDataList != null && this.RankData.TianTiMonthRoleInfoDataList.Count > 0)
						{
							tianTiRankData.TianTiMonthRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiMonthRoleInfoDataList);
						}
						return tianTiRankData;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x0012A480 File Offset: 0x00128680
		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateRoleInfoData(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0012A4CC File Offset: 0x001286CC
		public ZhengBaSyncData GetZhengBaRankData(ZhengBaSyncData lastSyncData)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncZhengBaData(lastSyncData);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x0012A51C File Offset: 0x0012871C
		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaSupport(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x0012A570 File Offset: 0x00128770
		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaRequestEnter(roleId, gameId, enter);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0012A5C4 File Offset: 0x001287C4
		public int ZhengBaKuaFuLogin(int roleId, int gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaKuaFuLogin(roleId, gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x0012A618 File Offset: 0x00128818
		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner, int FirstLeaveRoleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaPkResult(gameId, winner, FirstLeaveRoleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0012A668 File Offset: 0x00128868
		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaJoin(roleId1, roleId2, serverId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0012A6C0 File Offset: 0x001288C0
		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaQuit(roleId1, roleId2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0012A718 File Offset: 0x00128918
		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaSync(lastSyncTime);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0012A768 File Offset: 0x00128968
		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaPreDivorce(roleId1, roleId2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0012A7C0 File Offset: 0x001289C0
		public CoupleArenaFuBenData GetFuBenData(long gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetCoupleFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0012A810 File Offset: 0x00128A10
		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaPkResult(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0012A860 File Offset: 0x00128A60
		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishWishRole(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0012A8B8 File Offset: 0x00128AB8
		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishGetWishRecord(roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0012A908 File Offset: 0x00128B08
		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishSyncCenterData(oldThisWeek, oldLastWeek, oldStatue);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0012A958 File Offset: 0x00128B58
		public int CoupleWishPreDivorce(int man, int wife)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishPreDivorce(man, wife);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0012A9B0 File Offset: 0x00128BB0
		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.CoupleWishReportCoupleStatue(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0012AA00 File Offset: 0x00128C00
		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishAdmire(fromRole, fromZone, admireType, toCoupleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0012AA5C File Offset: 0x00128C5C
		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishJoinParty(fromRole, fromZone, toCoupleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0012AAB4 File Offset: 0x00128CB4
		public ZhengDuoSyncData ZhengDuoSync(long age)
		{
			if (Math.Abs(TimeUtil.NOW() - age) > 20000L)
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						ZhengDuoSyncData data = kuaFuService.ZhengDuoSync(this.ClientInfo.ServerId, age);
						if (null != data)
						{
							this._ZhengDuoSyncData = data;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			return this._ZhengDuoSyncData;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0012AB48 File Offset: 0x00128D48
		public int ZhengDuoSign(int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengDuoSign(this.ClientInfo.ServerId, bhid, usedTime, zoneId, bhName, bhLevel, bhZhanLi);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0012ABB0 File Offset: 0x00128DB0
		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengDuoResult(bhidSuccess, bhids);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0012AC04 File Offset: 0x00128E04
		public int GmCommand(string[] args, byte[] data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GmCommand(args, data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0012AC58 File Offset: 0x00128E58
		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetZhengDuoFuBenDataByBhid(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0012ACA8 File Offset: 0x00128EA8
		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetZhengDuoFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0012ACF8 File Offset: 0x00128EF8
		public BHMatchBHData GetBHDataByBhid_BHMatch(int type, int bhid)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<BHMatchBHData> bhdata = null;
						if (type == 1)
						{
							if (!this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out bhdata))
							{
								bhdata = new KuaFuData<BHMatchBHData>();
								this.BHMatchBHDataDict_Gold[bhid] = bhdata;
							}
						}
						else if (!this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out bhdata))
						{
							bhdata = new KuaFuData<BHMatchBHData>();
							this.BHMatchBHDataDict_Rookie[bhid] = bhdata;
						}
						KuaFuCmdData result = kuaFuService.GetBHDataByBhid_BHMatch(type, bhid, bhdata.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > bhdata.Age)
						{
							bhdata.Age = result.Age;
							if (null != result.Bytes0)
							{
								bhdata.V = DataHelper2.BytesToObject<BHMatchBHData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null != bhdata.V)
							{
								if (type == 1)
								{
									this.BHMatchBHDataDict_Gold[bhid] = bhdata;
								}
								else
								{
									this.BHMatchBHDataDict_Rookie[bhid] = bhdata;
								}
							}
						}
						return bhdata.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0012AF04 File Offset: 0x00129104
		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncData_BHMatch(ageRank, agePKInfo, ageChampion);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0012AF54 File Offset: 0x00129154
		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuGameState_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0012AFA4 File Offset: 0x001291A4
		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CheckRookieJoinLast_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x0012AFF4 File Offset: 0x001291F4
		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RookieSignUp_BHMatch(bhid, zoneid_bh, bhname, rid, rname, zoneid_r);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0012B058 File Offset: 0x00129258
		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByBhid_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0012B0A8 File Offset: 0x001292A8
		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int GameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_BHMatch(GameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0012B0F8 File Offset: 0x001292F8
		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenComplete_BHMatch(data);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0012B14C File Offset: 0x0012934C
		public int RemoveBangHui_BHMatch(int bhid)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RemoveBangHui_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0012B1B0 File Offset: 0x001293B0
		public void SwitchLastGoldBH_GM()
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				kuaFuService.SwitchLastGoldBH_GM();
			}
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0012B1DC File Offset: 0x001293DC
		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.Comp_SyncData(ageComp, ageRankJX, ageRankJXL, ageRankBD, ageRankBJF, ageRankMJF);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0012B234 File Offset: 0x00129434
		public KFCompRoleData Comp_GetCompRoleData(int roleId)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFCompRoleData> CompRoleData = null;
						if (!this.CompRoleDataDict.TryGetValue(roleId, out CompRoleData))
						{
							CompRoleData = new KuaFuData<KFCompRoleData>();
							this.CompRoleDataDict[roleId] = CompRoleData;
						}
						KuaFuCmdData result = kuaFuService.Comp_GetCompRoleData(roleId, CompRoleData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > CompRoleData.Age)
						{
							CompRoleData.Age = result.Age;
							if (null != result.Bytes0)
							{
								CompRoleData.V = DataHelper2.BytesToObject<KFCompRoleData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null != CompRoleData.V)
							{
								this.CompRoleDataDict[roleId] = CompRoleData;
							}
						}
						return CompRoleData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0012B3E0 File Offset: 0x001295E0
		public int Comp_ChangeName(int roleId, string roleName)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_ChangeName(roleId, roleName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0012B434 File Offset: 0x00129634
		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_JoinComp_Repair(roleId, zoneId, roleName, compType, battleJiFen);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0012B48C File Offset: 0x0012968C
		public int Comp_JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_JoinComp(roleId, zoneId, roleName, compType);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0012B4E4 File Offset: 0x001296E4
		public int Comp_CompOpt(int compType, int optType, int param1, int param2)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_CompOpt(compType, optType, param1, param2);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0012B53C File Offset: 0x0012973C
		public int Comp_SetBulletin(int compType, string bulletin)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_SetBulletin(compType, bulletin);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0012B590 File Offset: 0x00129790
		public int Comp_BroadCastCompNotice(List<KFCompNotice> noticeList)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] sendBytes = DataHelper.ObjectToBytes<List<KFCompNotice>>(noticeList);
					kuaFuService.Comp_BroadCastCompNotice(this.ClientInfo.ServerId, sendBytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0012B5F8 File Offset: 0x001297F8
		public int Comp_CompChat(List<KFCompChat> chatList)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] sendBytes = DataHelper.ObjectToBytes<List<KFCompChat>>(chatList);
					kuaFuService.Comp_CompChat(this.ClientInfo.ServerId, sendBytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0012B660 File Offset: 0x00129860
		public int Comp_SetRoleData4Selector(int roleId, byte[] bytes)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_SetRoleData4Selector(roleId, bytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x0012B6B4 File Offset: 0x001298B4
		public int Comp_UpdateMapRoleNum(int mapCode, int roleNum)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateMapRoleNum(mapCode, roleNum);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x0012B708 File Offset: 0x00129908
		public void Comp_UpdateKuaFuMapClientCount(int gameType, CompFuBenData fubenItem)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateFuBenMapRoleNum(gameType, fubenItem);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x0012B754 File Offset: 0x00129954
		public void Comp_UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateStrongholdData(cityID, shDataList);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0012B7A0 File Offset: 0x001299A0
		public int Comp_GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_GameFuBenRoleChangeState(gameType, serverId, cityID, roleId, zhiwu, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0012B7FC File Offset: 0x001299FC
		public CompFuBenData Comp_GetKuaFuFuBenData(int gameType, int cityId)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(gameType, cityId);
						KuaFuData<CompFuBenData> KFuBenData = null;
						if (!this.CompFuBenDataDict.TryGetValue(kvpKey, out KFuBenData))
						{
							KFuBenData = new KuaFuData<CompFuBenData>();
							this.CompFuBenDataDict[kvpKey] = KFuBenData;
						}
						KuaFuCmdData result = kuaFuService.Comp_GetKuaFuFuBenData(gameType, cityId, KFuBenData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KFuBenData.Age)
						{
							KFuBenData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KFuBenData.V = DataHelper2.BytesToObject<CompFuBenData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KFuBenData.V)
							{
								KFuBenData.V = new CompFuBenData();
							}
						}
						return KFuBenData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0012B9B8 File Offset: 0x00129BB8
		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
				foreach (KeyValuePair<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> kv in this.CompFuBenDataDict)
				{
					if (kv.Value.V.EndTime < now)
					{
						list.Add(kv.Key);
					}
				}
				foreach (KeyValuePair<int, int> key in list)
				{
					this.CompFuBenDataDict.Remove(key);
				}
			}
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0012BACC File Offset: 0x00129CCC
		public int ChangeRoleSex(int serverID, int rid, int newSex, string data)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0012BB20 File Offset: 0x00129D20
		public int ChangeRoleOcc(int serverID, int rid, int newOcc, string data)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0012BB74 File Offset: 0x00129D74
		public int CreateZhanDui(TianTi5v5ZhanDuiData pData)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.CreateZhanDui(this.ClientInfo.ServerId, pData);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0012BBDC File Offset: 0x00129DDC
		public int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateZhanDuiXuanYan(teamID, xuanYan);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0012BC3C File Offset: 0x00129E3C
		public int ExcuteGMCmd(int serverID, int rid, string[] cmd)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.ExcuteGMCmd(serverID, rid, cmd);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0012BC9C File Offset: 0x00129E9C
		public int DeleteZhanDui(int serverID, int roleid, int teamID)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.DeleteZhanDui(serverID, roleid, teamID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0012BCFC File Offset: 0x00129EFC
		public int ZhanDuiRoleSignUp(int serverId, int gameType, int teamID, long zhanLi, int groupIndex)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiRoleSignUp(serverId, gameType, teamID, zhanLi, groupIndex);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x0012BD54 File Offset: 0x00129F54
		public int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID = 0)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiRoleChangeState(serverId, zhanDuiID, roleId, state, gameID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0012BDAC File Offset: 0x00129FAC
		public KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiGetFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0012BDFC File Offset: 0x00129FFC
		public TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiGetRankingData(modifyTime);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0012BE4C File Offset: 0x0012A04C
		public int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateZhanDuiData(data, modeType);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0012BEA0 File Offset: 0x0012A0A0
		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncData_ZorkBattle(gsTicks, ageRank);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0012BEF0 File Offset: 0x0012A0F0
		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuGameState_ZorkBattle(zhanduiID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0012BF40 File Offset: 0x0012A140
		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.SignUp_ZorkBattle(zhanduiID, serverID);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0012BF9C File Offset: 0x0012A19C
		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenComplete_ZorkBattle(data);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0012BFF0 File Offset: 0x0012A1F0
		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int GameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_ZorkBattle(GameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0012C040 File Offset: 0x0012A240
		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByZhanDuiId_ZorkBattle(ZhanDuiId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x04001C11 RID: 7185
		private static TianTiClient instance = new TianTiClient();

		// Token: 0x04001C12 RID: 7186
		private object Mutex = new object();

		// Token: 0x04001C13 RID: 7187
		private object RemotingMutex = new object();

		// Token: 0x04001C14 RID: 7188
		private ICoreInterface CoreInterface = null;

		// Token: 0x04001C15 RID: 7189
		private ITianTiService KuaFuService = null;

		// Token: 0x04001C16 RID: 7190
		private bool ClientInitialized = false;

		// Token: 0x04001C17 RID: 7191
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x04001C18 RID: 7192
		public int SceneType = 26;

		// Token: 0x04001C19 RID: 7193
		private int CurrentRequestCount = 0;

		// Token: 0x04001C1A RID: 7194
		private int MaxRequestCount = 50;

		// Token: 0x04001C1B RID: 7195
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		// Token: 0x04001C1C RID: 7196
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		// Token: 0x04001C1D RID: 7197
		private TianTiRankData RankData = new TianTiRankData();

		// Token: 0x04001C1E RID: 7198
		private string RemoteServiceUri = null;

		// Token: 0x04001C1F RID: 7199
		private DateTime NextClearFuBenTime;

		// Token: 0x04001C20 RID: 7200
		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		// Token: 0x04001C21 RID: 7201
		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		// Token: 0x04001C22 RID: 7202
		public Dictionary<int, KuaFuData<BHMatchRoleData>> BHMatchRoleDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchRoleData>>();

		// Token: 0x04001C23 RID: 7203
		public Dictionary<int, KuaFuData<BHMatchRoleData>> BHMatchRoleDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchRoleData>>();

		// Token: 0x04001C24 RID: 7204
		public Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict = new Dictionary<int, KuaFuData<KFCompRoleData>>();

		// Token: 0x04001C25 RID: 7205
		public Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> CompFuBenDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>>();

		// Token: 0x04001C26 RID: 7206
		private bool RomoteServiceConnect = true;

		// Token: 0x04001C27 RID: 7207
		private DuplexChannelFactory<ITianTiService> channelFactory;

		// Token: 0x04001C28 RID: 7208
		private ZhengDuoSyncData _ZhengDuoSyncData;
	}
}
