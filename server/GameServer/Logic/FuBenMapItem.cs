using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006C3 RID: 1731
	public class FuBenMapItem
	{
		// Token: 0x04003690 RID: 13968
		public int FuBenID = 0;

		// Token: 0x04003691 RID: 13969
		public int MapCode = 0;

		// Token: 0x04003692 RID: 13970
		public int MaxTime = -1;

		// Token: 0x04003693 RID: 13971
		public int MinSaoDangTimer = -1;

		// Token: 0x04003694 RID: 13972
		public int Money1 = 0;

		// Token: 0x04003695 RID: 13973
		public int Experience = 0;

		// Token: 0x04003696 RID: 13974
		public int nFirstGold = 0;

		// Token: 0x04003697 RID: 13975
		public int nFirstExp = 0;

		// Token: 0x04003698 RID: 13976
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x04003699 RID: 13977
		public List<GoodsData> FirstGoodsDataList = null;

		// Token: 0x0400369A RID: 13978
		public int nXingHunAward = 0;

		// Token: 0x0400369B RID: 13979
		public int nFirstXingHunAward = 0;

		// Token: 0x0400369C RID: 13980
		public int nZhanGongaward = 0;

		// Token: 0x0400369D RID: 13981
		public int YuanSuFenMoaward = 0;

		// Token: 0x0400369E RID: 13982
		public int LightAward = 0;

		// Token: 0x0400369F RID: 13983
		public int WolfMoney = 0;
	}
}
