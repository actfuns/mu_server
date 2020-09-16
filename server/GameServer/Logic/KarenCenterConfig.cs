using System;

namespace GameServer.Logic
{
	
	public class KarenCenterConfig
	{
		
		public object Clone()
		{
			return new KarenCenterConfig
			{
				ID = this.ID,
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				Radius = this.Radius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum,
				OccupyTime = this.OccupyTime
			};
		}

		
		public int ID;

		
		public int NPCID;

		
		public int PosX;

		
		public int PosY;

		
		public int Radius;

		
		public int ProduceTime;

		
		public int ProduceNum;

		
		public int OccupyTime;

		
		public int BattleWhichSide;

		
		public long OwnTicks;

		
		public long OwnTicksDelta;

		
		public long OwnCalculateSide;

		
		public long OwnCalculateTicks;
	}
}
