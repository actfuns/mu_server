using System;
using System.Collections.Generic;

namespace GameServer.Logic.OnePiece
{
	// Token: 0x020003A5 RID: 933
	public class OnePieceTreasureEventConfig
	{
		// Token: 0x04001882 RID: 6274
		public int ID = 0;

		// Token: 0x04001883 RID: 6275
		public TreasureEventType Type = TreasureEventType.ETET_Null;

		// Token: 0x04001884 RID: 6276
		public AwardsItemList GoodsList = new AwardsItemList();

		// Token: 0x04001885 RID: 6277
		public OnePieceMoneyPair NewValue = new OnePieceMoneyPair();

		// Token: 0x04001886 RID: 6278
		public int NewDiec = 0;

		// Token: 0x04001887 RID: 6279
		public int NewSuperDiec = 0;

		// Token: 0x04001888 RID: 6280
		public List<OnePieceGoodsPair> NeedGoods = new List<OnePieceGoodsPair>();

		// Token: 0x04001889 RID: 6281
		public OnePieceMoneyPair NeedValue = new OnePieceMoneyPair();

		// Token: 0x0400188A RID: 6282
		public List<int> MoveRange = new List<int>();

		// Token: 0x0400188B RID: 6283
		public int FuBenID = 0;

		// Token: 0x0400188C RID: 6284
		public List<OnePieceTreasureBoxPair> BoxList = new List<OnePieceTreasureBoxPair>();
	}
}
