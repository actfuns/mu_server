using System;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000760 RID: 1888
	public class MonsterTask : ScheduleTask
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x002AE83C File Offset: 0x002ACA3C
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x002AE854 File Offset: 0x002ACA54
		public MonsterTask(int mapCode, int subMapCode = -1)
		{
			this.mapCode = mapCode;
			this.subMapCode = subMapCode;
		}

		// Token: 0x06002FE7 RID: 12263 RVA: 0x002AE8B8 File Offset: 0x002ACAB8
		public void run()
		{
			try
			{
				long ticks = TimeUtil.NOW();
				if (!GameManager.IsKuaFuServer)
				{
					GameManager.ClientMgr.DoSpriteExtensionWorkByPerMap(this.mapCode, this.subMapCode);
				}
				long ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 1000L)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("DoSpriteExtensionWorkByPerMap, mapCode:{0}, subMapCode:{1}, 消耗:{2}毫秒", this.mapCode, this.subMapCode, ticks2 - ticks), null, true);
				}
				long startTicks = TimeUtil.NOW();
				ticks = TimeUtil.NOW();
				GameManager.MonsterMgr.DoMonsterHeartTimer(this.mapCode, this.subMapCode);
				ticks2 = TimeUtil.NOW();
				if (ticks2 > ticks + 800L)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("DoMonsterHeartTimer, mapCode:{0}, subMapCode:{1}, 消耗:{2}毫秒", this.mapCode, this.subMapCode, ticks2 - ticks), null, true);
				}
				this.heartbeatNum++;
				this.hearBeatTotalTime += ticks2 - ticks;
				ticks = TimeUtil.NOW();
				if (this.attackFrameCount % 5 == 0)
				{
					GameManager.MonsterMgr.DoMonsterAttack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 0, this.mapCode, this.subMapCode);
					this.attackNum++;
				}
				ticks2 = TimeUtil.NOW();
				this.attackTotalTime += ticks2 - ticks;
				if (++this.attackFrameCount > 1000000)
				{
					this.attackFrameCount = 0;
				}
				this.frameCount++;
				if (this.frameCount % 240 == 0)
				{
					long heartBeatCount = this.hearBeatTotalTime / (long)this.heartbeatNum;
					long attackCount = this.attackTotalTime / (long)this.attackNum;
					if (heartBeatCount > 32L)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("DoMonsterHeartTimer 平均耗时:{0}毫秒, MapID: {1}, SubMapCode: {2}", heartBeatCount, this.mapCode, this.subMapCode), null, true);
					}
					if (attackCount > 32L)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("DoMonsterAttack 平均耗时:{0}毫秒, MapID: {1}, SubMapCode: {2}", attackCount, this.mapCode, this.subMapCode), null, true);
					}
					this.hearBeatTotalTime = 0L;
					this.heartbeatNum = 0;
					this.attackTotalTime = 0L;
					this.attackNum = 0;
				}
				if (this.frameCount >= 2400000)
				{
					this.frameCount = 0;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "monsterHeartTimer_Tick", false, false);
			}
		}

		// Token: 0x04003CE7 RID: 15591
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x04003CE8 RID: 15592
		public int mapCode;

		// Token: 0x04003CE9 RID: 15593
		public int subMapCode = -1;

		// Token: 0x04003CEA RID: 15594
		private int attackFrameCount = 0;

		// Token: 0x04003CEB RID: 15595
		private int heartbeatNum = 0;

		// Token: 0x04003CEC RID: 15596
		private int attackNum = 0;

		// Token: 0x04003CED RID: 15597
		private long hearBeatTotalTime = 0L;

		// Token: 0x04003CEE RID: 15598
		private long attackTotalTime = 0L;

		// Token: 0x04003CEF RID: 15599
		private int frameCount = 0;
	}
}
