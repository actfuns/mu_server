using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000287 RID: 647
	public class CopyWolfWaveInfo
	{
		// Token: 0x04001006 RID: 4102
		public int WaveID = 0;

		// Token: 0x04001007 RID: 4103
		public int NextTime = 0;

		// Token: 0x04001008 RID: 4104
		public List<int[]> MonsterList = new List<int[]>();

		// Token: 0x04001009 RID: 4105
		public List<CopyWolfSiteInfo> MonsterSiteDic = new List<CopyWolfSiteInfo>();
	}
}
