using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class BangHuiMatchScene
	{
		
		public void CleanAllInfo()
		{
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = GameSceneStatuses.STATUS_NULL;
			this.m_bEndFlag = false;
		}

		
		public int FuBenSeqId = 0;

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public int SuccessSide = 0;

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public Dictionary<int, CopyMap> CopyMapDict = new Dictionary<int, CopyMap>();

		
		public BHMatchConfig SceneInfo;

		
		public BangHuiMatchScoreData ScoreData = new BangHuiMatchScoreData();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public Dictionary<int, BHMatchClientContextData> ClientContextDataDict = new Dictionary<int, BHMatchClientContextData>();

		
		public BHMatchClientContextData ClientContextMVP = new BHMatchClientContextData();

		
		public Dictionary<int, BHMatchQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, BHMatchQiZhiConfig>();

		
		public BangHuiMatchStatisticalData GameStatisticalData = new BangHuiMatchStatisticalData();

		
		public int LT_BHServerID;

		
		public int LT_BattleWhichSide;

		
		public long LT_OwnTicks;

		
		public long LT_OwnTicksDelta;

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
