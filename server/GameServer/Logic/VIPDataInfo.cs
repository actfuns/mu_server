using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class VIPDataInfo
	{
		
		
		
		public int AwardID { get; set; }

		
		
		
		public int VIPlev { get; set; }

		
		
		
		public int DailyMaxUseTimes { get; set; }

		
		
		
		public List<GoodsData> AwardGoods { get; set; }

		
		
		
		public int ZuanShi { get; set; }

		
		
		
		public int BindZuanShi { get; set; }

		
		
		
		public int JinBi { get; set; }

		
		
		
		public int BindJinBi { get; set; }

		
		
		
		public int[] BufferGoods { get; set; }

		
		
		
		public int XiHongMing { get; set; }

		
		
		
		public int XiuLi { get; set; }
	}
}
