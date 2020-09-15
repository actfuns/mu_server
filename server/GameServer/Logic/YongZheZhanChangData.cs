using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000818 RID: 2072
	public class YongZheZhanChangData
	{
		// Token: 0x04004490 RID: 17552
		public object Mutex = new object();

		// Token: 0x04004491 RID: 17553
		public Dictionary<int, YongZheZhanChangBirthPoint> MapBirthPointDict = new Dictionary<int, YongZheZhanChangBirthPoint>();

		// Token: 0x04004492 RID: 17554
		public Dictionary<RangeKey, YongZheZhanChangSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, YongZheZhanChangSceneInfo>(RangeKey.Comparer);

		// Token: 0x04004493 RID: 17555
		public Dictionary<int, YongZheZhanChangSceneInfo> SceneDataDict = new Dictionary<int, YongZheZhanChangSceneInfo>();

		// Token: 0x04004494 RID: 17556
		public Dictionary<int, BattleCrystalMonsterItem> BattleCrystalMonsterDict = new Dictionary<int, BattleCrystalMonsterItem>();

		// Token: 0x04004495 RID: 17557
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04004496 RID: 17558
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		// Token: 0x04004497 RID: 17559
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		// Token: 0x04004498 RID: 17560
		public Dictionary<int, List<BattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<BattleDynamicMonsterItem>>();

		// Token: 0x04004499 RID: 17561
		public double WarriorBattleBossAttackPercent = 0.001;

		// Token: 0x0400449A RID: 17562
		public int WarriorBattleBossAttackScore = 20;

		// Token: 0x0400449B RID: 17563
		public int WarriorBattleBOssLastAttack = 20;

		// Token: 0x0400449C RID: 17564
		public int WarriorBattlePk = 8;

		// Token: 0x0400449D RID: 17565
		public int WarriorBattleLowestJiFen = 5;

		// Token: 0x0400449E RID: 17566
		public int WarriorBattleDie = 5;

		// Token: 0x0400449F RID: 17567
		public int WarriorBattleUltraKillParam1 = 27;

		// Token: 0x040044A0 RID: 17568
		public int WarriorBattleUltraKillParam2 = 3;

		// Token: 0x040044A1 RID: 17569
		public int WarriorBattleUltraKillParam3 = 30;

		// Token: 0x040044A2 RID: 17570
		public int WarriorBattleUltraKillParam4 = 75;

		// Token: 0x040044A3 RID: 17571
		public int WarriorBattleShutDownParam1 = -10;

		// Token: 0x040044A4 RID: 17572
		public int WarriorBattleShutDownParam2 = 5;

		// Token: 0x040044A5 RID: 17573
		public int WarriorBattleShutDownParam3 = 0;

		// Token: 0x040044A6 RID: 17574
		public int WarriorBattleShutDownParam4 = 100;

		// Token: 0x040044A7 RID: 17575
		public int SighUpStateMagicNum = 100000000;

		// Token: 0x040044A8 RID: 17576
		public string RoleParamsAwardsDefaultString = "";

		// Token: 0x040044A9 RID: 17577
		public bool PrepareGame;

		// Token: 0x040044AA RID: 17578
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
