using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	// Token: 0x02000020 RID: 32
	[Serializable]
	public class TianTiRankData
	{
		// Token: 0x040000A0 RID: 160
		public DateTime ModifyTime;

		// Token: 0x040000A1 RID: 161
		public int MaxPaiMingRank;

		// Token: 0x040000A2 RID: 162
		public List<TianTiRoleInfoData> TianTiRoleInfoDataList;

		// Token: 0x040000A3 RID: 163
		public List<TianTiRoleInfoData> TianTiMonthRoleInfoDataList;
	}
}
