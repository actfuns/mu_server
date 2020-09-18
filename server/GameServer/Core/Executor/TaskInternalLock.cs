using System;
using System.Threading;

namespace GameServer.Core.Executor
{
	
	public class TaskInternalLock
	{
		
		public bool TryEnter()
		{
			bool result;
			if (Interlocked.CompareExchange(ref this._LockCount, 1, 0) != 0)
			{
				this.LogRunTime = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public bool Leave()
		{
			bool logRunTime = this.LogRunTime;
			Interlocked.CompareExchange(ref this._LockCount, 0, 1);
			this.LogRunTime = false;
			return logRunTime;
		}

		
		private int _LockCount;

		
		private bool LogRunTime;
	}
}
