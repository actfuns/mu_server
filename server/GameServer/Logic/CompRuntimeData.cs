using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200026A RID: 618
	public class CompRuntimeData
	{
		// Token: 0x04000F42 RID: 3906
		public const string Comp_FileName = "Config/Comp.xml";

		// Token: 0x04000F43 RID: 3907
		public const string CompResources_FileName = "Config/CompResources.xml";

		// Token: 0x04000F44 RID: 3908
		public const string CompSolderSite_FileName = "Config/CompSolderSite.xml";

		// Token: 0x04000F45 RID: 3909
		public const string CompSolder_FileName = "Config/CompSolder.xml";

		// Token: 0x04000F46 RID: 3910
		public const string CompNotice_FileName = "Config/CompNotice.xml";

		// Token: 0x04000F47 RID: 3911
		public const string CompLevel_FileName = "Config/CompLevel.xml";

		// Token: 0x04000F48 RID: 3912
		public object Mutex = new object();

		// Token: 0x04000F49 RID: 3913
		public Dictionary<int, CompConfig> CompConfigDict = new Dictionary<int, CompConfig>();

		// Token: 0x04000F4A RID: 3914
		public Dictionary<int, CompResourcesConfig> CompResourcesConfigDict = new Dictionary<int, CompResourcesConfig>();

		// Token: 0x04000F4B RID: 3915
		public List<CompSolderSiteConfig> CompSolderSiteConfigList = new List<CompSolderSiteConfig>();

		// Token: 0x04000F4C RID: 3916
		public Dictionary<KeyValuePair<int, int>, CompSolderConfig> CompSolderConfigDict = new Dictionary<KeyValuePair<int, int>, CompSolderConfig>();

		// Token: 0x04000F4D RID: 3917
		public Dictionary<int, CompNoticeConfig> CompNoticeConfigDict = new Dictionary<int, CompNoticeConfig>();

		// Token: 0x04000F4E RID: 3918
		public List<CompLevelConfig> CompLevelConfigList = new List<CompLevelConfig>();

		// Token: 0x04000F4F RID: 3919
		public AwardsItemList CompRecommend = new AwardsItemList();

		// Token: 0x04000F50 RID: 3920
		public double CompRecommendRatio = 0.6;

		// Token: 0x04000F51 RID: 3921
		public double CompReplaceAmerce;

		// Token: 0x04000F52 RID: 3922
		public int CompReplaceNeed;

		// Token: 0x04000F53 RID: 3923
		public int[] CompSolderCD;

		// Token: 0x04000F54 RID: 3924
		public int[] CompBossCompNum;

		// Token: 0x04000F55 RID: 3925
		public int[] CompBossCompHonor;

		// Token: 0x04000F56 RID: 3926
		public double[] CompBossRealive;

		// Token: 0x04000F57 RID: 3927
		public int[] CompEnemy;

		// Token: 0x04000F58 RID: 3928
		public double CompEnemyHurtNum = 0.05;

		// Token: 0x04000F59 RID: 3929
		public List<Tuple<int, int>> CompShop = new List<Tuple<int, int>>();

		// Token: 0x04000F5A RID: 3930
		public int[] CompShopDuiHuanType;

		// Token: 0x04000F5B RID: 3931
		public Dictionary<int, int> MaxDailyTaskNumDict = new Dictionary<int, int>();

		// Token: 0x04000F5C RID: 3932
		public Dictionary<int, List<int>> CompTaskBeginDict = new Dictionary<int, List<int>>();

		// Token: 0x04000F5D RID: 3933
		public long NextHeartBeatTicks = 0L;

		// Token: 0x04000F5E RID: 3934
		public List<KFCompChat> CompChatList = new List<KFCompChat>();

		// Token: 0x04000F5F RID: 3935
		public List<KFCompNotice> CompNoticeList = new List<KFCompNotice>();
	}
}
