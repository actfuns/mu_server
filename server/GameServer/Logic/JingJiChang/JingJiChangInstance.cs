using System;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang
{
	
	public class JingJiChangInstance : ScheduleTask
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
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

		
		public void ResetJingJiTime()
		{
			this.startedTime = TimeUtil.NOW() + JingJiChangInstance.StartCDTime;
			this.stopCDTime = this.startedTime + JingJiChangInstance.CombatTime;
			this.stopedTime = this.stopCDTime + JingJiChangInstance.StopCDTime;
			this.destroyTime = this.stopedTime + JingJiChangInstance.DelayDestroyTime;
		}

		
		
		
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

		
		public JingJiFuBenState getState()
		{
			return this.state;
		}

		
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

		
		public int getFuBenSeqId()
		{
			return this.fubenSeqId;
		}

		
		public GameClient getPlayer()
		{
			return this.player;
		}

		
		public Robot getRobot()
		{
			return this.robot;
		}

		
		public void setRobot(Robot robot)
		{
			this.robot = robot;
		}

		
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

		
		public void release()
		{
			this.handle = null;
			this.player = null;
			this.robot = null;
		}

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		private int fubenSeqId;

		
		private PeriodicTaskHandle handle;

		
		public JingJiFuBenType type = JingJiFuBenType.NORMAL;

		
		private JingJiFuBenState state = JingJiFuBenState.INITIALIZED;

		
		private long createTime = 0L;

		
		private long startCDTime = 0L;

		
		private long startedTime = 0L;

		
		private long stopCDTime = 0L;

		
		private long stopedTime = 0L;

		
		private long destroyTime = 0L;

		
		private GameClient player = null;

		
		private Robot robot = null;

		
		private static readonly long DelayStart = 2000L;

		
		private static readonly long StartCDTime = 6000L;

		
		private static readonly long CombatTime = 165000L;

		
		private static readonly long StopCDTime = 10000L;

		
		private static readonly long DelayDestroyTime = 10000L;
	}
}
