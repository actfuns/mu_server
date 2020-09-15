using System;
using System.Collections.Generic;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200003A RID: 58
	public class EverydayActivityGroupConfig
	{
		// Token: 0x04000136 RID: 310
		public int GroupID = 0;

		// Token: 0x04000137 RID: 311
		public int TypeID = 0;

		// Token: 0x04000138 RID: 312
		public int NeedType = 0;

		// Token: 0x04000139 RID: 313
		public EveryActLimitData NeedNum = new EveryActLimitData();

		// Token: 0x0400013A RID: 314
		public List<int> ActivityIDList = new List<int>();
	}
}
