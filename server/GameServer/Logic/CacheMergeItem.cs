using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class CacheMergeItem
	{
		
		
		
		public List<int> NewGoodsID { get; set; }

		
		
		
		public List<int> OrigGoodsIDList { get; set; }

		
		
		
		public List<int> OrigGoodsNumList { get; set; }

		
		
		
		public int DianJuan { get; set; }

		
		
		
		public int Money { get; set; }

		
		
		
		public int ZhenQi { get; set; }

		
		
		
		public int JiFen { get; set; }

		
		
		
		public int JingYuan { get; set; }

		
		
		
		public int SuccessRate { get; set; }

		
		
		
		public Dictionary<string, int> DestroyGoodsIDs { get; set; }

		
		
		
		public string PubStartTime { get; set; }

		
		
		
		public string PubEndTime { get; set; }

		
		public int MergeType;
	}
}
