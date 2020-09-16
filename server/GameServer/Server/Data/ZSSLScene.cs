using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	
	public class ZSSLScene
	{
		
		public void CleanAllInfo()
		{
			this.State = BattleStates.NoBattle;
			this.StartTick = 0L;
			this.EndTick = 0L;
			this.ClearTick = 0L;
			this.StatusEndTime = 0L;
			this.BossDie = false;
			this.AttackLog = new BossAttackLog
			{
				InjureSum = 0L,
				BHInjure = new Dictionary<long, BHAttackLog>(),
				BHAttackRank = new List<BHAttackLog>()
			};
		}

		
		public ZhuanShengMapInfo SceneInfo;

		
		public CopyMap m_CopyMap;

		
		public BattleStates State;

		
		public long StartTick;

		
		public long EndTick;

		
		public long ClearTick;

		
		public long StatusEndTime;

		
		public bool BossDie;

		
		public BossAttackLog AttackLog;
	}
}
