using System;
using System.Collections.Generic;

namespace GameServer.Logic.OnePiece
{
	
	public class OnePieceTreasureEventConfig
	{
		
		public int ID = 0;

		
		public TreasureEventType Type = TreasureEventType.ETET_Null;

		
		public AwardsItemList GoodsList = new AwardsItemList();

		
		public OnePieceMoneyPair NewValue = new OnePieceMoneyPair();

		
		public int NewDiec = 0;

		
		public int NewSuperDiec = 0;

		
		public List<OnePieceGoodsPair> NeedGoods = new List<OnePieceGoodsPair>();

		
		public OnePieceMoneyPair NeedValue = new OnePieceMoneyPair();

		
		public List<int> MoveRange = new List<int>();

		
		public int FuBenID = 0;

		
		public List<OnePieceTreasureBoxPair> BoxList = new List<OnePieceTreasureBoxPair>();
	}
}
