using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SongLiItem
	{
		
		public long StartTicks = 0L;

		
		public long EndTicks = 0L;

		
		public int IsNeedCode = 0;

		
		public Dictionary<int, List<GoodsData>> SongGoodsDataDict = new Dictionary<int, List<GoodsData>>();
	}
}
