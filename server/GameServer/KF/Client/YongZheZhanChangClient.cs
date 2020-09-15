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

namespace KF.Client
{
	// Token: 0x02000810 RID: 2064
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class YongZheZhanChangClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06003A5A RID: 14938 RVA: 0x003183A8 File Offset: 0x003165A8
		public static YongZheZhanChangClient getInstance()
		{
			return YongZheZhanChangClient.instance;
		}

		// Token: 0x06003A5B RID: 14939 RVA: 0x003183C0 File Offset: 0x003165C0
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06003A5C RID: 14940 RVA: 0x003183D4 File Offset: 0x003165D4
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 5;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		// Token: 0x06003A5D RID: 14941 RVA: 0x00318430 File Offset: 0x00316630
		public bool startup()
		{
			return true;
		}

		// Token: 0x06003A5E RID: 14942 RVA: 0x00318444 File Offset: 0x00316644
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06003A5F RID: 14943 RVA: 0x00318458 File Offset: 0x00316658
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06003A60 RID: 14944 RVA: 0x0031846C File Offset: 0x0031666C
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

		// Token: 0x06003A61 RID: 14945 RVA: 0x003184C0 File Offset: 0x003166C0
		public void UpdateKuaFuMapClientCount(Dictionary<int, int> dict)
		{
			lock (this.Mutex)
			{
				this.ClientInfo.MapClientCountDict = dict;
			}
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateKuaFuMapClientCount(this.ClientInfo.ServerId, dict);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x06003A62 RID: 14946 RVA: 0x00318558 File Offset: 0x00316758
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
				string YongZheZhanChangUri = this.CoreInterface.GetRuntimeVariable("YongZheZhanChangUri", null);
				if (this.RemoteServiceUri != YongZheZhanChangUri)
				{
					this.RemoteServiceUri = YongZheZhanChangUri;
				}
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
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

		// Token: 0x06003A63 RID: 14947 RVA: 0x00318654 File Offset: 0x00316854
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("YongZheZhanChangUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		// Token: 0x06003A64 RID: 14948 RVA: 0x003186C4 File Offset: 0x003168C4
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x06003A65 RID: 14949 RVA: 0x003186CE File Offset: 0x003168CE
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x06003A66 RID: 14950 RVA: 0x003186D8 File Offset: 0x003168D8
		private IYongZheZhanChangService GetKuaFuService(bool noWait = false)
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
					IYongZheZhanChangService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IYongZheZhanChangService)Activator.GetObject(typeof(IYongZheZhanChangService), this.RemoteServiceUri);
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
							return this.KuaFuService;
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

		// Token: 0x06003A67 RID: 14951 RVA: 0x00318954 File Offset: 0x00316B54
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

		// Token: 0x06003A68 RID: 14952 RVA: 0x00318A08 File Offset: 0x00316C08
		public int RoleChangeState(int serverId, int rid, int state)
		{
			int result = -11;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
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

		// Token: 0x06003A69 RID: 14953 RVA: 0x00318A60 File Offset: 0x00316C60
		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11000;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					lock (this.Mutex)
					{
						YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
						if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
						{
							yongZheZhanChangFuBenData.State = state;
						}
					}
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

		// Token: 0x06003A6A RID: 14954 RVA: 0x00318B1C File Offset: 0x00316D1C
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

		// Token: 0x06003A6B RID: 14955 RVA: 0x00318B4C File Offset: 0x00316D4C
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x06003A6C RID: 14956 RVA: 0x00318B60 File Offset: 0x00316D60
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				int num = eventType;
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
				case 4:
				case 5:
					break;
				case 3:
					if (args.Length == 1)
					{
						KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
						if (null != kuaFuRoleData)
						{
							this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
							YongZheZhanChangFuBenData fuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
							if (fuBenData != null && fuBenData.State == GameFuBenState.Start)
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
								if (KuaFuManager.getInstance().TryGetValue(fuBenData.ServerId, out kuaFuServerInfo))
								{
									kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
									kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								}
								GameTypes gameType = (GameTypes)kuaFuRoleData.GameType;
								switch (gameType)
								{
								case GameTypes.YongZheZhanChang:
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 27);
									break;
								case GameTypes.KuaFuBoss:
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 31);
									break;
								default:
									if (gameType == GameTypes.KingOfBattle)
									{
										this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 39);
									}
									break;
								}
							}
						}
					}
					break;
				case 6:
					if (args.Length == 1)
					{
						LangHunLingYuBangHuiDataEx data = args[0] as LangHunLingYuBangHuiDataEx;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyBangHuiDataGameEvent(data), 35);
					}
					break;
				case 7:
					if (args.Length == 1)
					{
						LangHunLingYuCityDataEx data2 = args[0] as LangHunLingYuCityDataEx;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityDataGameEvent(data2), 35);
					}
					break;
				case 8:
					if (args.Length == 1)
					{
						Dictionary<int, List<int>> data3 = args[0] as Dictionary<int, List<int>>;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyOtherCityListGameEvent(data3), 35);
					}
					break;
				case 9:
					if (args.Length == 1)
					{
						List<LangHunLingYuKingHist> data4 = args[0] as List<LangHunLingYuKingHist>;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityOwnerHistGameEvent(data4), 35);
					}
					break;
				case 10:
					if (args.Length == 2)
					{
						int rid = (int)args[0];
						int admirecount = (int)args[1];
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityOwnerAdmireGameEvent(rid, admirecount), 35);
					}
					break;
				default:
					if (num == 9997)
					{
						if (GMCommands.EnableGMSetAllServerTime && item.Args.Length == 4)
						{
							string[] a = new string[item.Args.Length];
							for (int i = 0; i < a.Length; i++)
							{
								a[i] = (item.Args[i] as string);
								if (string.IsNullOrEmpty(a[i]))
								{
									return;
								}
							}
							if (a[0] == "-settime")
							{
								GMCommands.GMSetTime(null, a, false);
							}
						}
					}
					break;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06003A6D RID: 14957 RVA: 0x00318F74 File Offset: 0x00317174
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

		// Token: 0x06003A6E RID: 14958 RVA: 0x00318FF8 File Offset: 0x003171F8
		public int YongZheZhanChangSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
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
						IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
						if (null == kuaFuService)
						{
							return -11001;
						}
						try
						{
							IGameData YongZheZhanChangGameData = new IGameData
							{
								ZhanDouLi = zhanDouLi
							};
							int result = kuaFuService.RoleSignUp(this.ClientInfo.ServerId, userId, zoneId, roleId, gameType, groupIndex, YongZheZhanChangGameData);
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

		// Token: 0x06003A6F RID: 14959 RVA: 0x00319184 File Offset: 0x00317384
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
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(noWait);
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

		// Token: 0x06003A70 RID: 14960 RVA: 0x003192C4 File Offset: 0x003174C4
		public YongZheZhanChangFuBenData GetKuaFuFuBenData(int gameId)
		{
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData = null;
			lock (this.Mutex)
			{
				if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
				{
					return yongZheZhanChangFuBenData;
				}
			}
			if (yongZheZhanChangFuBenData == null)
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						yongZheZhanChangFuBenData = kuaFuService.GetFuBenData(gameId);
						if (null != yongZheZhanChangFuBenData)
						{
							lock (this.Mutex)
							{
								this.FuBenDataDict[gameId] = yongZheZhanChangFuBenData;
							}
						}
					}
					catch (Exception ex)
					{
						yongZheZhanChangFuBenData = null;
					}
				}
			}
			return yongZheZhanChangFuBenData;
		}

		// Token: 0x06003A71 RID: 14961 RVA: 0x003193D0 File Offset: 0x003175D0
		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, YongZheZhanChangFuBenData> kv in this.FuBenDataDict)
				{
					if (kv.Value.EndTime < now)
					{
						list.Add(kv.Key);
					}
				}
				foreach (int key in list)
				{
					this.FuBenDataDict.Remove(key);
				}
			}
		}

		// Token: 0x06003A72 RID: 14962 RVA: 0x003194DC File Offset: 0x003176DC
		public int GetKuaFuRoleState(int roleId)
		{
			int state = 0;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					object result = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 2);
					if (null != result)
					{
						state = (int)result;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return state;
		}

		// Token: 0x06003A73 RID: 14963 RVA: 0x0031955C File Offset: 0x0031775C
		public int GameFuBenRoleChangeState(int roleId, int state, int serverId = 0, int gameId = 0)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
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

		// Token: 0x06003A74 RID: 14964 RVA: 0x003195FC File Offset: 0x003177FC
		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		// Token: 0x06003A75 RID: 14965 RVA: 0x0031965C File Offset: 0x0031785C
		public KuaFuRoleData GetKuaFuRoleDataFromServer(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
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

		// Token: 0x06003A76 RID: 14966 RVA: 0x003196B4 File Offset: 0x003178B4
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			YongZheZhanChangFuBenData YongZheZhanChangFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
			if (YongZheZhanChangFuBenData != null && YongZheZhanChangFuBenData.State < GameFuBenState.End)
			{
				if (YongZheZhanChangFuBenData.ServerId == GameManager.ServerId)
				{
					if (YongZheZhanChangFuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId))
					{
						kuaFuServerLoginData.FuBenSeqId = YongZheZhanChangFuBenData.SequenceId;
						KuaFuRoleData kuaFuRoleData = this.GetKuaFuRoleDataFromServer(kuaFuServerLoginData.ServerId, kuaFuServerLoginData.RoleId);
						if (kuaFuRoleData.GameId == YongZheZhanChangFuBenData.GameId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06003A77 RID: 14967 RVA: 0x00319758 File Offset: 0x00317958
		public void PushGameResultData(object data)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateStatisticalData(new AsyncDataItem(KuaFuEventTypes.UpdateStatisticalData, new object[]
					{
						data
					}));
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		// Token: 0x06003A78 RID: 14968 RVA: 0x003197B8 File Offset: 0x003179B8
		public int ExecuteCommand(string cmd)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ExecuteCommand(cmd);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		// Token: 0x06003A79 RID: 14969 RVA: 0x0031980C File Offset: 0x00317A0C
		public object GetKuaFuLineDataList(int mapCode)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					AsyncDataItem item = kuaFuService.GetKuaFuLineDataList(mapCode);
					if (item != null && item.Args != null && item.Args.Length > 0)
					{
						return item.Args[0];
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x06003A7A RID: 14970 RVA: 0x0031988C File Offset: 0x00317A8C
		public int EnterKuaFuMap(int roleId, int mapCode, int kuaFuLine, int roleSourceServerId, KuaFuServerLoginData kuaFuServerLoginData)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					int kuaFuServerId = kuaFuService.EnterKuaFuMap(this.ClientInfo.ServerId, roleId, mapCode, kuaFuLine);
					if (kuaFuServerId > 0)
					{
						kuaFuServerLoginData.RoleId = roleId;
						kuaFuServerLoginData.ServerId = roleSourceServerId;
						kuaFuServerLoginData.GameType = 7;
						kuaFuServerLoginData.GameId = (long)mapCode;
						KuaFuServerInfo kuaFuServerInfo;
						if (KuaFuManager.getInstance().TryGetValue(kuaFuServerId, out kuaFuServerInfo))
						{
							kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
							kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
							return kuaFuServerId;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		// Token: 0x06003A7B RID: 14971 RVA: 0x0031995C File Offset: 0x00317B5C
		public bool CanEnterKuaFuMap(KuaFuServerLoginData kuaFuServerLoginData)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType((int)kuaFuServerLoginData.GameId);
			bool result;
			if (SceneUIClasses.ChongShengMap == sceneType)
			{
				result = true;
			}
			else
			{
				KuaFuMapRoleData kuaFuMapRoleData = YongZheZhanChangClient.getInstance().GetKuaFuMapRoleData(kuaFuServerLoginData.RoleId);
				if (kuaFuMapRoleData == null || kuaFuMapRoleData.KuaFuServerId != GameManager.ServerId || (long)kuaFuMapRoleData.KuaFuMapCode != kuaFuServerLoginData.GameId)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003A7C RID: 14972 RVA: 0x003199F4 File Offset: 0x00317BF4
		public KuaFuMapRoleData GetKuaFuMapRoleData(int roleId)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuMapRoleData(roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		// Token: 0x06003A7D RID: 14973 RVA: 0x00319A44 File Offset: 0x00317C44
		public int LangHunLingYuSignUp(string bhName, int bhid, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int result = -11000;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.LangHunLingYuSignUp(bhName, bhid, zoneId, gameType, groupIndex, zhanDouLi);
					}
					catch (Exception ex)
					{
						result = -11000;
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				result = -11000;
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		// Token: 0x06003A7E RID: 14974 RVA: 0x00319AD0 File Offset: 0x00317CD0
		public int GameFuBenComplete(LangHunLingYuStatisticalData data)
		{
			int result = -11000;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.GameFuBenComplete(data);
					}
					catch (Exception ex)
					{
						result = -11000;
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				result = -11000;
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		// Token: 0x06003A7F RID: 14975 RVA: 0x00319B54 File Offset: 0x00317D54
		public bool LangHunLingYunAdmire(int rid)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.LangHunLingYuAdmaire(rid);
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
			return false;
		}

		// Token: 0x06003A80 RID: 14976 RVA: 0x00319BC4 File Offset: 0x00317DC4
		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenData(int gameId)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.GetLangHunLingYuGameFuBenData(gameId);
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
			return null;
		}

		// Token: 0x06003A81 RID: 14977 RVA: 0x00319C34 File Offset: 0x00317E34
		public bool LangHunLingYuKuaFuLoginData(int roleId, int cityId, int gameId, KuaFuServerLoginData kuaFuServerLoginData)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						LangHunLingYuFuBenData fuBenData;
						lock (this.Mutex)
						{
							if (!this.LangHunLingYuFuBenDataDict.TryGetValue(gameId, out fuBenData))
							{
								fuBenData = null;
							}
						}
						if (null == fuBenData)
						{
							fuBenData = kuaFuService.GetLangHunLingYuGameFuBenDataByCityId(cityId);
						}
						if (null != fuBenData)
						{
							kuaFuServerLoginData.RoleId = roleId;
							kuaFuServerLoginData.GameId = (long)fuBenData.GameId;
							kuaFuServerLoginData.GameType = 10;
							kuaFuServerLoginData.EndTicks = fuBenData.EndTime.Ticks;
							kuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
							KuaFuServerInfo kuaFuServerInfo;
							if (KuaFuManager.getInstance().TryGetValue(fuBenData.ServerId, out kuaFuServerInfo))
							{
								kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
								kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								return true;
							}
						}
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
			return false;
		}

		// Token: 0x04004446 RID: 17478
		private static YongZheZhanChangClient instance = new YongZheZhanChangClient();

		// Token: 0x04004447 RID: 17479
		private object Mutex = new object();

		// Token: 0x04004448 RID: 17480
		private object RemotingMutex = new object();

		// Token: 0x04004449 RID: 17481
		private ICoreInterface CoreInterface = null;

		// Token: 0x0400444A RID: 17482
		private IYongZheZhanChangService KuaFuService = null;

		// Token: 0x0400444B RID: 17483
		private bool ClientInitialized = false;

		// Token: 0x0400444C RID: 17484
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x0400444D RID: 17485
		public int SceneType = 27;

		// Token: 0x0400444E RID: 17486
		private int CurrentRequestCount = 0;

		// Token: 0x0400444F RID: 17487
		private int MaxRequestCount = 50;

		// Token: 0x04004450 RID: 17488
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		// Token: 0x04004451 RID: 17489
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		// Token: 0x04004452 RID: 17490
		private Dictionary<int, YongZheZhanChangFuBenData> FuBenDataDict = new Dictionary<int, YongZheZhanChangFuBenData>();

		// Token: 0x04004453 RID: 17491
		private Dictionary<int, LangHunLingYuFuBenData> LangHunLingYuFuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();

		// Token: 0x04004454 RID: 17492
		private DateTime NextClearFuBenTime;

		// Token: 0x04004455 RID: 17493
		private string RemoteServiceUri = null;

		// Token: 0x04004456 RID: 17494
		private DuplexChannelFactory<IYongZheZhanChangService> channelFactory;
	}
}
