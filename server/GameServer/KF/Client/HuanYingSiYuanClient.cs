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
	// Token: 0x020002F4 RID: 756
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class HuanYingSiYuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06000BFF RID: 3071 RVA: 0x000BD180 File Offset: 0x000BB380
		public static HuanYingSiYuanClient getInstance()
		{
			return HuanYingSiYuanClient.instance;
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x000BD198 File Offset: 0x000BB398
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x000BD1AC File Offset: 0x000BB3AC
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 1;
			return true;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x000BD1F4 File Offset: 0x000BB3F4
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x000BD208 File Offset: 0x000BB408
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x000BD21C File Offset: 0x000BB41C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x000BD230 File Offset: 0x000BB430
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

		// Token: 0x06000C06 RID: 3078 RVA: 0x000BD284 File Offset: 0x000BB484
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

		// Token: 0x06000C07 RID: 3079 RVA: 0x000BD3F0 File Offset: 0x000BB5F0
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("HuanYingSiYuanUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x000BD460 File Offset: 0x000BB660
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000BD46A File Offset: 0x000BB66A
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x000BD474 File Offset: 0x000BB674
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

		// Token: 0x06000C0B RID: 3083 RVA: 0x000BD6EC File Offset: 0x000BB8EC
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

		// Token: 0x06000C0C RID: 3084 RVA: 0x000BD840 File Offset: 0x000BBA40
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

		// Token: 0x06000C0D RID: 3085 RVA: 0x000BD898 File Offset: 0x000BBA98
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

		// Token: 0x06000C0E RID: 3086 RVA: 0x000BD8F8 File Offset: 0x000BBAF8
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

		// Token: 0x06000C0F RID: 3087 RVA: 0x000BD928 File Offset: 0x000BBB28
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x000BD93C File Offset: 0x000BBB3C
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

		// Token: 0x06000C11 RID: 3089 RVA: 0x000BDC20 File Offset: 0x000BBE20
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

		// Token: 0x06000C12 RID: 3090 RVA: 0x000BDCA4 File Offset: 0x000BBEA4
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

		// Token: 0x06000C13 RID: 3091 RVA: 0x000BDE30 File Offset: 0x000BC030
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

		// Token: 0x06000C14 RID: 3092 RVA: 0x000BDF70 File Offset: 0x000BC170
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

		// Token: 0x06000C15 RID: 3093 RVA: 0x000BDFD8 File Offset: 0x000BC1D8
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

		// Token: 0x06000C16 RID: 3094 RVA: 0x000BE048 File Offset: 0x000BC248
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

		// Token: 0x06000C17 RID: 3095 RVA: 0x000BE0E8 File Offset: 0x000BC2E8
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

		// Token: 0x06000C18 RID: 3096 RVA: 0x000BE170 File Offset: 0x000BC370
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

		// Token: 0x06000C19 RID: 3097 RVA: 0x000BE1C8 File Offset: 0x000BC3C8
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

		// Token: 0x06000C1A RID: 3098 RVA: 0x000BE260 File Offset: 0x000BC460
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

		// Token: 0x06000C1B RID: 3099 RVA: 0x000BE2D0 File Offset: 0x000BC4D0
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

		// Token: 0x06000C1C RID: 3100 RVA: 0x000BE350 File Offset: 0x000BC550
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

		// Token: 0x06000C1D RID: 3101 RVA: 0x000BE3C4 File Offset: 0x000BC5C4
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

		// Token: 0x06000C1E RID: 3102 RVA: 0x000BE428 File Offset: 0x000BC628
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

		// Token: 0x06000C1F RID: 3103 RVA: 0x000BE47C File Offset: 0x000BC67C
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

		// Token: 0x06000C20 RID: 3104 RVA: 0x000BE4D0 File Offset: 0x000BC6D0
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

		// Token: 0x06000C21 RID: 3105 RVA: 0x000BE524 File Offset: 0x000BC724
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

		// Token: 0x06000C22 RID: 3106 RVA: 0x000BE594 File Offset: 0x000BC794
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

		// Token: 0x06000C23 RID: 3107 RVA: 0x000BE710 File Offset: 0x000BC910
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

		// Token: 0x0400137B RID: 4987
		private static HuanYingSiYuanClient instance = new HuanYingSiYuanClient();

		// Token: 0x0400137C RID: 4988
		private object Mutex = new object();

		// Token: 0x0400137D RID: 4989
		private object RemotingMutex = new object();

		// Token: 0x0400137E RID: 4990
		private ICoreInterface CoreInterface = null;

		// Token: 0x0400137F RID: 4991
		private IKuaFuService KuaFuService = null;

		// Token: 0x04001380 RID: 4992
		private bool ClientInitialized = false;

		// Token: 0x04001381 RID: 4993
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x04001382 RID: 4994
		public int SceneType = 25;

		// Token: 0x04001383 RID: 4995
		public GameTypes GameType = GameTypes.HuanYingSiYuan;

		// Token: 0x04001384 RID: 4996
		private int CurrentRequestCount = 0;

		// Token: 0x04001385 RID: 4997
		private int MaxRequestCount = 50;

		// Token: 0x04001386 RID: 4998
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		// Token: 0x04001387 RID: 4999
		private Dictionary<string, KuaFuRoleData> UserId2RoleDataDict = new Dictionary<string, KuaFuRoleData>();

		// Token: 0x04001388 RID: 5000
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		// Token: 0x04001389 RID: 5001
		public Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict = new Dictionary<int, KuaFuData<KuaFuLueDuoBHData>>();

		// Token: 0x0400138A RID: 5002
		private string RemoteServiceUri = null;

		// Token: 0x0400138B RID: 5003
		private DuplexChannelFactory<IKuaFuService> channelFactory;
	}
}
