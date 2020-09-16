using System;

namespace GameDBServer.Core
{
	
	public class PorcessCmdMoniter
	{
		
		public PorcessCmdMoniter(int cmd, long processTime)
		{
			this.cmd = cmd;
			this.processNum++;
			this.processTotalTime += processTime;
			this.processMaxTime = processTime;
		}

		
		public void onProcess(long processTime, long waitTime)
		{
			this.processNum++;
			this.processTotalTime += processTime;
			this.processMaxTime = ((this.processMaxTime >= processTime) ? this.processMaxTime : processTime);
			this.waitProcessTotalTime += waitTime;
			this.maxWaitProcessTime = ((this.maxWaitProcessTime >= waitTime) ? this.maxWaitProcessTime : waitTime);
		}

		
		public void onProcessNoWait(long processTime)
		{
			this.processNum++;
			this.processTotalTime += processTime;
			if (this.processMaxTime >= processTime)
			{
				this.processMaxTime = processTime;
			}
		}

		
		public long avgProcessTime()
		{
			return (this.processNum > 0) ? (this.processTotalTime / (long)this.processNum) : 0L;
		}

		
		public long avgWaitProcessTime()
		{
			return (this.processNum > 0) ? (this.waitProcessTotalTime / (long)this.processNum) : 0L;
		}

		
		public int cmd;

		
		public int processNum = 0;

		
		public long processTotalTime = 0L;

		
		public long processMaxTime = 0L;

		
		public long waitProcessTotalTime = 0L;

		
		public long maxWaitProcessTime = 0L;
	}
}
