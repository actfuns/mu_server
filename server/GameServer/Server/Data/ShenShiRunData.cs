using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x02000418 RID: 1048
	public class ShenShiRunData
	{
		// Token: 0x04001BEB RID: 7147
		public object Mutex = new object();

		// Token: 0x04001BEC RID: 7148
		public Dictionary<int, int> ParentMagicCode = new Dictionary<int, int>();

		// Token: 0x04001BED RID: 7149
		public Dictionary<int, FuWenHoleItem> FuWenHoleDict = new Dictionary<int, FuWenHoleItem>();

		// Token: 0x04001BEE RID: 7150
		public Dictionary<int, FuWenItem> FuWenDict = new Dictionary<int, FuWenItem>();

		// Token: 0x04001BEF RID: 7151
		public Dictionary<int, FuWenGodItem> FuWenGodDict = new Dictionary<int, FuWenGodItem>();

		// Token: 0x04001BF0 RID: 7152
		public List<FuWenRandomItem> FuWenRandomList = new List<FuWenRandomItem>();

		// Token: 0x04001BF1 RID: 7153
		public List<FuWenRandomItem> FuWenPayRandomList = new List<FuWenRandomItem>();

		// Token: 0x04001BF2 RID: 7154
		public List<FuWenRandomItem> HuoDongFuWenRandomList = new List<FuWenRandomItem>();

		// Token: 0x04001BF3 RID: 7155
		public List<FuWenRandomItem> HuoDongFuWenPayRandomList = new List<FuWenRandomItem>();
	}
}
