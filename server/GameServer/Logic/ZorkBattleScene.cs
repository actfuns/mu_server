using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class ZorkBattleScene
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

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public ZorkBattleSceneInfo SceneInfo;

		
		public ZorkBattleSideScore ScoreData = new ZorkBattleSideScore();

		
		public Dictionary<int, List<ZorkBattleRoleInfo>> ClientContextDataDict = new Dictionary<int, List<ZorkBattleRoleInfo>>();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public List<ZorkBattleArmyConfig> ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();

		
		public Dictionary<int, List<ZorkBattleMonsterConfig>> ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public Dictionary<string, ZorkBattleSceneBuff> SceneBuffDict = new Dictionary<string, ZorkBattleSceneBuff>();

		
		public ZorkBattleStatisticalData GameStatisticalData = new ZorkBattleStatisticalData();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
