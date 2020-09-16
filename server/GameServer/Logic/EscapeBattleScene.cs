using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class EscapeBattleScene
	{
		
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		
		public int m_nMapCode = 0;

		
		public int FuBenSeqId = 0;

		
		public int CopyMapId = 0;

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lFightTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public EscapeBattleGameSceneStatuses m_eStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;

		
		public int SuccessSide = 0;

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public EscapeBattleMatchConfig SceneInfo;

		
		public EscapeBattleFuBenData FuBenData;

		
		public Dictionary<int, GameClient> ClientDict = new Dictionary<int, GameClient>();

		
		public EscapeBattleSideScore ScoreData = new EscapeBattleSideScore();

		
		public Dictionary<int, List<EscapeBattleRoleInfo>> ClientContextDataDict = new Dictionary<int, List<EscapeBattleRoleInfo>>();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public List<EscapeBattleCollection> CollectionConfigList = new List<EscapeBattleCollection>();

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public EscapeBattleStatisticalData GameStatisticalData = new EscapeBattleStatisticalData();

		
		public double[][] TopClientCalExtProps = new double[2][];

		
		public long AreaDamageTicks;

		
		public int SafeAreaRefreshState = -1;

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
