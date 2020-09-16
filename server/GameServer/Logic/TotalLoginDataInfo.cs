using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class TotalLoginDataInfo
	{
		
		
		
		public int TotalLoginDays { get; set; }

		
		
		
		public List<GoodsData> NormalAward { get; set; }

		
		
		
		public List<GoodsData> Award0 { get; set; }

		
		
		
		public List<GoodsData> Award1 { get; set; }

		
		
		
		public List<GoodsData> Award2 { get; set; }

		
		
		
		public List<GoodsData> Award3 { get; set; }

		
		
		
		public List<GoodsData> Award5 { get; set; }
	}
}
