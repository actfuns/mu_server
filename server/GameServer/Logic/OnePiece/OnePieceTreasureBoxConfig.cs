using System;

namespace GameServer.Logic.OnePiece
{
	
	public class OnePieceTreasureBoxConfig
	{
		
		public int ID = 0;

		
		public TeasureBoxType Type = TeasureBoxType.ETBT_Null;

		
		public AwardsItemList Goods = new AwardsItemList();

		
		public int Num = 0;

		
		public int BeginNum = 0;

		
		public int EndNum = 0;
	}
}
