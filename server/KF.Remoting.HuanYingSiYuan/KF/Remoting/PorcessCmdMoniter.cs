using System;

namespace KF.Remoting
{
	
	public class PorcessCmdMoniter
	{
		
		public PorcessCmdMoniter(int cmd, long processTime)
		{
			this.cmd = cmd;
			this.processTotalTime = processTime;
			this.processMaxTime = processTime;
		}

		
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

		
		public long avgWaitProcessTime()
		{
			return (this.processNum > 0) ? (this.waitProcessTotalTime / (long)this.processNum) : 0L;
		}

		
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

		
		public void OnOutputData(long dataSize)
		{
			lock (this)
			{
				this.SendNum += 1L;
				this.OutPutBytes += dataSize;
			}
		}

		
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

		
		public long GetTotalBytes()
		{
			return this.TotalBytes;
		}

		
		public int cmd;

		
		public int processNum = 0;

		
		public long processTotalTime = 0L;

		
		public long processMaxTime = 0L;

		
		public long waitProcessTotalTime = 0L;

		
		public long maxWaitProcessTime = 0L;

		
		public long TotalBytes = 0L;

		
		public long SendNum = 0L;

		
		public long OutPutBytes = 0L;

		
		public long Num_Faild;

		
		public long Num_OK;

		
		public long Num_WithData;

		
		public string cmdName;
	}
}
