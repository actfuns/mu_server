using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000774 RID: 1908
	public class PetIDManager
	{
		// Token: 0x06003102 RID: 12546 RVA: 0x002B6FA8 File Offset: 0x002B51A8
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

		// Token: 0x06003103 RID: 12547 RVA: 0x002B7030 File Offset: 0x002B5230
		public void PushID(long id)
		{
			lock (this.Mutex)
			{
				this.IDsQueue.Enqueue(id);
			}
		}

		// Token: 0x04003D85 RID: 15749
		private object Mutex = new object();

		// Token: 0x04003D86 RID: 15750
		private long BaseID = 2134900736L;

		// Token: 0x04003D87 RID: 15751
		private Queue<long> IDsQueue = new Queue<long>(1000);
	}
}
