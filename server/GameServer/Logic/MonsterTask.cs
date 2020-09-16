using System;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MonsterTask : ScheduleTask
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public MonsterTask(int mapCode, int subMapCode = -1)
		{
			this.mapCode = mapCode;
			this.subMapCode = subMapCode;
		}

		
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

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		public int mapCode;

		
		public int subMapCode = -1;

		
		private int attackFrameCount = 0;

		
		private int heartbeatNum = 0;

		
		private int attackNum = 0;

		
		private long hearBeatTotalTime = 0L;

		
		private long attackTotalTime = 0L;

		
		private int frameCount = 0;
	}
}
