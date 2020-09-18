using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class ChangeLifeDataInfo
	{
		
		
		
		public int ChangeLifeID { get; set; }

		
		
		
		public int NeedLevel { get; set; }

		
		
		
		public int NeedMoney { get; set; }

		
		
		
		public int NeedMoJing { get; set; }

		
		
		
		public List<GoodsData> NeedGoodsDataList { get; set; }

		
		
		
		public List<GoodsData> AwardGoodsDataList { get; set; }

		
		
		
		public long ExpProportion { get; set; }

		
		public ChangeLifePropertyInfo Propertyinfo = null;
	}
}
