using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x02000191 RID: 401
	public class TalentInfo
	{
		// Token: 0x040008D7 RID: 2263
		public int ID = 0;

		// Token: 0x040008D8 RID: 2264
		public int Type = 0;

		// Token: 0x040008D9 RID: 2265
		public string Name = "";

		// Token: 0x040008DA RID: 2266
		public int NeedTalentCount = 0;

		// Token: 0x040008DB RID: 2267
		public int NeedTalentID = 0;

		// Token: 0x040008DC RID: 2268
		public int NeedTalentLevel = 0;

		// Token: 0x040008DD RID: 2269
		public int LevelMax = 0;

		// Token: 0x040008DE RID: 2270
		public int EffectType = 0;

		// Token: 0x040008DF RID: 2271
		public Dictionary<int, List<TalentEffectInfo>> EffectList = null;
	}
}
