using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	// Token: 0x020000BC RID: 188
	[Serializable]
	public class TianTi5v5RankData
	{
		// Token: 0x04000505 RID: 1285
		public DateTime ModifyTime;

		// Token: 0x04000506 RID: 1286
		public int MaxPaiMingRank;

		// Token: 0x04000507 RID: 1287
		public List<TianTi5v5ZhanDuiData> TianTi5v5RoleInfoDataList;

		// Token: 0x04000508 RID: 1288
		public List<TianTi5v5ZhanDuiData> TianTi5v5MonthRoleInfoDataList;
	}
}
