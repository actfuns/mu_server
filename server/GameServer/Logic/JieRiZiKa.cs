using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200070B RID: 1803
	public class JieRiZiKa
	{
		// Token: 0x04003A3C RID: 14908
		public int type;

		// Token: 0x04003A3D RID: 14909
		public int id;

		// Token: 0x04003A3E RID: 14910
		public int DayMaxTimes = 0;

		// Token: 0x04003A3F RID: 14911
		public List<GoodsData> NeedGoodsList = null;

		// Token: 0x04003A40 RID: 14912
		public int NeedMoJing;

		// Token: 0x04003A41 RID: 14913
		public int NeedQiFuJiFen;

		// Token: 0x04003A42 RID: 14914
		public int NeedPetJiFen;

		// Token: 0x04003A43 RID: 14915
		public AwardItem MyAwardItem = new AwardItem();
	}
}
