using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class BigAwardItem
	{
		
		public long StartTicks = 0L;

		
		public long EndTicks = 0L;

		
		public Dictionary<int, int> NeedJiFenDict = new Dictionary<int, int>();

		
		public Dictionary<int, List<GoodsData>> GoodsDataListDict = new Dictionary<int, List<GoodsData>>();
	}
}
