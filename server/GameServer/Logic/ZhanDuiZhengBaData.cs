using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000827 RID: 2087
	public class ZhanDuiZhengBaData
	{
		// Token: 0x04004539 RID: 17721
		public object Mutex = new object();

		// Token: 0x0400453A RID: 17722
		public int TotalSecs = 1860;

		// Token: 0x0400453B RID: 17723
		public List<ZhanDuiZhengBaAwardsConfig> AwardsConfig = new List<ZhanDuiZhengBaAwardsConfig>();

		// Token: 0x0400453C RID: 17724
		public ZhanDuiZhengBaConfig Config = new ZhanDuiZhengBaConfig();

		// Token: 0x0400453D RID: 17725
		public int[] TeamBattleWatch = new int[2];

		// Token: 0x0400453E RID: 17726
		public int[] TeamBattleName = new int[2];

		// Token: 0x0400453F RID: 17727
		public RunByTime SyncDataByTime = new RunByTime(15000L);

		// Token: 0x04004540 RID: 17728
		public ZhanDuiZhengBaSyncData SyncDataRequest = new ZhanDuiZhengBaSyncData();

		// Token: 0x04004541 RID: 17729
		public ZhanDuiZhengBaSyncData SyncData = new ZhanDuiZhengBaSyncData();

		// Token: 0x04004542 RID: 17730
		public TimeSpan StartTime;

		// Token: 0x04004543 RID: 17731
		public Dictionary<long, ZhanDuiZhengBaFuBenData> KuaFuCopyDataDict = new Dictionary<long, ZhanDuiZhengBaFuBenData>();

		// Token: 0x04004544 RID: 17732
		public Queue<Tuple<int, int, int>> PKResultQueue = new Queue<Tuple<int, int, int>>();
	}
}
