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
	// Token: 0x02000038 RID: 56
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class KuaFuCopyService : MarshalByRefObject, IKuaFuCopyService
	{
		// Token: 0x06000280 RID: 640 RVA: 0x00025980 File Offset: 0x00023B80
		public InputKingPaiHangDataEx GetPlatChargeKing()
		{
			return this.PCKMgr.GetRankEx();
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000259A0 File Offset: 0x00023BA0
		public List<InputKingPaiHangDataEx> GetPlatChargeKingEveryDay(DateTime FromDate, DateTime ToDate)
		{
			return this.PCKMgr.GetRankExList(FromDate, ToDate);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000259C0 File Offset: 0x00023BC0
		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			return this.PCKMgr.QueryHuodongAwardUserHist(actType, huoDongKeyStr, userid);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000259E0 File Offset: 0x00023BE0
		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			return this.PCKMgr.UpdateHuodongAwardUserHist(actType, huoDongKeyStr, userid, extTag);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00025A04 File Offset: 0x00023C04
		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			return SpecPriorityActivityMgr.Instance().SpecPriority_ModifyActivityConditionNum(key, add);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00025A24 File Offset: 0x00023C24
		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			return SpecPriorityActivityMgr.Instance().SpecPriority_GetActivityConditionInfo();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00025A40 File Offset: 0x00023C40
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

		// Token: 0x06000287 RID: 647 RVA: 0x00025AE0 File Offset: 0x00023CE0
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

		// Token: 0x06000288 RID: 648 RVA: 0x00025BA4 File Offset: 0x00023DA4
		~KuaFuCopyService()
		{
			this.BackgroundThread.Abort();
			this.PlatChargeKingThread.Abort();
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00025BE8 File Offset: 0x00023DE8
		public void RemoveGameTeam(int kfSrvId, long teamId)
		{
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, kfSrvId, teamId);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00025C00 File Offset: 0x00023E00
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

		// Token: 0x0600028B RID: 651 RVA: 0x00025D34 File Offset: 0x00023F34
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00025D58 File Offset: 0x00023F58
		public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
		{
			return this.teamMgr.CreateTeam(req);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00025D78 File Offset: 0x00023F78
		public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
		{
			return this.teamMgr.JoinTeam(req);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00025D98 File Offset: 0x00023F98
		public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
		{
			return this.teamMgr.KickoutTeam(req);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00025DB8 File Offset: 0x00023FB8
		public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
		{
			return this.teamMgr.LeaveTeam(req);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00025DD8 File Offset: 0x00023FD8
		public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
		{
			return this.teamMgr.StartGame(req);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00025DF8 File Offset: 0x00023FF8
		public KFCopyTeamSetReadyRsp TeamSetReady(KFCopyTeamSetReadyReq req)
		{
			return this.teamMgr.TeamSetReady(req);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00025E18 File Offset: 0x00024018
		public KFCopyTeamSetFlagRsp TeamSetFlag(KFCopyTeamSetFlagReq req)
		{
			return this.teamMgr.TeamSetFlag(req);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00025E38 File Offset: 0x00024038
		public CopyTeamData GetTeamData(long teamid)
		{
			return this.teamMgr.GetTeamData(teamid);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00025E56 File Offset: 0x00024056
		public void RemoveTeam(long teamId)
		{
			this.teamMgr.RemoveTeam(teamId);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00025E68 File Offset: 0x00024068
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

		// Token: 0x06000296 RID: 662 RVA: 0x00025F1C File Offset: 0x0002411C
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		// Token: 0x04000160 RID: 352
		public static KuaFuCopyService Instance = null;

		// Token: 0x04000161 RID: 353
		public readonly GameTypes GameType = GameTypes.KuaFuCopy;

		// Token: 0x04000162 RID: 354
		private KFCopyTeamManager teamMgr = new KFCopyTeamManager();

		// Token: 0x04000163 RID: 355
		private long LastSaveServerStateMs = 0L;

		// Token: 0x04000164 RID: 356
		public KuaFuCopyDbMgr dbMgr = KuaFuCopyDbMgr.Instance;

		// Token: 0x04000165 RID: 357
		public Thread BackgroundThread;

		// Token: 0x04000166 RID: 358
		public Thread PlatChargeKingThread;

		// Token: 0x04000167 RID: 359
		private PlatChargeKingManager PCKMgr = new PlatChargeKingManager();
	}
}
