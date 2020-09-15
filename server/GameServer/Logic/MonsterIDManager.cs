using System;
using System.Collections.Generic;
using System.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200075C RID: 1884
	public class MonsterIDManager
	{
		// Token: 0x06002F7E RID: 12158 RVA: 0x002A7458 File Offset: 0x002A5658
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

		// Token: 0x06002F7F RID: 12159 RVA: 0x002A74F8 File Offset: 0x002A56F8
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

		// Token: 0x04003CDF RID: 15583
		private List<long> IdleIDList = new List<long>();

		// Token: 0x04003CE0 RID: 15584
		private long _MaxID = 2130771968L;
	}
}
