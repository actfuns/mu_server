using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200008B RID: 139
	public class EscapeBattleData
	{
		// Token: 0x04000335 RID: 821
		public object Mutex = new object();

		// Token: 0x04000336 RID: 822
		public List<EscapeBattleAwardsConfig> AwardsConfig = new List<EscapeBattleAwardsConfig>();

		// Token: 0x04000337 RID: 823
		public List<EscapeBDuanAwardsConfig> DuanAwardsConfig = new List<EscapeBDuanAwardsConfig>();

		// Token: 0x04000338 RID: 824
		public Dictionary<int, EscapeBattleBirthPoint> MapBirthPointDict = new Dictionary<int, EscapeBattleBirthPoint>();

		// Token: 0x04000339 RID: 825
		public List<EscapeBattleCollection> CollectionConfigList = new List<EscapeBattleCollection>();

		// Token: 0x0400033A RID: 826
		public List<EscapeMapSafeArea> EscapeMapSafeAreaList = new List<EscapeMapSafeArea>();

		// Token: 0x0400033B RID: 827
		public EscapeBattleConfig Config = new EscapeBattleConfig();

		// Token: 0x0400033C RID: 828
		public int BuyDevilLossDiamonds = 5;

		// Token: 0x0400033D RID: 829
		public int KillReplyHp = 50000;

		// Token: 0x0400033E RID: 830
		public int[] BuffMaxLayerNum;

		// Token: 0x0400033F RID: 831
		public double[] BuffAttributeProportion;

		// Token: 0x04000340 RID: 832
		public int[] BuffAttributeType;

		// Token: 0x04000341 RID: 833
		public List<int[]> LifeSeedNum = new List<int[]>();

		// Token: 0x04000342 RID: 834
		public int MaxLifeNum = 5;

		// Token: 0x04000343 RID: 835
		public int DevilLossNum;

		// Token: 0x04000344 RID: 836
		public int ReadyMapCode;

		// Token: 0x04000345 RID: 837
		public DateTime EscapeStartTime;

		// Token: 0x04000346 RID: 838
		public double[] DebuffCalExtProps = new double[177];

		// Token: 0x04000347 RID: 839
		public int[] TeamBattleMap;

		// Token: 0x04000348 RID: 840
		public RunByTime SyncDataByTime = new RunByTime(15000L);

		// Token: 0x04000349 RID: 841
		public EscapeBattleSyncData SyncDataRequest = new EscapeBattleSyncData();

		// Token: 0x0400034A RID: 842
		public EscapeBattleSyncData SyncData = new EscapeBattleSyncData();

		// Token: 0x0400034B RID: 843
		public TimeSpan DiffTimeSpan = TimeSpan.Zero;

		// Token: 0x0400034C RID: 844
		public int LastStartSecs;

		// Token: 0x0400034D RID: 845
		public Dictionary<long, EscapeBattleFuBenData> KuaFuCopyDataDict = new Dictionary<long, EscapeBattleFuBenData>();

		// Token: 0x0400034E RID: 846
		public Dictionary<int, EscapeBattlePiPeiState> ConfirmBattleDict = new Dictionary<int, EscapeBattlePiPeiState>();

		// Token: 0x0400034F RID: 847
		public Queue<KeyValuePair<int, List<int>>> PKResultQueue = new Queue<KeyValuePair<int, List<int>>>();

		// Token: 0x04000350 RID: 848
		public Queue<KeyValuePair<int, int>> GameStateQueue = new Queue<KeyValuePair<int, int>>();
	}
}
