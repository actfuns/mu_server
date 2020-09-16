using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class EscapeBDuanAwardsConfig
	{
		
		public int ID;

		
		public string RankLevelName;

		
		public int RankValue;

		
		public List<List<int>> WinRankValue;

		
		public List<List<int>> LoseRankValue;

		
		public object FirstWinAwardsItemList;

		
		public object WinAwardsItemList;

		
		public object LoseAwardsItemList;
	}
}
