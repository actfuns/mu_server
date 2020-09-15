using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200025C RID: 604
	public class CompBattleRuntimeData
	{
		// Token: 0x04000ECD RID: 3789
		public const string CompBattle_FileName = "Config/ForceCraft.xml";

		// Token: 0x04000ECE RID: 3790
		public const string CompStronghold_FileName = "Config/ForceStronghold.xml";

		// Token: 0x04000ECF RID: 3791
		public const string CompBattleBirth_FileName = "Config/ForceCraftBirth.xml";

		// Token: 0x04000ED0 RID: 3792
		public const string CompBattleReward_FileName = "Config/ForceCraftReward.xml";

		// Token: 0x04000ED1 RID: 3793
		public object Mutex = new object();

		// Token: 0x04000ED2 RID: 3794
		public Dictionary<int, CompBattleConfig> CompBattleConfigDict = new Dictionary<int, CompBattleConfig>();

		// Token: 0x04000ED3 RID: 3795
		public Dictionary<int, CompStrongholdConfig> CompStrongholdConfigDict = new Dictionary<int, CompStrongholdConfig>();

		// Token: 0x04000ED4 RID: 3796
		public Dictionary<KeyValuePair<int, int>, CompBattleBirthConfig> CompBattleBirthConfigDict = new Dictionary<KeyValuePair<int, int>, CompBattleBirthConfig>();

		// Token: 0x04000ED5 RID: 3797
		public List<CompBattleRewardConfig> CompBattleRewardConfigList = new List<CompBattleRewardConfig>();

		// Token: 0x04000ED6 RID: 3798
		public int[] CompBattleSingleIntegral;

		// Token: 0x04000ED7 RID: 3799
		public double[] CompBattleRewardRate;

		// Token: 0x04000ED8 RID: 3800
		public int CompBattleFlagDamage = 10;

		// Token: 0x04000ED9 RID: 3801
		public Dictionary<int, CompFuBenData> FuBenItemData = new Dictionary<int, CompFuBenData>();

		// Token: 0x04000EDA RID: 3802
		public Dictionary<KeyValuePair<int, int>, List<CompBattleWaitData>> CompBattleWaitQueueDict = new Dictionary<KeyValuePair<int, int>, List<CompBattleWaitData>>();

		// Token: 0x04000EDB RID: 3803
		public Dictionary<int, KeyValuePair<int, int>> CompBattleWaitAllDict = new Dictionary<int, KeyValuePair<int, int>>();

		// Token: 0x04000EDC RID: 3804
		public CompBattleBaseData compBattleBaseData = new CompBattleBaseData();
	}
}
