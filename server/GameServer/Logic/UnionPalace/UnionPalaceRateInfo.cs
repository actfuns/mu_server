using System;
using System.Collections.Generic;

namespace GameServer.Logic.UnionPalace
{
	
	public class UnionPalaceRateInfo
	{
		
		public int StatueLevel = 0;

		
		public List<int> RateList = new List<int>();

		
		public Dictionary<int, List<int>> AddNumList = new Dictionary<int, List<int>>();
	}
}
