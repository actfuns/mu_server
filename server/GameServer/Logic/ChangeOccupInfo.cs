using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class ChangeOccupInfo
	{
		
		
		
		public int OccupationID { get; set; }

		
		
		
		public int NeedLevel { get; set; }

		
		
		
		public int NeedMoney { get; set; }

		
		
		
		public List<GoodsData> NeedGoodsDataList { get; set; }

		
		
		
		public List<GoodsData> AwardGoodsDataList { get; set; }

		
		
		
		public int AwardPropPoint { get; set; }
	}
}
