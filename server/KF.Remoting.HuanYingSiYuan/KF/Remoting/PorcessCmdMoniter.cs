using System;

namespace KF.Remoting
{
	// Token: 0x02000007 RID: 7
	public class PorcessCmdMoniter
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00004504 File Offset: 0x00002704
		public PorcessCmdMoniter(int cmd, long processTime)
		{
			this.cmd = cmd;
			this.processTotalTime = processTime;
			this.processMaxTime = processTime;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004570 File Offset: 0x00002770
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

		// Token: 0x06000040 RID: 64 RVA: 0x000045F0 File Offset: 0x000027F0
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

		// Token: 0x06000041 RID: 65 RVA: 0x0000468C File Offset: 0x0000288C
		public long avgWaitProcessTime()
		{
			return (this.processNum > 0) ? (this.waitProcessTotalTime / (long)this.processNum) : 0L;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000046BC File Offset: 0x000028BC
		public void onProcessNoWait(long processTime, long dataSize, TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_OK)
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

		// Token: 0x06000043 RID: 67 RVA: 0x00004788 File Offset: 0x00002988
		public void OnOutputData(long dataSize)
		{
			lock (this)
			{
				this.SendNum += 1L;
				this.OutPutBytes += dataSize;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000047E8 File Offset: 0x000029E8
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

		// Token: 0x06000045 RID: 69 RVA: 0x0000481C File Offset: 0x00002A1C
		public long GetTotalBytes()
		{
			return this.TotalBytes;
		}

		// Token: 0x04000025 RID: 37
		public int cmd;

		// Token: 0x04000026 RID: 38
		public int processNum = 0;

		// Token: 0x04000027 RID: 39
		public long processTotalTime = 0L;

		// Token: 0x04000028 RID: 40
		public long processMaxTime = 0L;

		// Token: 0x04000029 RID: 41
		public long waitProcessTotalTime = 0L;

		// Token: 0x0400002A RID: 42
		public long maxWaitProcessTime = 0L;

		// Token: 0x0400002B RID: 43
		public long TotalBytes = 0L;

		// Token: 0x0400002C RID: 44
		public long SendNum = 0L;

		// Token: 0x0400002D RID: 45
		public long OutPutBytes = 0L;

		// Token: 0x0400002E RID: 46
		public long Num_Faild;

		// Token: 0x0400002F RID: 47
		public long Num_OK;

		// Token: 0x04000030 RID: 48
		public long Num_WithData;

		// Token: 0x04000031 RID: 49
		public string cmdName;
	}
}
