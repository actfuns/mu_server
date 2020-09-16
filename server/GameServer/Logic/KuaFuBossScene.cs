using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuBossScene
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

		
		public int ElapsedSeconds;

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public KuaFuBossSceneInfo SceneInfo;

		
		public KuaFuBossStatisticalData GameStatisticalData = new KuaFuBossStatisticalData();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public long NextNotifySceneStateDataTicks = 0L;

		
		public KuaFuBossSceneStateData SceneStateData = new KuaFuBossSceneStateData();

		
		public List<BattleDynamicMonsterItem> DynMonsterList = null;

		
		public HashSet<int> DynMonsterSet = new HashSet<int>();

		
		public HashSet<int> KilledMonsterSet = new HashSet<int>();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
