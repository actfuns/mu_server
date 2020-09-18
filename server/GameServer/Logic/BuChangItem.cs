using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class BuChangItem
	{
		
		public int MinLevel = 0;

		
		public int MinZhuanSheng = 0;

		
		public int MaxLevel = 0;

		
		public int MaxZhuanSheng = 0;

		
		public int MoJing = 0;

		
		public long Experience = 0L;

		
		public List<GoodsData> GoodsDataList = new List<GoodsData>();
	}
}
