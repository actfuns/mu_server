using System;
using GameServer.Logic;

namespace Server.Data
{
	
	public class ThemeBossScene
	{
		
		public int MapCode;

		
		public ThemeBossConfig BossConfigInfo;

		
		public BattleStates State;

		
		public long StartTick;

		
		public long EndTick;

		
		public int AliveBossNum;

		
		public bool BroadCast4014 = false;
	}
}
