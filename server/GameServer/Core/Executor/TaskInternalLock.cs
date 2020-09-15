using System;
using System.Threading;

namespace GameServer.Core.Executor
{
	// Token: 0x02000109 RID: 265
	public class TaskInternalLock
	{
		// Token: 0x06000406 RID: 1030 RVA: 0x0003DFB8 File Offset: 0x0003C1B8
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

		// Token: 0x06000407 RID: 1031 RVA: 0x0003DFEC File Offset: 0x0003C1EC
		public bool Leave()
		{
			bool logRunTime = this.LogRunTime;
			Interlocked.CompareExchange(ref this._LockCount, 0, 1);
			this.LogRunTime = false;
			return logRunTime;
		}

		// Token: 0x04000596 RID: 1430
		private int _LockCount;

		// Token: 0x04000597 RID: 1431
		private bool LogRunTime;
	}
}
