using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class PetIDManager
	{
		
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

		
		public void PushID(long id)
		{
			lock (this.Mutex)
			{
				this.IDsQueue.Enqueue(id);
			}
		}

		
		private object Mutex = new object();

		
		private long BaseID = 2134900736L;

		
		private Queue<long> IDsQueue = new Queue<long>(1000);
	}
}
