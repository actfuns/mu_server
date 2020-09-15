using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x02000853 RID: 2131
	public class ArrayAdditionItem
	{
		// Token: 0x04004694 RID: 18068
		public int Type;

		// Token: 0x04004695 RID: 18069
		public int NeedLevel;

		// Token: 0x04004696 RID: 18070
		public int NeedSuperiorNum;

		// Token: 0x04004697 RID: 18071
		public int NeedOrderNum;

		// Token: 0x04004698 RID: 18072
		public List<KeyValuePair<int, double>> AdditionProps;
	}
}
