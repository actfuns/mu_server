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
	// Token: 0x02000350 RID: 848
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class KFCopyRpcClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06000E68 RID: 3688 RVA: 0x000E3E84 File Offset: 0x000E2084
		public static KFCopyRpcClient getInstance()
		{
			return KFCopyRpcClient.instance;
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x000E3E9C File Offset: 0x000E209C
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 8;
			return true;
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x000E3EE4 File Offset: 0x000E20E4
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x000E3EF8 File Offset: 0x000E20F8
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x000E3F0C File Offset: 0x000E210C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x000E3F20 File Offset: 0x000E2120
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

		// Token: 0x06000E6E RID: 3694 RVA: 0x000E41D4 File Offset: 0x000E23D4
		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x000E41E8 File Offset: 0x000E23E8
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

		// Token: 0x06000E70 RID: 3696 RVA: 0x000E4218 File Offset: 0x000E2418
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

		// Token: 0x06000E71 RID: 3697 RVA: 0x000E42C4 File Offset: 0x000E24C4
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

		// Token: 0x06000E72 RID: 3698 RVA: 0x000E4348 File Offset: 0x000E2548
		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000E43A8 File Offset: 0x000E25A8
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

		// Token: 0x06000E74 RID: 3700 RVA: 0x000E449C File Offset: 0x000E269C
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

		// Token: 0x06000E75 RID: 3701 RVA: 0x000E4514 File Offset: 0x000E2714
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x000E451E File Offset: 0x000E271E
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x000E4528 File Offset: 0x000E2728
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

		// Token: 0x06000E78 RID: 3704 RVA: 0x000E47A0 File Offset: 0x000E29A0
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

		// Token: 0x06000E79 RID: 3705 RVA: 0x000E47E8 File Offset: 0x000E29E8
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

		// Token: 0x06000E7A RID: 3706 RVA: 0x000E4818 File Offset: 0x000E2A18
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

		// Token: 0x06000E7B RID: 3707 RVA: 0x000E4848 File Offset: 0x000E2A48
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

		// Token: 0x06000E7C RID: 3708 RVA: 0x000E4878 File Offset: 0x000E2A78
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

		// Token: 0x06000E7D RID: 3709 RVA: 0x000E48A8 File Offset: 0x000E2AA8
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

		// Token: 0x06000E7E RID: 3710 RVA: 0x000E48D8 File Offset: 0x000E2AD8
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

		// Token: 0x06000E7F RID: 3711 RVA: 0x000E4908 File Offset: 0x000E2B08
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

		// Token: 0x06000E80 RID: 3712 RVA: 0x000E4938 File Offset: 0x000E2B38
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

		// Token: 0x06000E81 RID: 3713 RVA: 0x000E4968 File Offset: 0x000E2B68
		public void KFCopyTeamRemove(long teamId)
		{
			IKuaFuCopyService service = this.GetKuaFuService(false);
			if (service != null)
			{
				service.RemoveTeam(teamId);
			}
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x000E4994 File Offset: 0x000E2B94
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

		// Token: 0x06000E83 RID: 3715 RVA: 0x000E49D4 File Offset: 0x000E2BD4
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

		// Token: 0x06000E84 RID: 3716 RVA: 0x000E4A34 File Offset: 0x000E2C34
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

		// Token: 0x06000E85 RID: 3717 RVA: 0x000E4A94 File Offset: 0x000E2C94
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

		// Token: 0x06000E86 RID: 3718 RVA: 0x000E4AF4 File Offset: 0x000E2CF4
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

		// Token: 0x06000E87 RID: 3719 RVA: 0x000E4B54 File Offset: 0x000E2D54
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

		// Token: 0x0400165D RID: 5725
		private const int MaxRequestCount = 50;

		// Token: 0x0400165E RID: 5726
		public const int SceneType = 10001;

		// Token: 0x0400165F RID: 5727
		private static readonly KFCopyRpcClient instance = new KFCopyRpcClient();

		// Token: 0x04001660 RID: 5728
		private object Mutex = new object();

		// Token: 0x04001661 RID: 5729
		private object RemotingMutex = new object();

		// Token: 0x04001662 RID: 5730
		private int CurrentRequestCount = 0;

		// Token: 0x04001663 RID: 5731
		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		// Token: 0x04001664 RID: 5732
		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		// Token: 0x04001665 RID: 5733
		private int ServerInfoAsyncAge = 0;

		// Token: 0x04001666 RID: 5734
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x04001667 RID: 5735
		private string RemoteServiceUri = null;

		// Token: 0x04001668 RID: 5736
		private IKuaFuCopyService KuaFuService = null;

		// Token: 0x04001669 RID: 5737
		private ICoreInterface CoreInterface = null;

		// Token: 0x0400166A RID: 5738
		private DuplexChannelFactory<IKuaFuCopyService> channelFactory;
	}
}
