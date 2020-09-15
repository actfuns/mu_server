using System;
using GameDBServer.DB;

namespace GameDBServer.Data
{
	// Token: 0x02000027 RID: 39
	internal class AccountActiveData
	{
		// Token: 0x0400008D RID: 141
		[DBMapping(ColumnName = "Account")]
		public string strAccount;

		// Token: 0x0400008E RID: 142
		[DBMapping(ColumnName = "createTime")]
		public string strCreateTime;

		// Token: 0x0400008F RID: 143
		[DBMapping(ColumnName = "seriesLoginCount")]
		public int nSeriesLoginCount;

		// Token: 0x04000090 RID: 144
		[DBMapping(ColumnName = "lastSeriesLoginTime")]
		public string strLastSeriesLoginTime;
	}
}
