using System;
using System.Collections.Generic;

namespace GameServer.Logic.OnePiece
{
	
	public class OnePieceTreasureMapConfig
	{
		
		public int ID = 0;

		
		public int Num = 0;

		
		public int Floor = 0;

		
		public TriggerType Trigger = TriggerType.ETT_Null;

		
		public int Score = 0;

		
		public List<OnePieceRandomEvent> LisRandomEvent = new List<OnePieceRandomEvent>();
	}
}
