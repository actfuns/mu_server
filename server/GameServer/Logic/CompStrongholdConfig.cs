using System;

namespace GameServer.Logic
{
	
	public class CompStrongholdConfig
	{
		
		public object Clone()
		{
			return new CompStrongholdConfig
			{
				ID = this.ID,
				MapCode = this.MapCode,
				QiZhiID = this.QiZhiID,
				Name = this.Name,
				QiZuoID = this.QiZuoID,
				PosX = this.PosX,
				PosY = this.PosY,
				Rate = this.Rate,
				Point = this.Point
			};
		}

		
		public int ID;

		
		public int MapCode;

		
		public int[] QiZhiID;

		
		public string Name;

		
		public int QiZuoID;

		
		public int PosX;

		
		public int PosY;

		
		public double Rate;

		
		public int Point;

		
		public int BattleWhichSide;

		
		public int BattleWhichSideLast;

		
		public bool Alive;

		
		public long DeadTicks;
	}
}
