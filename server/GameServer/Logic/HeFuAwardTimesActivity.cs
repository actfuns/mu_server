using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000718 RID: 1816
	public class HeFuAwardTimesActivity : Activity
	{
		// Token: 0x06002B6A RID: 11114 RVA: 0x00268240 File Offset: 0x00266440
		public bool InActivityList(int value)
		{
			return this.activityList.Contains(value);
		}

		// Token: 0x04003A55 RID: 14933
		public List<int> activityList = new List<int>();

		// Token: 0x04003A56 RID: 14934
		public float activityTimes;

		// Token: 0x04003A57 RID: 14935
		public int specialTimeID;
	}
}
