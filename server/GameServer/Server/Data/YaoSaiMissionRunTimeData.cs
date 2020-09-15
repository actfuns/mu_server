using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x02000803 RID: 2051
	public class YaoSaiMissionRunTimeData
	{
		// Token: 0x040043E9 RID: 17385
		public object Mutex = new object();

		// Token: 0x040043EA RID: 17386
		public Dictionary<int, List<YaoSaiMissionData>> RoleMissionCacheDict = new Dictionary<int, List<YaoSaiMissionData>>();

		// Token: 0x040043EB RID: 17387
		public SortedList<long, RoleMissionData> MissionSortList = new SortedList<long, RoleMissionData>();
	}
}
