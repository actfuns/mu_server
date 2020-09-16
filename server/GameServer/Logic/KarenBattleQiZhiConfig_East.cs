using System;

namespace GameServer.Logic
{
	
	public class KarenBattleQiZhiConfig_East
	{
		
		public object Clone()
		{
			return new KarenBattleQiZhiConfig_East
			{
				ID = this.ID,
				MonsterID = this.MonsterID,
				PosX = this.PosX,
				PosY = this.PosY,
				BeginTime = this.BeginTime,
				RefreshCD = this.RefreshCD,
				CollectTime = this.CollectTime,
				HandInNum = this.HandInNum,
				BuffGoodsID = this.BuffGoodsID
			};
		}

		
		public int ID;

		
		public int MonsterID;

		
		public int PosX;

		
		public int PosY;

		
		public int BeginTime;

		
		public int RefreshCD;

		
		public int CollectTime;

		
		public int HandInNum;

		
		public int HoldTme;

		
		public int BuffGoodsID;

		
		public int BattleWhichSide;

		
		public bool Alive;

		
		public long DeadTicks;
	}
}
