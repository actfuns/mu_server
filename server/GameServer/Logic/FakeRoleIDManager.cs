using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020006BF RID: 1727
	public class FakeRoleIDManager
	{
		// Token: 0x0600207A RID: 8314 RVA: 0x001BF58C File Offset: 0x001BD78C
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

		// Token: 0x0600207B RID: 8315 RVA: 0x001BF614 File Offset: 0x001BD814
		public void PushID(long id)
		{
			lock (this.Mutex)
			{
				this.IDsQueue.Enqueue(id);
			}
		}

		// Token: 0x04003679 RID: 13945
		private object Mutex = new object();

		// Token: 0x0400367A RID: 13946
		private long BaseID = 2135097344L;

		// Token: 0x0400367B RID: 13947
		private Queue<long> IDsQueue = new Queue<long>(1000);
	}
}
