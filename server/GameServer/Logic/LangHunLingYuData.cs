using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;

namespace GameServer.Logic
{
	
	public class LangHunLingYuData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, List<MapBirthPoint>> MapBirthPointListDict = new Dictionary<int, List<MapBirthPoint>>();

		
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		
		public List<LangHunLingYuQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LangHunLingYuQiZhiBuffOwnerData>();

		
		public List<int> MapCodeList = new List<int>();

		
		public List<int> MapCodeLongTaList = new List<int>();

		
		public int[] BattleWitchSideJunQi = null;

		
		public int CutLifeV = 10;

		
		public HashSet<int> JunQiMonsterHashSet = new HashSet<int>();

		
		public int MinZhuanSheng = 0;

		
		public int MinLevel = 0;

		
		public long EnrollTime = 1800L;

		
		public int SuperQiZhiNpcId = 80000;

		
		public Dictionary<int, double[]> QiZhiBuffDisableParamsDict = new Dictionary<int, double[]>();

		
		public Dictionary<int, double[]> QiZhiBuffEnableParamsDict = new Dictionary<int, double[]>();

		
		public int SuperQiZhiOwnerBirthPosX;

		
		public int SuperQiZhiOwnerBirthPosY;

		
		public Dictionary<RangeKey, LangHunLingYuSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, LangHunLingYuSceneInfo>(RangeKey.Comparer);

		
		public Dictionary<int, LangHunLingYuSceneInfo> SceneDataDict = new Dictionary<int, LangHunLingYuSceneInfo>();

		
		public List<LangHunLingYuSceneInfo> SceneDataList = new List<LangHunLingYuSceneInfo>();

		
		public int SceneInfoId = 1;

		
		public Dictionary<int, CityLevelInfo> CityLevelInfoDict = new Dictionary<int, CityLevelInfo>();

		
		public Queue<LangHunLingYuStatisticalData> StatisticalDataQueue = new Queue<LangHunLingYuStatisticalData>();

		
		public int GongNengOpenDaysFromKaiFu = 5;

		
		public TimeSpan NoRequestTimeStart;

		
		public TimeSpan NoRequestTimeEnd;

		
		public int[] WeekPoints = new int[0];

		
		public DateTime TimePoints;

		
		public DateTime WangChengZhanFightingDateTime;

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public bool CanRequestState = false;

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;

		
		public long ChengHaoBHid = 0L;

		
		public int MaxTakingHuangGongSecs = 5000;

		
		public ConcurrentDictionary<int, LangHunLingYuScene> SceneDict = new ConcurrentDictionary<int, LangHunLingYuScene>();

		
		public Dictionary<long, LangHunLingYuCityData> BangHui2CityDict = new Dictionary<long, LangHunLingYuCityData>();

		
		public Dictionary<int, LangHunLingYuCityData> CityDataDict = new Dictionary<int, LangHunLingYuCityData>();

		
		public Dictionary<long, LangHunLingYuBangHuiData> BangHuiDataDict = new Dictionary<long, LangHunLingYuBangHuiData>();

		
		public Dictionary<int, List<int>> OtherCityList = null;

		
		public List<LangHunLingYuKingHist> OwnerHistList = null;

		
		public Dictionary<long, LangHunLingYuBangHuiDataEx> BangHuiDataExDict = new Dictionary<long, LangHunLingYuBangHuiDataEx>();

		
		public Dictionary<int, LangHunLingYuCityDataEx> CityDataExDict = new Dictionary<int, LangHunLingYuCityDataEx>();

		
		public Dictionary<int, LangHunLingYuFuBenData> FuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();

		
		public Dictionary<int, BangHuiMiniData> BangHuiMiniDataCacheDict = new Dictionary<int, BangHuiMiniData>();
	}
}
