using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000738 RID: 1848
	public class JunQiIDManager
	{
		// Token: 0x06002E07 RID: 11783 RVA: 0x002864BC File Offset: 0x002846BC
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

		// Token: 0x06002E08 RID: 11784 RVA: 0x00286544 File Offset: 0x00284744
		public void PushID(long id)
		{
			lock (this.Mutex)
			{
				this.IDsQueue.Enqueue(id);
			}
		}

		// Token: 0x04003C1F RID: 15391
		private object Mutex = new object();

		// Token: 0x04003C20 RID: 15392
		private long BaseID = 2135031808L;

		// Token: 0x04003C21 RID: 15393
		private Queue<long> IDsQueue = new Queue<long>(1000);
	}
}
