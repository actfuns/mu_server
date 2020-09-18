using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	
	public class KuaFuWorldClient : IManager2
	{
		
		public static KuaFuWorldClient getInstance()
		{
			return KuaFuWorldClient.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.KuaFuServerId;
			this.ClientInfo.GameType = 32;
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
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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
				string KuaFuWorldUri = this.CoreInterface.GetRuntimeVariable("KuaFuWorldUri", null);
				if (this.RemoteServiceUri != KuaFuWorldUri)
				{
					this.RemoteServiceUri = KuaFuWorldUri;
				}
				IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						if (KuaFuManager.KuaFuWorldKuaFuGameServer)
						{
							List<KuaFuServerInfo> dict = kuaFuService.GetKuaFuServerInfoData(KuaFuManager.getInstance().GetServerInfoAsyncAge());
							KuaFuManager.getInstance().UpdateServerInfoList(dict);
						}
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("KuaFuWorldUri", null);
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

		
		private IKuaFuWorld GetKuaFuService(bool noWait = false)
		{
			try
			{
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
					IKuaFuWorld kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IKuaFuWorld)Activator.GetObject(typeof(IKuaFuWorld), this.RemoteServiceUri);
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

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				int num = eventType;
				if (num != 39)
				{
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
				}
				else
				{
					RebornManager.getInstance().OnChatListData(args[0] as byte[]);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public int ExecuteCommand(string cmd)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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

		
		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.RegPTKuaFuRoleData(ref data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public int EnterPTKuaFuMap(int roleSourceServerId, int roleId, int ptid, int mapCode, int kuaFuLine, KuaFuServerLoginData kuaFuServerLoginData, out string signToken, out string signKey)
		{
			signToken = null;
			signKey = null;
			int tempRoleID = -11000;
			int kuaFuServerID = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					string[] ips;
					int[] ports;
					tempRoleID = kuaFuService.EnterPTKuaFuMap(roleSourceServerId, roleId, ptid, mapCode, kuaFuLine, out signToken, out signKey, out kuaFuServerID, out ips, out ports);
					if (tempRoleID <= 0)
					{
						return tempRoleID;
					}
					kuaFuServerLoginData.RoleId = roleId;
					kuaFuServerLoginData.ServerId = (KuaFuManager.KuaFuWorldKuaFuGameServer ? roleSourceServerId : GameManager.KuaFuServerId);
					kuaFuServerLoginData.GameType = 32;
					kuaFuServerLoginData.GameId = (long)mapCode;
					kuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
					kuaFuServerLoginData.TargetServerID = kuaFuServerID;
					KuaFuServerInfo kuaFuServerInfo;
					if (ips != null && ports != null)
					{
						kuaFuServerLoginData.ServerIp = ips[0];
						kuaFuServerLoginData.ServerPort = ports[0];
					}
					else if (KuaFuManager.getInstance().TryGetValue(kuaFuServerID, out kuaFuServerInfo))
					{
						kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
						kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return tempRoleID;
		}

		
		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			signKey = null;
			ips = null;
			ports = null;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CheckEnterWorldKuaFuSign(worldRoleID, token, out signKey, out ips, out ports);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public int Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_SetRoleData4Selector(ptId, roleId, bytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			int result = -11;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Reborn_RoleReborn(ptId, roleId, roleName, level);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.Reborn_SyncData(ageRank, ageBoss);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public int Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_RebornOpt(ptid, rid, optType, param1, param2, param3);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public KFRebornRoleData Reborn_GetRebornRoleData(int ptId, int roleId)
		{
			try
			{
				IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
						KuaFuData<KFRebornRoleData> RebornRoleData = null;
						if (!this.RebornRoleDataDict.TryGetValue(key, out RebornRoleData))
						{
							RebornRoleData = new KuaFuData<KFRebornRoleData>();
							this.RebornRoleDataDict[key] = RebornRoleData;
						}
						KuaFuCmdData result = kuaFuService.Reborn_GetRebornRoleData(ptId, roleId, RebornRoleData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > RebornRoleData.Age)
						{
							RebornRoleData.Age = result.Age;
							if (null != result.Bytes0)
							{
								RebornRoleData.V = DataHelper2.BytesToObject<KFRebornRoleData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null != RebornRoleData.V)
							{
								this.RebornRoleDataDict[key] = RebornRoleData;
							}
						}
						return RebornRoleData.V;
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

		
		public int Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_ChangeName(ptId, roleId, roleName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public int Reborn_PlatFormChat(List<KFPlatFormChat> chatList)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] sendBytes = DataHelper.ObjectToBytes<List<KFPlatFormChat>>(chatList);
					kuaFuService.Reborn_PlatFormChat(this.ClientInfo.ServerId, sendBytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		private static KuaFuWorldClient instance = new KuaFuWorldClient();

		
		private object Mutex = new object();

		
		private object RemotingMutex = new object();

		
		private ICoreInterface CoreInterface = null;

		
		private IKuaFuWorld KuaFuService = null;

		
		private bool ClientInitialized = false;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		private string RemoteServiceUri = null;

		
		public Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>>();

		
		private DuplexChannelFactory<IKuaFuWorld> channelFactory;
	}
}
