using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompMineScene
	{
		
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
			this.ScoreData = new CompMineSideScore();
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

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public CompMineConfig SceneInfo;

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public Dictionary<int, CompMineClientContextData> ClientContextDataDict = new Dictionary<int, CompMineClientContextData>();

		
		public CompMineSideScore ScoreData = new CompMineSideScore();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
