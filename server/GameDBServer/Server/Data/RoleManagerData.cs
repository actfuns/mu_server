using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x0200016C RID: 364
	public class RoleManagerData
	{
		// Token: 0x04000891 RID: 2193
		public object Mutex = new object();

		// Token: 0x04000892 RID: 2194
		public int PurchaseOccupationNeedZuanShi = 1500;

		// Token: 0x04000893 RID: 2195
		public HashSet<int> CanChangeOccupationMapCodes = new HashSet<int>();

		// Token: 0x04000894 RID: 2196
		public HashSet<int> CanChangeOccupationGoodsTypes = new HashSet<int>();
	}
}
