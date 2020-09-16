using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	internal class CoupleArenaCopyScene
	{
		
		public int FuBenSeq;

		
		public int GameId;

		
		public int MapCode;

		
		public CopyMap CopyMap;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public int WinSide = 0;

		
		public long m_lPrevUpdateTime = 0L;

		
		public long m_lCurrUpdateTime = 0L;

		
		public Dictionary<int, int> EnterRoleSide = new Dictionary<int, int>();

		
		public bool IsYongQiMonsterExist = false;

		
		public int YongQiBuff_Role;

		
		public bool IsZhenAiMonsterExist = false;

		
		public int ZhenAiBuff_Role;

		
		public long ZhenAiBuff_StartMs;
	}
}
