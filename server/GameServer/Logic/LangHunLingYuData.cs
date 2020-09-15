using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020003D2 RID: 978
	public class LangHunLingYuData
	{
		// Token: 0x0400198B RID: 6539
		public object Mutex = new object();

		// Token: 0x0400198C RID: 6540
		public Dictionary<int, List<MapBirthPoint>> MapBirthPointListDict = new Dictionary<int, List<MapBirthPoint>>();

		// Token: 0x0400198D RID: 6541
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		// Token: 0x0400198E RID: 6542
		public List<LangHunLingYuQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LangHunLingYuQiZhiBuffOwnerData>();

		// Token: 0x0400198F RID: 6543
		public List<int> MapCodeList = new List<int>();

		// Token: 0x04001990 RID: 6544
		public List<int> MapCodeLongTaList = new List<int>();

		// Token: 0x04001991 RID: 6545
		public int[] BattleWitchSideJunQi = null;

		// Token: 0x04001992 RID: 6546
		public int CutLifeV = 10;

		// Token: 0x04001993 RID: 6547
		public HashSet<int> JunQiMonsterHashSet = new HashSet<int>();

		// Token: 0x04001994 RID: 6548
		public int MinZhuanSheng = 0;

		// Token: 0x04001995 RID: 6549
		public int MinLevel = 0;

		// Token: 0x04001996 RID: 6550
		public long EnrollTime = 1800L;

		// Token: 0x04001997 RID: 6551
		public int SuperQiZhiNpcId = 80000;

		// Token: 0x04001998 RID: 6552
		public Dictionary<int, double[]> QiZhiBuffDisableParamsDict = new Dictionary<int, double[]>();

		// Token: 0x04001999 RID: 6553
		public Dictionary<int, double[]> QiZhiBuffEnableParamsDict = new Dictionary<int, double[]>();

		// Token: 0x0400199A RID: 6554
		public int SuperQiZhiOwnerBirthPosX;

		// Token: 0x0400199B RID: 6555
		public int SuperQiZhiOwnerBirthPosY;

		// Token: 0x0400199C RID: 6556
		public Dictionary<RangeKey, LangHunLingYuSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, LangHunLingYuSceneInfo>(RangeKey.Comparer);

		// Token: 0x0400199D RID: 6557
		public Dictionary<int, LangHunLingYuSceneInfo> SceneDataDict = new Dictionary<int, LangHunLingYuSceneInfo>();

		// Token: 0x0400199E RID: 6558
		public List<LangHunLingYuSceneInfo> SceneDataList = new List<LangHunLingYuSceneInfo>();

		// Token: 0x0400199F RID: 6559
		public int SceneInfoId = 1;

		// Token: 0x040019A0 RID: 6560
		public Dictionary<int, CityLevelInfo> CityLevelInfoDict = new Dictionary<int, CityLevelInfo>();

		// Token: 0x040019A1 RID: 6561
		public Queue<LangHunLingYuStatisticalData> StatisticalDataQueue = new Queue<LangHunLingYuStatisticalData>();

		// Token: 0x040019A2 RID: 6562
		public int GongNengOpenDaysFromKaiFu = 5;

		// Token: 0x040019A3 RID: 6563
		public TimeSpan NoRequestTimeStart;

		// Token: 0x040019A4 RID: 6564
		public TimeSpan NoRequestTimeEnd;

		// Token: 0x040019A5 RID: 6565
		public int[] WeekPoints = new int[0];

		// Token: 0x040019A6 RID: 6566
		public DateTime TimePoints;

		// Token: 0x040019A7 RID: 6567
		public DateTime WangChengZhanFightingDateTime;

		// Token: 0x040019A8 RID: 6568
		public int WaitingEnterSecs;

		// Token: 0x040019A9 RID: 6569
		public int PrepareSecs;

		// Token: 0x040019AA RID: 6570
		public int FightingSecs;

		// Token: 0x040019AB RID: 6571
		public int ClearRolesSecs;

		// Token: 0x040019AC RID: 6572
		public bool CanRequestState = false;

		// Token: 0x040019AD RID: 6573
		public int MapGridWidth = 100;

		// Token: 0x040019AE RID: 6574
		public int MapGridHeight = 100;

		// Token: 0x040019AF RID: 6575
		public long ChengHaoBHid = 0L;

		// Token: 0x040019B0 RID: 6576
		public int MaxTakingHuangGongSecs = 5000;

		// Token: 0x040019B1 RID: 6577
		public ConcurrentDictionary<int, LangHunLingYuScene> SceneDict = new ConcurrentDictionary<int, LangHunLingYuScene>();

		// Token: 0x040019B2 RID: 6578
		public Dictionary<long, LangHunLingYuCityData> BangHui2CityDict = new Dictionary<long, LangHunLingYuCityData>();

		// Token: 0x040019B3 RID: 6579
		public Dictionary<int, LangHunLingYuCityData> CityDataDict = new Dictionary<int, LangHunLingYuCityData>();

		// Token: 0x040019B4 RID: 6580
		public Dictionary<long, LangHunLingYuBangHuiData> BangHuiDataDict = new Dictionary<long, LangHunLingYuBangHuiData>();

		// Token: 0x040019B5 RID: 6581
		public Dictionary<int, List<int>> OtherCityList = null;

		// Token: 0x040019B6 RID: 6582
		public List<LangHunLingYuKingHist> OwnerHistList = null;

		// Token: 0x040019B7 RID: 6583
		public Dictionary<long, LangHunLingYuBangHuiDataEx> BangHuiDataExDict = new Dictionary<long, LangHunLingYuBangHuiDataEx>();

		// Token: 0x040019B8 RID: 6584
		public Dictionary<int, LangHunLingYuCityDataEx> CityDataExDict = new Dictionary<int, LangHunLingYuCityDataEx>();

		// Token: 0x040019B9 RID: 6585
		public Dictionary<int, LangHunLingYuFuBenData> FuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();

		// Token: 0x040019BA RID: 6586
		public Dictionary<int, BangHuiMiniData> BangHuiMiniDataCacheDict = new Dictionary<int, BangHuiMiniData>();
	}
}
