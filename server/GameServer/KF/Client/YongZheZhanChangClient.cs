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
	
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class YongZheZhanChangClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static YongZheZhanChangClient getInstance()
		{
			return YongZheZhanChangClient.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 5;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
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

		
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("YongZheZhanChangUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		
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

		
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		
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

		
		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		
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

		
		private static YongZheZhanChangClient instance = new YongZheZhanChangClient();

		
		private object Mutex = new object();

		
		private object RemotingMutex = new object();

		
		private ICoreInterface CoreInterface = null;

		
		private IYongZheZhanChangService KuaFuService = null;

		
		private bool ClientInitialized = false;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		public int SceneType = 27;

		
		private int CurrentRequestCount = 0;

		
		private int MaxRequestCount = 50;

		
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		
		private Dictionary<int, YongZheZhanChangFuBenData> FuBenDataDict = new Dictionary<int, YongZheZhanChangFuBenData>();

		
		private Dictionary<int, LangHunLingYuFuBenData> LangHunLingYuFuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();

		
		private DateTime NextClearFuBenTime;

		
		private string RemoteServiceUri = null;

		
		private DuplexChannelFactory<IYongZheZhanChangService> channelFactory;
	}
}
