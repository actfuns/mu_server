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
	public class HuanYingSiYuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static HuanYingSiYuanClient getInstance()
		{
			return HuanYingSiYuanClient.instance;
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
			this.ClientInfo.GameType = 1;
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

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string huanYingSiYuanUri = this.CoreInterface.GetRuntimeVariable("HuanYingSiYuanUri", null);
				if (this.RemoteServiceUri != huanYingSiYuanUri)
				{
					this.RemoteServiceUri = huanYingSiYuanUri;
				}
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						List<KuaFuServerInfo> dict = kuaFuService.GetKuaFuServerInfoData(KuaFuManager.getInstance().GetServerInfoAsyncAge());
						KuaFuManager.getInstance().UpdateServerInfoList(dict);
						AsyncData asyncData = kuaFuService.GetClientCacheItems2(this.ClientInfo.ServerId, TimeUtil.NOW());
						long nowTicks = TimeUtil.NOW();
						long subTicks = nowTicks - asyncData.RequestTicks;
						if (subTicks < 200L)
						{
							if (TimeUtil.AsyncNetTicks(asyncData.RequestTicks, asyncData.ServerTicks))
							{
								LogManager.WriteLog(LogTypes.Ignore, string.Format("时间漂移#local={0},server={1}", asyncData.RequestTicks, asyncData.ServerTicks), null, true);
							}
						}
						AsyncDataItem[] items = asyncData.ItemList;
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("HuanYingSiYuanUri", null);
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

		
		private IKuaFuService GetKuaFuService(bool noWait = false)
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
					IKuaFuService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IKuaFuService)Activator.GetObject(typeof(IKuaFuService), this.RemoteServiceUri);
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
				int oldRoleId = 0;
				int oldServerId = 0;
				lock (this.Mutex)
				{
					if (kuaFuRoleData.State == KuaFuRoleStates.None)
					{
						this.RemoveRoleData(kuaFuRoleData.RoleId);
						return 0;
					}
					string userId = kuaFuRoleData.UserId;
					KuaFuRoleData oldKuaFuRoleData;
					if (this.UserId2RoleDataDict.TryGetValue(userId, out oldKuaFuRoleData) && oldKuaFuRoleData.RoleId != roleId)
					{
						oldRoleId = oldKuaFuRoleData.RoleId;
						int gameType = oldKuaFuRoleData.GameType;
						int groupIndex = oldKuaFuRoleData.GroupIndex;
						oldServerId = oldKuaFuRoleData.ServerId;
					}
					if (oldKuaFuRoleData != kuaFuRoleData)
					{
						this.UserId2RoleDataDict[userId] = kuaFuRoleData;
						this.RoleId2RoleDataDict[roleId] = kuaFuRoleData;
						this.RoleId2KuaFuStateDict[roleId] = (int)kuaFuRoleData.State;
					}
				}
				if (oldRoleId > 0)
				{
					this.RoleChangeState(oldServerId, oldRoleId, 0);
					this.RemoveRoleData(oldRoleId);
				}
				result2 = result;
			}
			return result2;
		}

		
		public int RoleChangeState(int serverId, int rid, int state)
		{
			int result = -11;
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
				if (num <= 9996)
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
								HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
								if (huanYingSiYuanFuBenData != null && huanYingSiYuanFuBenData.State == GameFuBenState.Start)
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
									if (KuaFuManager.getInstance().TryGetValue(huanYingSiYuanFuBenData.ServerId, out kuaFuServerInfo))
									{
										kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
										kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
									}
									else
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("服务器列表中无法找到serverid={0}的IP和端口信息", huanYingSiYuanFuBenData.ServerId), null, true);
									}
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), this.SceneType);
								}
							}
						}
						break;
					default:
						if (num == 9996)
						{
							if (args.Length == 1)
							{
								GMCmdData data = args[0] as GMCmdData;
								GVoiceManager.getInstance().UpdateGVoicePriority(data, true);
							}
						}
						break;
					}
				}
				else if (num != 10015)
				{
					if (num == 10029)
					{
						KuaFuLueDuoNtfEnterData data2 = args[0] as KuaFuLueDuoNtfEnterData;
						KuaFuLueDuoManager.getInstance().HandleNtfEnterEvent(data2);
					}
				}
				else if (args != null && args.Length == 2)
				{
					KuaFuManager.getInstance().UpdateServerInfoList(args[1] as List<KuaFuServerInfo>);
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

		
		public int HuanYingSiYuanSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
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
						IKuaFuService kuaFuService = this.GetKuaFuService(false);
						if (null == kuaFuService)
						{
							return -11001;
						}
						try
						{
							IGameData huanYingSiYuanGameData = new IGameData
							{
								ZhanDouLi = zhanDouLi
							};
							int result = kuaFuService.RoleSignUp(this.ClientInfo.ServerId, userId, zoneId, roleId, gameType, groupIndex, huanYingSiYuanGameData);
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
			IKuaFuService kuaFuService = this.GetKuaFuService(noWait);
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

		
		private HuanYingSiYuanFuBenData GetKuaFuFuBenData(int gameId)
		{
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			if (huanYingSiYuanFuBenData == null)
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						huanYingSiYuanFuBenData = kuaFuService.GetFuBenData(gameId);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						huanYingSiYuanFuBenData = null;
					}
				}
			}
			return huanYingSiYuanFuBenData;
		}

		
		public int GetRoleKuaFuFuBenRoleCount(int roleId)
		{
			int roleCount = 0;
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					int result = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 0);
					bool flag = 1 == 0;
					roleCount = result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return roleCount;
		}

		
		public int GameFuBenRoleChangeState(int roleId, int state, int serverId = 0, int gameId = 0)
		{
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
				KuaFuRoleData kuaFuRoleData;
				if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					this.UserId2RoleDataDict.Remove(kuaFuRoleData.UserId);
				}
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		
		public KuaFuRoleData GetKuaFuRoleDataFromServer(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
			if (huanYingSiYuanFuBenData != null && huanYingSiYuanFuBenData.State < GameFuBenState.End)
			{
				if (huanYingSiYuanFuBenData.ServerId == GameManager.ServerId)
				{
					if (huanYingSiYuanFuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId))
					{
						KuaFuRoleData kuaFuRoleData = this.GetKuaFuRoleDataFromServer(kuaFuServerLoginData.ServerId, kuaFuServerLoginData.RoleId);
						if (kuaFuRoleData.GameId == huanYingSiYuanFuBenData.GameId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		
		public int GetRoleBattleWhichSide(int gameId, int roleId)
		{
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = this.GetKuaFuFuBenData(gameId);
			if (huanYingSiYuanFuBenData != null && huanYingSiYuanFuBenData.State < GameFuBenState.End)
			{
				if (huanYingSiYuanFuBenData.ServerId == this.ClientInfo.ServerId)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (huanYingSiYuanFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
					{
						return kuaFuFuBenRoleData.Side;
					}
				}
			}
			return 0;
		}

		
		public int UseGiftCode(string ptid, string uid, string rid, string channel, string codeno, string appid, int zoneid, ref string giftid)
		{
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.UseGiftCode(ptid, uid, rid, channel, codeno, appid, zoneid, ref giftid);
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
			return -11000;
		}

		
		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						kuaFuService.BroadcastGMCmdData(data, serverFlag);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.JingJia_KuaFuLueDuo(bhid, zoneid_bh, bhname, ziJin, serverId, oldZiJin);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return -11000;
		}

		
		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByServerId_KuaFuLueDuo(serverId);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		
		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameId)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_KuaFuLueDuo(gameId);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		
		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetRoleData_KuaFuLueDuo(rid);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		
		public KuaFuLueDuoSyncData SyncData_KuaFuLueDuo(KuaFuLueDuoSyncData SyncData)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] result = kuaFuService.SyncData_KuaFuLueDuo(DataHelper.ObjectToBytes<KuaFuLueDuoSyncData>(SyncData));
					if (null != result)
					{
						return DataHelper.BytesToObject<KuaFuLueDuoSyncData>(result, 0, result.Length);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public KuaFuLueDuoBHData GetBHDataByBhid_KuaFuLueDuo(int bhid)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KuaFuLueDuoBHData> bhdata = null;
						if (!this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out bhdata))
						{
							bhdata = new KuaFuData<KuaFuLueDuoBHData>();
							this.KuaFuLueDuoBHDataDict[bhid] = bhdata;
						}
						KuaFuCmdData result = kuaFuService.GetBHDataByBhid_KuaFuLueDuo(bhid, bhdata.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > bhdata.Age)
						{
							bhdata.Age = result.Age;
							if (null != result.Bytes0)
							{
								bhdata.V = DataHelper2.BytesToObject<KuaFuLueDuoBHData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null != bhdata.V)
							{
								this.KuaFuLueDuoBHDataDict[bhid] = bhdata;
							}
						}
						return bhdata.V;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		
		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GameFuBenComplete_KuaFuLueDuo(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		private static HuanYingSiYuanClient instance = new HuanYingSiYuanClient();

		
		private object Mutex = new object();

		
		private object RemotingMutex = new object();

		
		private ICoreInterface CoreInterface = null;

		
		private IKuaFuService KuaFuService = null;

		
		private bool ClientInitialized = false;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		public int SceneType = 25;

		
		public GameTypes GameType = GameTypes.HuanYingSiYuan;

		
		private int CurrentRequestCount = 0;

		
		private int MaxRequestCount = 50;

		
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		
		private Dictionary<string, KuaFuRoleData> UserId2RoleDataDict = new Dictionary<string, KuaFuRoleData>();

		
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		
		public Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict = new Dictionary<int, KuaFuData<KuaFuLueDuoBHData>>();

		
		private string RemoteServiceUri = null;

		
		private DuplexChannelFactory<IKuaFuService> channelFactory;
	}
}
