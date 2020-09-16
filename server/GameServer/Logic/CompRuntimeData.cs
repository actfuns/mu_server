using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompRuntimeData
	{
		
		public const string Comp_FileName = "Config/Comp.xml";

		
		public const string CompResources_FileName = "Config/CompResources.xml";

		
		public const string CompSolderSite_FileName = "Config/CompSolderSite.xml";

		
		public const string CompSolder_FileName = "Config/CompSolder.xml";

		
		public const string CompNotice_FileName = "Config/CompNotice.xml";

		
		public const string CompLevel_FileName = "Config/CompLevel.xml";

		
		public object Mutex = new object();

		
		public Dictionary<int, CompConfig> CompConfigDict = new Dictionary<int, CompConfig>();

		
		public Dictionary<int, CompResourcesConfig> CompResourcesConfigDict = new Dictionary<int, CompResourcesConfig>();

		
		public List<CompSolderSiteConfig> CompSolderSiteConfigList = new List<CompSolderSiteConfig>();

		
		public Dictionary<KeyValuePair<int, int>, CompSolderConfig> CompSolderConfigDict = new Dictionary<KeyValuePair<int, int>, CompSolderConfig>();

		
		public Dictionary<int, CompNoticeConfig> CompNoticeConfigDict = new Dictionary<int, CompNoticeConfig>();

		
		public List<CompLevelConfig> CompLevelConfigList = new List<CompLevelConfig>();

		
		public AwardsItemList CompRecommend = new AwardsItemList();

		
		public double CompRecommendRatio = 0.6;

		
		public double CompReplaceAmerce;

		
		public int CompReplaceNeed;

		
		public int[] CompSolderCD;

		
		public int[] CompBossCompNum;

		
		public int[] CompBossCompHonor;

		
		public double[] CompBossRealive;

		
		public int[] CompEnemy;

		
		public double CompEnemyHurtNum = 0.05;

		
		public List<Tuple<int, int>> CompShop = new List<Tuple<int, int>>();

		
		public int[] CompShopDuiHuanType;

		
		public Dictionary<int, int> MaxDailyTaskNumDict = new Dictionary<int, int>();

		
		public Dictionary<int, List<int>> CompTaskBeginDict = new Dictionary<int, List<int>>();

		
		public long NextHeartBeatTicks = 0L;

		
		public List<KFCompChat> CompChatList = new List<KFCompChat>();

		
		public List<KFCompNotice> CompNoticeList = new List<KFCompNotice>();
	}
}
