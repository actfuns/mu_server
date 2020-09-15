using System;
using GameServer.Server;

namespace GameServer.Logic
{
	// Token: 0x02000779 RID: 1913
	public class PorcessCmdMoniter
	{
		// Token: 0x06003110 RID: 12560 RVA: 0x002B7644 File Offset: 0x002B5844
		public PorcessCmdMoniter(int cmd, long processTime)
		{
			this.cmd = cmd;
			this.processMaxTime = processTime;
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x002B76A8 File Offset: 0x002B58A8
		public void Reset()
		{
			lock (this)
			{
				this.maxWaitProcessTime = 0L;
				this.processMaxTime = 0L;
				this.processNum = 0;
				this.processTotalTime = 0L;
				this.waitProcessTotalTime = 0L;
				this.TotalBytes = 0L;
				this.SendNum = 0L;
				this.OutPutBytes = 0L;
			}
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x002B7728 File Offset: 0x002B5928
		public void onProcess(long processTime, long waitTime)
		{
			lock (this)
			{
				this.processNum++;
				this.processTotalTime += processTime;
				this.processMaxTime = ((this.processMaxTime >= processTime) ? this.processMaxTime : processTime);
				this.waitProcessTotalTime += waitTime;
				this.maxWaitProcessTime = ((this.maxWaitProcessTime >= waitTime) ? this.maxWaitProcessTime : waitTime);
			}
		}

		// Token: 0x06003113 RID: 12563 RVA: 0x002B77C4 File Offset: 0x002B59C4
		public long avgWaitProcessTime()
		{
			return (this.processNum > 0) ? (this.waitProcessTotalTime / (long)this.processNum) : 0L;
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x002B77F4 File Offset: 0x002B59F4
		public void onProcessNoWait(long processTime, long dataSize, TCPProcessCmdResults result)
		{
			lock (this)
			{
				this.processNum++;
				this.processTotalTime += processTime;
				this.TotalBytes += dataSize;
				if (this.processMaxTime >= processTime)
				{
					this.processMaxTime = processTime;
				}
				switch (result)
				{
				case TCPProcessCmdResults.RESULT_OK:
					this.Num_OK += 1L;
					break;
				case TCPProcessCmdResults.RESULT_FAILED:
					this.Num_Faild += 1L;
					break;
				case TCPProcessCmdResults.RESULT_DATA:
					this.Num_WithData += 1L;
					break;
				}
			}
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x002B78C0 File Offset: 0x002B5AC0
		public void OnOutputData(long dataSize)
		{
			lock (this)
			{
				this.SendNum += 1L;
				this.OutPutBytes += dataSize;
			}
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x002B7920 File Offset: 0x002B5B20
		public long avgProcessTime()
		{
			int num = this.processNum;
			long result;
			if (num > 0)
			{
				result = this.processTotalTime / (long)num;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x002B7954 File Offset: 0x002B5B54
		public long GetTotalBytes()
		{
			return this.TotalBytes;
		}

		// Token: 0x04003D90 RID: 15760
		public int cmd;

		// Token: 0x04003D91 RID: 15761
		public int processNum = 0;

		// Token: 0x04003D92 RID: 15762
		public long processTotalTime = 0L;

		// Token: 0x04003D93 RID: 15763
		public long processMaxTime = 0L;

		// Token: 0x04003D94 RID: 15764
		public long waitProcessTotalTime = 0L;

		// Token: 0x04003D95 RID: 15765
		public long maxWaitProcessTime = 0L;

		// Token: 0x04003D96 RID: 15766
		public long TotalBytes = 0L;

		// Token: 0x04003D97 RID: 15767
		public long SendNum = 0L;

		// Token: 0x04003D98 RID: 15768
		public long OutPutBytes = 0L;

		// Token: 0x04003D99 RID: 15769
		public long Num_Faild;

		// Token: 0x04003D9A RID: 15770
		public long Num_OK;

		// Token: 0x04003D9B RID: 15771
		public long Num_WithData;
	}
}
