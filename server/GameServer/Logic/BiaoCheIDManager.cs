using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005C8 RID: 1480
	public class BiaoCheIDManager
	{
		// Token: 0x06001B1A RID: 6938 RVA: 0x0019B9BC File Offset: 0x00199BBC
		public long GetNewID()
		{
			long result;
			lock (this.Mutex)
			{
				if (this.IDsQueue.Count > 0)
				{
					result = this.IDsQueue.Dequeue();
				}
				else
				{
					long id = this.BaseID;
					this.BaseID += 1L;
					result = id;
				}
			}
			return result;
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x0019BA44 File Offset: 0x00199C44
		public void PushID(long id)
		{
			lock (this.Mutex)
			{
				this.IDsQueue.Enqueue(id);
			}
		}

		// Token: 0x040029CB RID: 10699
		private object Mutex = new object();

		// Token: 0x040029CC RID: 10700
		private long BaseID = 2134966272L;

		// Token: 0x040029CD RID: 10701
		private Queue<long> IDsQueue = new Queue<long>(1000);
	}
}
