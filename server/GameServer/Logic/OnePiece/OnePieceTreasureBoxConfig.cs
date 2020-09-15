using System;

namespace GameServer.Logic.OnePiece
{
	// Token: 0x020003A6 RID: 934
	public class OnePieceTreasureBoxConfig
	{
		// Token: 0x0400188D RID: 6285
		public int ID = 0;

		// Token: 0x0400188E RID: 6286
		public TeasureBoxType Type = TeasureBoxType.ETBT_Null;

		// Token: 0x0400188F RID: 6287
		public AwardsItemList Goods = new AwardsItemList();

		// Token: 0x04001890 RID: 6288
		public int Num = 0;

		// Token: 0x04001891 RID: 6289
		public int BeginNum = 0;

		// Token: 0x04001892 RID: 6290
		public int EndNum = 0;
	}
}
