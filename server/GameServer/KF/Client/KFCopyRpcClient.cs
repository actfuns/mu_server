using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class KFCopyRpcClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static KFCopyRpcClient getInstance()
		{
			return KFCopyRpcClient.instance;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 8;
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

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				switch (eventType)
				{
				case 10000:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomCreateEvent((CopyTeamCreateData)args[1]), 10001);
					}
					break;
				case 10001:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomJoinEvent((CopyTeamJoinData)args[1]), 10001);
					}
					break;
				case 10002:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomKickoutEvent((CopyTeamKickoutData)args[1]), 10001);
					}
					break;
				case 10003:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomLeaveEvent((CopyTeamLeaveData)args[1]), 10001);
					}
					break;
				case 10004:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomReadyEvent((CopyTeamReadyData)args[1]), 10001);
					}
					break;
				case 10005:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomStartEvent((CopyTeamStartData)args[1]), 10001);
					}
					break;
				case 10006:
					if (args != null && args.Length == 1)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyTeamDestroyEvent((CopyTeamDestroyData)args[0]), 10001);
					}
					break;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
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

		
		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string moRiJudgeUri = this.CoreInterface.GetRuntimeVariable("KuaFuCopyUri", null);
				if (this.RemoteServiceUri != moRiJudgeUri)
				{
					this.RemoteServiceUri = moRiJudgeUri;
				}
				IKuaFuCopyService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] items = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId);
						if (items != null && items.Length > 0)
						{
							foreach (AsyncDataItem item in items)
							{
								this.EventCallBackHandler(item);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "KFCopyRpcClient.TimerProc调度异常", ex, true);
				this.ResetKuaFuService();
			}
		}

		
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.ServerInfoAsyncAge = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("KuaFuCopyUri", null);
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

		
		private IKuaFuCopyService GetKuaFuService(bool noWait = false)
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
					IKuaFuCopyService tmpKuaFuService;
					if (this.KuaFuService == null)
					{
						tmpKuaFuService = (IKuaFuCopyService)Activator.GetObject(typeof(IKuaFuCopyService), this.RemoteServiceUri);
						if (null == tmpKuaFuService)
						{
							return null;
						}
					}
					else
					{
						tmpKuaFuService = this.KuaFuService;
					}
					int clientId = this.ClientInfo.ClientId;
					long nowTicks = TimeUtil.NOW();
					if (clientId <= 0 || Math.Abs(nowTicks - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = nowTicks;
						clientId = tmpKuaFuService.InitializeClient(this.ClientInfo);
					}
					if (tmpKuaFuService != null && (clientId != this.ClientInfo.ClientId || this.KuaFuService != tmpKuaFuService))
					{
						lock (this.Mutex)
						{
							if (clientId > 0)
							{
								this.KuaFuService = tmpKuaFuService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = clientId;
							return tmpKuaFuService;
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

		
		public bool GetKuaFuGSInfo(int serverId, out string gsIp, out int gsPort)
		{
			gsIp = string.Empty;
			gsPort = 0;
			KuaFuServerInfo info = null;
			bool result;
			if (!KuaFuManager.getInstance().TryGetValue(serverId, out info))
			{
				result = false;
			}
			else
			{
				gsIp = info.Ip;
				gsPort = info.Port;
				result = true;
			}
			return result;
		}

		
		public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamCreateRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.CreateTeam(req);
			}
			return result;
		}

		
		public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamJoinRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.JoinTeam(req);
			}
			return result;
		}

		
		public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamStartRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.StartGame(req);
			}
			return result;
		}

		
		public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamKickoutRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.KickoutTeam(req);
			}
			return result;
		}

		
		public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamLeaveRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.LeaveTeam(req);
			}
			return result;
		}

		
		public KFCopyTeamSetReadyRsp SetReady(KFCopyTeamSetReadyReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamSetReadyRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.TeamSetReady(req);
			}
			return result;
		}

		
		public KFCopyTeamSetFlagRsp SetFlag(KFCopyTeamSetFlagReq req)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			KFCopyTeamSetFlagRsp result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.TeamSetFlag(req);
			}
			return result;
		}

		
		public CopyTeamData GetTeamData(long teamid)
		{
			IKuaFuCopyService service = this.GetKuaFuService(false);
			CopyTeamData result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				result = service.GetTeamData(teamid);
			}
			return result;
		}

		
		public void KFCopyTeamRemove(long teamId)
		{
			IKuaFuCopyService service = this.GetKuaFuService(false);
			if (service != null)
			{
				service.RemoveTeam(teamId);
			}
		}

		
		public InputKingPaiHangDataEx GetPlatChargeKing()
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			InputKingPaiHangDataEx result;
			if (service == null)
			{
				result = null;
			}
			else
			{
				object obj = service.GetPlatChargeKing();
				result = ((obj != null) ? (obj as InputKingPaiHangDataEx) : null);
			}
			return result;
		}

		
		public List<InputKingPaiHangDataEx> GetPlatChargeKingEveryDay(DateTime FromDate, DateTime ToDate)
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			if (service != null)
			{
				try
				{
					object obj = service.GetPlatChargeKingEveryDay(FromDate, ToDate);
					return (obj != null) ? (obj as List<InputKingPaiHangDataEx>) : null;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long result = -11000L;
			IKuaFuCopyService service = this.GetKuaFuService(true);
			if (service != null)
			{
				try
				{
					result = service.QueryHuodongAwardUserHist(actType, huoDongKeyStr, userid);
				}
				catch (Exception ex)
				{
					result = -11003L;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			int result = -11000;
			IKuaFuCopyService service = this.GetKuaFuService(true);
			if (service != null)
			{
				try
				{
					result = service.UpdateHuodongAwardUserHist(actType, huoDongKeyStr, userid, extTag);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			int result = -11000;
			IKuaFuCopyService service = this.GetKuaFuService(true);
			if (service != null)
			{
				try
				{
					result = service.SpecPriority_ModifyActivityConditionNum(key, add);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			IKuaFuCopyService service = this.GetKuaFuService(true);
			if (service != null)
			{
				try
				{
					object obj = service.SpecPriority_GetActivityConditionInfo();
					return (obj != null) ? (obj as SpecPrioritySyncData) : null;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		private const int MaxRequestCount = 50;

		
		public const int SceneType = 10001;

		
		private static readonly KFCopyRpcClient instance = new KFCopyRpcClient();

		
		private object Mutex = new object();

		
		private object RemotingMutex = new object();

		
		private int CurrentRequestCount = 0;

		
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		
		private int ServerInfoAsyncAge = 0;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		private string RemoteServiceUri = null;

		
		private IKuaFuCopyService KuaFuService = null;

		
		private ICoreInterface CoreInterface = null;

		
		private DuplexChannelFactory<IKuaFuCopyService> channelFactory;
	}
}
