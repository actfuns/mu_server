using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000843 RID: 2115
	public class ZorkBattleData
	{
		// Token: 0x0400460E RID: 17934
		public object Mutex = new object();

		// Token: 0x0400460F RID: 17935
		public Dictionary<int, ZorkBattleBirthPoint> MapBirthPointDict = new Dictionary<int, ZorkBattleBirthPoint>();

		// Token: 0x04004610 RID: 17936
		public Dictionary<int, ZorkBattleSceneInfo> SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();

		// Token: 0x04004611 RID: 17937
		public List<ZorkBattleArmyConfig> ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();

		// Token: 0x04004612 RID: 17938
		public Dictionary<int, List<ZorkBattleMonsterConfig>> ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();

		// Token: 0x04004613 RID: 17939
		public Dictionary<int, ZorkAchievementConfig> ZorkAchievementDict = new Dictionary<int, ZorkAchievementConfig>();

		// Token: 0x04004614 RID: 17940
		public List<ZorkBattleAwardConfig> ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();

		// Token: 0x04004615 RID: 17941
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04004616 RID: 17942
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04004617 RID: 17943
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		// Token: 0x04004618 RID: 17944
		public int BossHurtCleanTime;

		// Token: 0x04004619 RID: 17945
		public HashSet<int> ZorkWarEnterMapSet = new HashSet<int>();

		// Token: 0x0400461A RID: 17946
		public int ZorkBattleUltraKillParam1 = 27;

		// Token: 0x0400461B RID: 17947
		public int ZorkBattleUltraKillParam2 = 3;

		// Token: 0x0400461C RID: 17948
		public int ZorkBattleUltraKillParam3 = 30;

		// Token: 0x0400461D RID: 17949
		public int ZorkBattleUltraKillParam4 = 75;

		// Token: 0x0400461E RID: 17950
		public int ZorkBattleShutDownParam1 = -10;

		// Token: 0x0400461F RID: 17951
		public int ZorkBattleShutDownParam2 = 5;

		// Token: 0x04004620 RID: 17952
		public int ZorkBattleShutDownParam3 = 0;

		// Token: 0x04004621 RID: 17953
		public int ZorkBattleShutDownParam4 = 100;

		// Token: 0x04004622 RID: 17954
		public int ZorkEnterPlayNumMin = 4;

		// Token: 0x04004623 RID: 17955
		public DateTime ZorkStartTime;

		// Token: 0x04004624 RID: 17956
		public Dictionary<int, KuaFu5v5FuBenData> FuBenItemData = new Dictionary<int, KuaFu5v5FuBenData>();
	}
}
