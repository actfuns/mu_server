using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x020002FE RID: 766
	public class JueXingRunData
	{
		// Token: 0x040013C7 RID: 5063
		public object Mutex = new object();

		// Token: 0x040013C8 RID: 5064
		public bool MoHuaOpen = false;

		// Token: 0x040013C9 RID: 5065
		public Dictionary<int, JueXingShiItem> JueXingShiDict = new Dictionary<int, JueXingShiItem>();

		// Token: 0x040013CA RID: 5066
		public Dictionary<int, TaoZhuang> TaoZhuangDict = new Dictionary<int, TaoZhuang>();

		// Token: 0x040013CB RID: 5067
		public Dictionary<int, AwakenLevelItem> AwakenLevelDict = new Dictionary<int, AwakenLevelItem>();

		// Token: 0x040013CC RID: 5068
		public Dictionary<int, int> AwakenRecoveryDict = new Dictionary<int, int>();

		// Token: 0x040013CD RID: 5069
		public int ExcellencePropLimit = 6;

		// Token: 0x040013CE RID: 5070
		public int SuitIDLimit = 11;
	}
}
