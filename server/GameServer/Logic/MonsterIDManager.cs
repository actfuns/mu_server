using System;
using System.Collections.Generic;
using System.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MonsterIDManager
	{
		
		public long GetNewID(int mapCode)
		{
			long id = 2130771968L;
			lock (this.IdleIDList)
			{
				if (this.IdleIDList.Count > 0)
				{
					id = this.IdleIDList.ElementAt(0);
					this.IdleIDList.RemoveAt(0);
				}
				else
				{
					id = (this._MaxID += 1L);
				}
			}
			return id;
		}

		
		public void PushBack(long id)
		{
			lock (this.IdleIDList)
			{
				if (this.IdleIDList.IndexOf(id) < 0 && this.IdleIDList.Count < 10000)
				{
					this.IdleIDList.Add(id);
				}
			}
			if (this.IdleIDList.Count > 10000)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("MonsterIDManager中的空闲ID数达到了{0}", this.IdleIDList.Count), null, true);
			}
		}

		
		private List<long> IdleIDList = new List<long>();

		
		private long _MaxID = 2130771968L;
	}
}
