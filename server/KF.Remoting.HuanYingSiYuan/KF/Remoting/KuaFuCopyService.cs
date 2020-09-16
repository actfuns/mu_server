using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class KuaFuCopyService : MarshalByRefObject, IKuaFuCopyService
	{
		
		public InputKingPaiHangDataEx GetPlatChargeKing()
		{
			return this.PCKMgr.GetRankEx();
		}

		
		public List<InputKingPaiHangDataEx> GetPlatChargeKingEveryDay(DateTime FromDate, DateTime ToDate)
		{
			return this.PCKMgr.GetRankExList(FromDate, ToDate);
		}

		
		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			return this.PCKMgr.QueryHuodongAwardUserHist(actType, huoDongKeyStr, userid);
		}

		
		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			return this.PCKMgr.UpdateHuodongAwardUserHist(actType, huoDongKeyStr, userid, extTag);
		}

		
		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			return SpecPriorityActivityMgr.Instance().SpecPriority_ModifyActivityConditionNum(key, add);
		}

		
		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			return SpecPriorityActivityMgr.Instance().SpecPriority_GetActivityConditionInfo();
		}

		
		public override object InitializeLifetimeService()
		{
			KuaFuCopyService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		
		public KuaFuCopyService()
		{
			KuaFuCopyService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
			this.PlatChargeKingThread = new Thread(delegate()
			{
				for (;;)
				{
					try
					{
						this.PCKMgr.Update();
						Thread.Sleep(20000);
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
				}
			});
			this.PlatChargeKingThread.IsBackground = true;
			this.PlatChargeKingThread.Start();
			this.teamMgr.SetService(this);
		}

		
		~KuaFuCopyService()
		{
			this.BackgroundThread.Abort();
			this.PlatChargeKingThread.Abort();
		}

		
		public void RemoveGameTeam(int kfSrvId, long teamId)
		{
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, kfSrvId, teamId);
		}

		
		private void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.dbMgr.Initialized);
			for (;;)
			{
				try
				{
					long nowMs = TimeUtil.NOW();
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					if (nowMs >= this.LastSaveServerStateMs + 30000L)
					{
						this.LastSaveServerStateMs = nowMs;
						this.dbMgr.SaveCopyTeamAnalysisData(this.teamMgr.BuildAnalysisData());
					}
					this.teamMgr.Update();
					AsyncDataItem[] evList = this.teamMgr.PopAsyncEvent();
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, evList);
					this.dbMgr.CheckLogAsyncEvents(evList);
					SpecPriorityActivityMgr.Instance().Update(now);
					int sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
					this.dbMgr.SaveCostTime(sleepMS);
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

		
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		
		public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
		{
			return this.teamMgr.CreateTeam(req);
		}

		
		public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
		{
			return this.teamMgr.JoinTeam(req);
		}

		
		public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
		{
			return this.teamMgr.KickoutTeam(req);
		}

		
		public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
		{
			return this.teamMgr.LeaveTeam(req);
		}

		
		public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
		{
			return this.teamMgr.StartGame(req);
		}

		
		public KFCopyTeamSetReadyRsp TeamSetReady(KFCopyTeamSetReadyReq req)
		{
			return this.teamMgr.TeamSetReady(req);
		}

		
		public KFCopyTeamSetFlagRsp TeamSetFlag(KFCopyTeamSetFlagReq req)
		{
			return this.teamMgr.TeamSetFlag(req);
		}

		
		public CopyTeamData GetTeamData(long teamid)
		{
			return this.teamMgr.GetTeamData(teamid);
		}

		
		public void RemoveTeam(long teamId)
		{
			this.teamMgr.RemoveTeam(teamId);
		}

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == (int)this.GameType && clientInfo.ServerId != 0)
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

		
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		
		public static KuaFuCopyService Instance = null;

		
		public readonly GameTypes GameType = GameTypes.KuaFuCopy;

		
		private KFCopyTeamManager teamMgr = new KFCopyTeamManager();

		
		private long LastSaveServerStateMs = 0L;

		
		public KuaFuCopyDbMgr dbMgr = KuaFuCopyDbMgr.Instance;

		
		public Thread BackgroundThread;

		
		public Thread PlatChargeKingThread;

		
		private PlatChargeKingManager PCKMgr = new PlatChargeKingManager();
	}
}
