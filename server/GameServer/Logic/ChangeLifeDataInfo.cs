using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200053A RID: 1338
	public class ChangeLifeDataInfo
	{
		// Token: 0x17000069 RID: 105
		
		
		public int ChangeLifeID { get; set; }

		// Token: 0x1700006A RID: 106
		
		
		public int NeedLevel { get; set; }

		// Token: 0x1700006B RID: 107
		
		
		public int NeedMoney { get; set; }

		// Token: 0x1700006C RID: 108
		
		
		public int NeedMoJing { get; set; }

		// Token: 0x1700006D RID: 109
		
		
		public List<GoodsData> NeedGoodsDataList { get; set; }

		// Token: 0x1700006E RID: 110
		
		
		public List<GoodsData> AwardGoodsDataList { get; set; }

		// Token: 0x1700006F RID: 111
		
		
		public long ExpProportion { get; set; }

		// Token: 0x0400239A RID: 9114
		public ChangeLifePropertyInfo Propertyinfo = null;
	}
}
