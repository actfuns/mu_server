using System;

namespace GameServer.Logic
{
	
	public class KingOfBattleQiZhiConfig
	{
		
		public object Clone()
		{
			return new KingOfBattleQiZhiConfig
			{
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				QiZhiMonsterID = this.QiZhiMonsterID
			};
		}

		
		public int NPCID;

		
		public int PosX;

		
		public int PosY;

		
		public int QiZhiMonsterID;

		
		public int BattleWhichSide;

		
		public bool Alive;

		
		public long DeadTicks;
	}
}
