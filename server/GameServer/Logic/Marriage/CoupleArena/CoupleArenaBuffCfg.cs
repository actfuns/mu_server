using System;
using System.Collections.Generic;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	public class CoupleArenaBuffCfg
	{
		
		public int Type;

		
		public string Name;

		
		public List<CoupleArenaBuffCfg.RandPos> RandPosList;

		
		public List<int> FlushSecList;

		
		public Dictionary<ExtPropIndexes, double> ExtProps;

		
		public int MonsterId;

		
		public class RandPos
		{
			
			public int X;

			
			public int Y;

			
			public int R;
		}
	}
}
