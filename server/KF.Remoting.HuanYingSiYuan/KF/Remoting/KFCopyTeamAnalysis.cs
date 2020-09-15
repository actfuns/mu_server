using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	// Token: 0x02000033 RID: 51
	public class KFCopyTeamAnalysis
	{
		// Token: 0x04000143 RID: 323
		public Dictionary<int, KFCopyTeamAnalysis.Item> AnalysisDict = new Dictionary<int, KFCopyTeamAnalysis.Item>();

		// Token: 0x02000034 RID: 52
		public class Item
		{
			// Token: 0x04000144 RID: 324
			public int TotalCopyCount;

			// Token: 0x04000145 RID: 325
			public int StartCopyCount;

			// Token: 0x04000146 RID: 326
			public int UnStartCopyCount;

			// Token: 0x04000147 RID: 327
			public int TotalRoleCount;

			// Token: 0x04000148 RID: 328
			public int StartRoleCount;

			// Token: 0x04000149 RID: 329
			public int UnStartRoleCount;
		}
	}
}
