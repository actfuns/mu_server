using System;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x0200072D RID: 1837
	public class JingJiChangInstance : ScheduleTask
	{
		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06002C98 RID: 11416 RVA: 0x0027C9EC File Offset: 0x0027ABEC
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06002C99 RID: 11417 RVA: 0x0027CA04 File Offset: 0x0027AC04
		public JingJiChangInstance(GameClient player, Robot robot, int fubenSeqId, JingJiFuBenType type = JingJiFuBenType.NORMAL)
		{
			this.type = type;
			this.state = JingJiFuBenState.INITIALIZED;
			this.fubenSeqId = fubenSeqId;
			this.player = player;
			this.robot = robot;
			this.createTime = TimeUtil.NOW();
			this.startCDTime = this.createTime + JingJiChangInstance.DelayStart;
			this.ResetJingJiTime();
		}

		// Token: 0x06002C9A RID: 11418 RVA: 0x0027CABC File Offset: 0x0027ACBC
		public void ResetJingJiTime()
		{
			this.startedTime = TimeUtil.NOW() + JingJiChangInstance.StartCDTime;
			this.stopCDTime = this.startedTime + JingJiChangInstance.CombatTime;
			this.stopedTime = this.stopCDTime + JingJiChangInstance.StopCDTime;
			this.destroyTime = this.stopedTime + JingJiChangInstance.DelayDestroyTime;
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06002C9B RID: 11419 RVA: 0x0027CB14 File Offset: 0x0027AD14
		// (set) Token: 0x06002C9C RID: 11420 RVA: 0x0027CB2C File Offset: 0x0027AD2C
		public PeriodicTaskHandle Handle
		{
			get
			{
				return this.handle;
			}
			set
			{
				this.handle = value;
			}
		}

		// Token: 0x06002C9D RID: 11421 RVA: 0x0027CB38 File Offset: 0x0027AD38
		public JingJiFuBenState getState()
		{
			return this.state;
		}

		// Token: 0x06002C9E RID: 11422 RVA: 0x0027CB50 File Offset: 0x0027AD50
		public void switchState(JingJiFuBenState state)
		{
			if (this.state != state)
			{
				this.state = state;
				switch (state)
				{
				case JingJiFuBenState.START_CD:
					JingJiChangManager.getInstance().onJingJiFuBenStartCD(this);
					break;
				case JingJiFuBenState.STARTED:
					JingJiChangManager.getInstance().onJingJiFuBenStarted(this);
					break;
				case JingJiFuBenState.STOP_CD:
					this.stopedTime = TimeUtil.NOW() + JingJiChangInstance.StopCDTime;
					this.destroyTime = this.stopedTime + JingJiChangInstance.DelayDestroyTime;
					break;
				case JingJiFuBenState.STOP_TIMEOUT_CD:
					JingJiChangManager.getInstance().onJingJiFuBenStopForTimeOutCD(this);
					this.switchState(JingJiFuBenState.STOP_CD);
					break;
				case JingJiFuBenState.STOPED:
					JingJiChangManager.getInstance().onJingJiFuBenStoped(this);
					this.destroyTime = TimeUtil.NOW() + JingJiChangInstance.DelayDestroyTime;
					break;
				case JingJiFuBenState.DESTROYED:
					JingJiChangManager.getInstance().onJingJiFuBenDestroy(this);
					break;
				}
			}
		}

		// Token: 0x06002C9F RID: 11423 RVA: 0x0027CC38 File Offset: 0x0027AE38
		public int getFuBenSeqId()
		{
			return this.fubenSeqId;
		}

		// Token: 0x06002CA0 RID: 11424 RVA: 0x0027CC50 File Offset: 0x0027AE50
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x06002CA1 RID: 11425 RVA: 0x0027CC68 File Offset: 0x0027AE68
		public Robot getRobot()
		{
			return this.robot;
		}

		// Token: 0x06002CA2 RID: 11426 RVA: 0x0027CC80 File Offset: 0x0027AE80
		public void setRobot(Robot robot)
		{
			this.robot = robot;
		}

		// Token: 0x06002CA3 RID: 11427 RVA: 0x0027CC8C File Offset: 0x0027AE8C
		public void run()
		{
			long now = TimeUtil.NOW();
			if (now > this.startCDTime && now < this.startedTime && this.state == JingJiFuBenState.WAITING_CHANGEMAP_FINISH)
			{
				this.switchState(JingJiFuBenState.START_CD);
			}
			else if (now > this.startedTime && now < this.stopCDTime && this.state == JingJiFuBenState.START_CD)
			{
				this.switchState(JingJiFuBenState.STARTED);
			}
			else if (now > this.stopCDTime && now < this.stopedTime && this.state == JingJiFuBenState.STARTED)
			{
				this.switchState(JingJiFuBenState.STOP_TIMEOUT_CD);
			}
			else if (now > this.stopedTime && now < this.destroyTime && this.state == JingJiFuBenState.STOP_CD)
			{
				this.switchState(JingJiFuBenState.STOPED);
			}
			else if (now > this.destroyTime && this.state == JingJiFuBenState.STOPED)
			{
				this.switchState(JingJiFuBenState.DESTROYED);
			}
			if (this.state != JingJiFuBenState.INITIALIZED && this.state != JingJiFuBenState.START_CD && this.state != JingJiFuBenState.DESTROYED && this.state != JingJiFuBenState.STOPED)
			{
				if (null != this.robot)
				{
					this.robot.onUpdate();
				}
			}
		}

		// Token: 0x06002CA4 RID: 11428 RVA: 0x0027CDE3 File Offset: 0x0027AFE3
		public void release()
		{
			this.handle = null;
			this.player = null;
			this.robot = null;
		}

		// Token: 0x04003B30 RID: 15152
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x04003B31 RID: 15153
		private int fubenSeqId;

		// Token: 0x04003B32 RID: 15154
		private PeriodicTaskHandle handle;

		// Token: 0x04003B33 RID: 15155
		public JingJiFuBenType type = JingJiFuBenType.NORMAL;

		// Token: 0x04003B34 RID: 15156
		private JingJiFuBenState state = JingJiFuBenState.INITIALIZED;

		// Token: 0x04003B35 RID: 15157
		private long createTime = 0L;

		// Token: 0x04003B36 RID: 15158
		private long startCDTime = 0L;

		// Token: 0x04003B37 RID: 15159
		private long startedTime = 0L;

		// Token: 0x04003B38 RID: 15160
		private long stopCDTime = 0L;

		// Token: 0x04003B39 RID: 15161
		private long stopedTime = 0L;

		// Token: 0x04003B3A RID: 15162
		private long destroyTime = 0L;

		// Token: 0x04003B3B RID: 15163
		private GameClient player = null;

		// Token: 0x04003B3C RID: 15164
		private Robot robot = null;

		// Token: 0x04003B3D RID: 15165
		private static readonly long DelayStart = 2000L;

		// Token: 0x04003B3E RID: 15166
		private static readonly long StartCDTime = 6000L;

		// Token: 0x04003B3F RID: 15167
		private static readonly long CombatTime = 165000L;

		// Token: 0x04003B40 RID: 15168
		private static readonly long StopCDTime = 10000L;

		// Token: 0x04003B41 RID: 15169
		private static readonly long DelayDestroyTime = 10000L;
	}
}
