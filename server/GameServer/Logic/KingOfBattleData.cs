using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000336 RID: 822
	public class KingOfBattleData
	{
		// Token: 0x0400156C RID: 5484
		public object Mutex = new object();

		// Token: 0x0400156D RID: 5485
		public Dictionary<int, YongZheZhanChangBirthPoint> MapBirthPointDict = new Dictionary<int, YongZheZhanChangBirthPoint>();

		// Token: 0x0400156E RID: 5486
		public Dictionary<RangeKey, KingOfBattleSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, KingOfBattleSceneInfo>(RangeKey.Comparer);

		// Token: 0x0400156F RID: 5487
		public Dictionary<int, KingOfBattleSceneInfo> SceneDataDict = new Dictionary<int, KingOfBattleSceneInfo>();

		// Token: 0x04001570 RID: 5488
		public Dictionary<int, KingOfBattleStoreConfig> KingOfBattleStoreDict = new Dictionary<int, KingOfBattleStoreConfig>();

		// Token: 0x04001571 RID: 5489
		public List<KingOfBattleStoreConfig> KingOfBattleStoreList = new List<KingOfBattleStoreConfig>();

		// Token: 0x04001572 RID: 5490
		public Dictionary<int, BattleCrystalMonsterItem> BattleCrystalMonsterDict = new Dictionary<int, BattleCrystalMonsterItem>();

		// Token: 0x04001573 RID: 5491
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04001574 RID: 5492
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04001575 RID: 5493
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		// Token: 0x04001576 RID: 5494
		public Dictionary<int, List<KingOfBattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<KingOfBattleDynamicMonsterItem>>();

		// Token: 0x04001577 RID: 5495
		public Dictionary<int, KingOfBattleDynamicMonsterItem> DynMonsterDict = new Dictionary<int, KingOfBattleDynamicMonsterItem>();

		// Token: 0x04001578 RID: 5496
		public Dictionary<int, KingOfBattleQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, KingOfBattleQiZhiConfig>();

		// Token: 0x04001579 RID: 5497
		public int BattleQiZhiMonsterID1 = 8800003;

		// Token: 0x0400157A RID: 5498
		public int BattleQiZhiMonsterID2 = 8800004;

		// Token: 0x0400157B RID: 5499
		public int KingOfBattleDamageJunQi = 1;

		// Token: 0x0400157C RID: 5500
		public int KingOfBattleDamageTower = 1;

		// Token: 0x0400157D RID: 5501
		public int KingOfBattleDamageCenter = 1;

		// Token: 0x0400157E RID: 5502
		public int KingOfBattleDie = 5;

		// Token: 0x0400157F RID: 5503
		public int KingOfBattleLowestJiFen = 5;

		// Token: 0x04001580 RID: 5504
		public double KingBattleBossAttackPercent = 0.001;

		// Token: 0x04001581 RID: 5505
		public int KingOfBattleUltraKillParam1 = 27;

		// Token: 0x04001582 RID: 5506
		public int KingOfBattleUltraKillParam2 = 3;

		// Token: 0x04001583 RID: 5507
		public int KingOfBattleUltraKillParam3 = 30;

		// Token: 0x04001584 RID: 5508
		public int KingOfBattleUltraKillParam4 = 75;

		// Token: 0x04001585 RID: 5509
		public int KingOfBattleShutDownParam1 = -10;

		// Token: 0x04001586 RID: 5510
		public int KingOfBattleShutDownParam2 = 5;

		// Token: 0x04001587 RID: 5511
		public int KingOfBattleShutDownParam3 = 0;

		// Token: 0x04001588 RID: 5512
		public int KingOfBattleShutDownParam4 = 100;

		// Token: 0x04001589 RID: 5513
		public int KingOfBattleStoreRefreshTm = 24;

		// Token: 0x0400158A RID: 5514
		public int KingOfBattleStoreRefreshNum = 6;

		// Token: 0x0400158B RID: 5515
		public int KingOfBattleStoreRefreshCost = 100;

		// Token: 0x0400158C RID: 5516
		public int SighUpStateMagicNum = 100000000;

		// Token: 0x0400158D RID: 5517
		public string RoleParamsAwardsDefaultString = "";

		// Token: 0x0400158E RID: 5518
		public bool PrepareGame;

		// Token: 0x0400158F RID: 5519
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
