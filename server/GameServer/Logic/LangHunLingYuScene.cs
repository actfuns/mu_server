using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class LangHunLingYuScene
	{
		
		public void CleanAllInfo()
		{
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
		}

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public int SuccessSide = 0;

		
		public int GameId;

		
		public Dictionary<int, CopyMap> CopyMapDict = new Dictionary<int, CopyMap>();

		
		public bool HasGuangMu = true;

		
		public LangHunLingYuSceneInfo SceneInfo;

		
		public CityLevelInfo LevelInfo;

		
		public List<BangHuiRoleCountData> LongTaBHRoleCountList = new List<BangHuiRoleCountData>();

		
		public LangHunLingYuLongTaOwnerData LongTaOwnerData = new LangHunLingYuLongTaOwnerData();

		
		public List<LangHunLingYuQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LangHunLingYuQiZhiBuffOwnerData>();

		
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		
		public int LastTheOnlyOneBangHui = 0;

		
		public int SuperQiZhiOwnerBhid;

		
		public long BangHuiTakeHuangGongTicks;

		
		public long LastAddBangZhanAwardsTicks = 0L;

		
		public Dictionary<int, LangHunLingYuClientContextData> ClientContextDataDict = new Dictionary<int, LangHunLingYuClientContextData>();

		
		public LangHunLingYuCityData CityData = new LangHunLingYuCityData();

		
		public Dictionary<int, BangHuiMiniData> BHID2BangHuiMiniDataDict = new Dictionary<int, BangHuiMiniData>();

		
		public int SuccessBangHuiId;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();
	}
}
