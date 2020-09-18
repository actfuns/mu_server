using System;

namespace GameServer.Logic
{
	
	public class BHMatchQiZhiConfig
	{
		
		public object Clone()
		{
			return new BHMatchQiZhiConfig
			{
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				RebirthSiteX = this.RebirthSiteX,
				RebirthSiteY = this.RebirthSiteY,
				RebirthRadius = this.RebirthRadius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum
			};
		}

		
		public int NPCID;

		
		public int PosX;

		
		public int PosY;

		
		public int RebirthSiteX;

		
		public int RebirthSiteY;

		
		public int RebirthRadius;

		
		public int ProduceTime;

		
		public int ProduceNum;

		
		public int BattleWhichSide;

		
		public bool Alive;

		
		public long DeadTicks;

		
		public long OwnTicks;

		
		public long OwnTicksDelta;
	}
}
