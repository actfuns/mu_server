using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class HuanYingSiYuanScene
	{
		
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_nPlarerCount = 0;
			this.m_bEndFlag = false;
		}

		
		public int m_nMapCode = 0;

		
		public int FuBenSeqId = 0;

		
		public int CopyMapId = 0;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public int m_nPlarerCount = 0;

		
		public Dictionary<int, HuanYingSiYuanShengBeiContextData> ShengBeiContextDict = new Dictionary<int, HuanYingSiYuanShengBeiContextData>();

		
		public int SuccessSide = 0;

		
		public HuanYingSiYuanScoreInfoData ScoreInfoData = new HuanYingSiYuanScoreInfoData();

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
	}
}
