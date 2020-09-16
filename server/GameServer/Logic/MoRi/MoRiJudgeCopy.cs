using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic.MoRi
{
	
	internal class MoRiJudgeCopy
	{
		
		public CopyMap MyCopyMap;

		
		public long GameId;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public long DeadlineMs = 0L;

		
		public long CurrStateBeginMs = 0L;

		
		public int CurrMonsterIdx = -1;

		
		public long CurrMonsterBegin = 0L;

		
		public List<MoRiMonsterData> MonsterList = new List<MoRiMonsterData>();

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public DateTime StartTime;

		
		public DateTime EndTime;

		
		public int LimitKillCount = 0;

		
		public int RoleCount;

		
		public bool Passed;
	}
}
