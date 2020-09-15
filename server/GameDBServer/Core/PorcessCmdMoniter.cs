using System;

namespace GameDBServer.Core
{
	// Token: 0x02000025 RID: 37
	public class PorcessCmdMoniter
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00004D20 File Offset: 0x00002F20
		public PorcessCmdMoniter(int cmd, long processTime)
		{
			this.cmd = cmd;
			this.processNum++;
			this.processTotalTime += processTime;
			this.processMaxTime = processTime;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004D88 File Offset: 0x00002F88
		public void onProcess(long processTime, long waitTime)
		{
			this.processNum++;
			this.processTotalTime += processTime;
			this.processMaxTime = ((this.processMaxTime >= processTime) ? this.processMaxTime : processTime);
			this.waitProcessTotalTime += waitTime;
			this.maxWaitProcessTime = ((this.maxWaitProcessTime >= waitTime) ? this.maxWaitProcessTime : waitTime);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004DF4 File Offset: 0x00002FF4
		public void onProcessNoWait(long processTime)
		{
			this.processNum++;
			this.processTotalTime += processTime;
			if (this.processMaxTime >= processTime)
			{
				this.processMaxTime = processTime;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004E34 File Offset: 0x00003034
		public long avgProcessTime()
		{
			return (this.processNum > 0) ? (this.processTotalTime / (long)this.processNum) : 0L;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004E64 File Offset: 0x00003064
		public long avgWaitProcessTime()
		{
			return (this.processNum > 0) ? (this.waitProcessTotalTime / (long)this.processNum) : 0L;
		}

		// Token: 0x04000066 RID: 102
		public int cmd;

		// Token: 0x04000067 RID: 103
		public int processNum = 0;

		// Token: 0x04000068 RID: 104
		public long processTotalTime = 0L;

		// Token: 0x04000069 RID: 105
		public long processMaxTime = 0L;

		// Token: 0x0400006A RID: 106
		public long waitProcessTotalTime = 0L;

		// Token: 0x0400006B RID: 107
		public long maxWaitProcessTime = 0L;
	}
}
