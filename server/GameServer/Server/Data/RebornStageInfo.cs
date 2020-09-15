using System;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x020003E3 RID: 995
	public class RebornStageInfo
	{
		// Token: 0x04001A72 RID: 6770
		public int ID;

		// Token: 0x04001A73 RID: 6771
		public int[] NeedZhuanSheng;

		// Token: 0x04001A74 RID: 6772
		public int NeedRebornLevel;

		// Token: 0x04001A75 RID: 6773
		public int NeedZhanLi;

		// Token: 0x04001A76 RID: 6774
		public double[] NeedMaxWing;

		// Token: 0x04001A77 RID: 6775
		public int NeedChengJie;

		// Token: 0x04001A78 RID: 6776
		public int NeedShengWang;

		// Token: 0x04001A79 RID: 6777
		public int[] NeedMagicBook;

		// Token: 0x04001A7A RID: 6778
		public int MaxRebornLevel;

		// Token: 0x04001A7B RID: 6779
		public int RebornPoint;

		// Token: 0x04001A7C RID: 6780
		public double[][] extProps = new double[6][];

		// Token: 0x04001A7D RID: 6781
		public AwardsItemList AwardGoods = new AwardsItemList();
	}
}
