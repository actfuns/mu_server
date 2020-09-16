using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class XiLianShuXing
	{
		
		public int ID;

		
		public string Name;

		
		public int NeedJinBi;

		
		public int NeedZuanShi;

		
		public List<int> NeedGoodsIDs = new List<int>();

		
		public List<int> NeedGoodsCounts = new List<int>();

		
		public Dictionary<int, List<long>> PromoteJinBiRange = new Dictionary<int, List<long>>();

		
		public Dictionary<int, List<long>> PromoteZuanShiRange = new Dictionary<int, List<long>>();

		
		public Dictionary<int, int> PromotePropLimit = new Dictionary<int, int>();

		
		public Dictionary<int, int> PromoteRangeMin = new Dictionary<int, int>();

		
		public Dictionary<int, int> PromoteRangeMax = new Dictionary<int, int>();
	}
}
