using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KarenBattleScene
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

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public int m_nPlarerCount = 0;

		
		public int SuccessSide = 0;

		
		public bool m_bEndFlag = false;

		
		public bool GuangMuNotify1 = false;

		
		public bool GuangMuNotify2 = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public KarenBattleSceneInfo SceneInfo;

		
		public List<KarenBattleScoreData> ScoreData = new List<KarenBattleScoreData>();

		
		public Dictionary<int, KarenBattleClientContextData> ClientContextDataDict = new Dictionary<int, KarenBattleClientContextData>();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public Dictionary<int, KarenBattleQiZhiConfig_West> NPCID2QiZhiConfigDict = new Dictionary<int, KarenBattleQiZhiConfig_West>();

		
		public Dictionary<int, KarenCenterConfig> KarenCenterConfigDict = new Dictionary<int, KarenCenterConfig>();

		
		public Dictionary<string, KarenBattleSceneBuff> SceneBuffDict = new Dictionary<string, KarenBattleSceneBuff>();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
