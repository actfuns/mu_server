using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020003FC RID: 1020
	public class RoleManagerData
	{
		// Token: 0x04001B35 RID: 6965
		public object Mutex = new object();

		// Token: 0x04001B36 RID: 6966
		public int[] PurchaseOccupationNeedZuanShi;

		// Token: 0x04001B37 RID: 6967
		public int PurchaseOccupationGoods = 2100;

		// Token: 0x04001B38 RID: 6968
		public HashSet<int> CanChangeOccupationMapCodes = new HashSet<int>();

		// Token: 0x04001B39 RID: 6969
		public HashSet<int> DisableChangeOccupationGoodsTypes = new HashSet<int>();
	}
}
