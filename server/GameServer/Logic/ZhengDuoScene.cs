using System;
using System.Collections.Generic;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class ZhengDuoScene
	{
		
		public void Start()
		{
			this.m_lPrepareTime = this.StartTimeTicks;
			this.m_lBeginTime = this.m_lPrepareTime + (long)(this.SceneInfo.SecondWait * 1000);
			this.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
			this.StateTimeData.GameType = 17;
			this.StateTimeData.State = (int)this.m_eStatus;
			this.StateTimeData.EndTicks = this.m_lBeginTime;
		}

		
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		
		public int m_nMapCode = 0;

		
		public int FuBenSeqId = 0;

		
		public int CopyMapId = 0;

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public int SuccessSide = 0;

		
		public int KillUsedTicks = 1800000;

		
		public int KillerId;

		
		public bool m_bEndFlag = false;

		
		public bool PreliminarisesMode = true;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public ZhengDuoSceneInfo SceneInfo;

		
		public ZhengDuoRankData[] RankDatas = new ZhengDuoRankData[2];

		
		public ZhengDuoScoreData[] ScoreData = new ZhengDuoScoreData[4];

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public int IsMonsterFlag = 0;

		
		public long CreateMonsterTime = 0L;

		
		public Dictionary<int, ZhengDuoScoreData> ClientContextDataDict = new Dictionary<int, ZhengDuoScoreData>();

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;

		
		public int Age;
	}
}
