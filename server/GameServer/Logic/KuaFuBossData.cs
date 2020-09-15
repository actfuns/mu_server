using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000343 RID: 835
	public class KuaFuBossData
	{
		// Token: 0x040015F1 RID: 5617
		public object Mutex = new object();

		// Token: 0x040015F2 RID: 5618
		public Dictionary<int, KuaFuBossBirthPoint> MapBirthPointDict = new Dictionary<int, KuaFuBossBirthPoint>();

		// Token: 0x040015F3 RID: 5619
		public Dictionary<RangeKey, KuaFuBossSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, KuaFuBossSceneInfo>(RangeKey.Comparer);

		// Token: 0x040015F4 RID: 5620
		public Dictionary<int, KuaFuBossSceneInfo> SceneDataDict = new Dictionary<int, KuaFuBossSceneInfo>();

		// Token: 0x040015F5 RID: 5621
		public Dictionary<int, List<BattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<BattleDynamicMonsterItem>>();

		// Token: 0x040015F6 RID: 5622
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x040015F7 RID: 5623
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x040015F8 RID: 5624
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		// Token: 0x040015F9 RID: 5625
		public bool PrepareGame;

		// Token: 0x040015FA RID: 5626
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
