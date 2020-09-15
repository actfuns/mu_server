using System;
using System.Collections.Generic;
using GameServer.Logic;
using KF.Contract.Data;

namespace Server.Data
{
	// Token: 0x02000311 RID: 785
	public class LingDiCaiJiRunData
	{
		// Token: 0x0400141C RID: 5148
		public object Mutex = new object();

		// Token: 0x0400141D RID: 5149
		public bool KuaFuSyncNeed = true;

		// Token: 0x0400141E RID: 5150
		public List<LingDiData> LingDiDataList = new List<LingDiData>();

		// Token: 0x0400141F RID: 5151
		public Dictionary<int, SortedList<long, List<object>>> NormalShuiJingQueue = new Dictionary<int, SortedList<long, List<object>>>();

		// Token: 0x04001420 RID: 5152
		public Dictionary<int, SortedList<long, List<object>>> ChaoShuiJingQueue = new Dictionary<int, SortedList<long, List<object>>>();

		// Token: 0x04001421 RID: 5153
		public Dictionary<int, List<LingDiShouWeiMonsterItem>> ShouWeiQueue = new Dictionary<int, List<LingDiShouWeiMonsterItem>>();

		// Token: 0x04001422 RID: 5154
		public List<RoleData4Selector> LingZhuRoleDataList = new List<RoleData4Selector>(2);

		// Token: 0x04001423 RID: 5155
		public Dictionary<int, Dictionary<int, Monster>> ShouWeiMonster = new Dictionary<int, Dictionary<int, Monster>>();

		// Token: 0x04001424 RID: 5156
		public List<bool> DoubleOpenState = new List<bool>();
	}
}
