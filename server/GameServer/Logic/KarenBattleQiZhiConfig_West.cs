using System;

namespace GameServer.Logic
{
	
	public class KarenBattleQiZhiConfig_West
	{
		
		public object Clone()
		{
			return new KarenBattleQiZhiConfig_West
			{
				ID = this.ID,
				QiZhiID = this.QiZhiID,
				QiZuoID = this.QiZuoID,
				PosX = this.PosX,
				PosY = this.PosY,
				BirthX = this.BirthX,
				BirthY = this.BirthY,
				BirthRadius = this.BirthRadius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum
			};
		}

		
		public int ID;

		
		public int QiZhiID;

		
		public int QiZuoID;

		
		public int PosX;

		
		public int PosY;

		
		public int BirthX;

		
		public int BirthY;

		
		public int BirthRadius;

		
		public int ProduceTime;

		
		public int ProduceNum;

		
		public int BattleWhichSide;

		
		public bool Alive;

		
		public long DeadTicks;

		
		public long OwnTicks;

		
		public long OwnTicksDelta;
	}
}
