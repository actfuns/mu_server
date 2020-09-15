using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000583 RID: 1411
	[ProtoContract]
	public class QiangGouItemData
	{
		// Token: 0x0400260E RID: 9742
		[ProtoMember(1)]
		public int QiangGouID = 0;

		// Token: 0x0400260F RID: 9743
		[ProtoMember(2)]
		public int Group = 0;

		// Token: 0x04002610 RID: 9744
		[ProtoMember(3)]
		public int ItemID = 0;

		// Token: 0x04002611 RID: 9745
		[ProtoMember(4)]
		public int GoodsID = 0;

		// Token: 0x04002612 RID: 9746
		[ProtoMember(5)]
		public string StartTime = "";

		// Token: 0x04002613 RID: 9747
		[ProtoMember(6)]
		public string EndTime = "";

		// Token: 0x04002614 RID: 9748
		[ProtoMember(7)]
		public int IsTimeOver = 0;

		// Token: 0x04002615 RID: 9749
		[ProtoMember(8)]
		public int SinglePurchase = 0;

		// Token: 0x04002616 RID: 9750
		[ProtoMember(9)]
		public int FullPurchase = 0;

		// Token: 0x04002617 RID: 9751
		[ProtoMember(10)]
		public int FullHasPurchase = 0;

		// Token: 0x04002618 RID: 9752
		[ProtoMember(11)]
		public int SingleHasPurchase = 0;

		// Token: 0x04002619 RID: 9753
		[ProtoMember(12)]
		public int CurrentRoleID = 0;

		// Token: 0x0400261A RID: 9754
		[ProtoMember(13)]
		public int DaysTime = 0;

		// Token: 0x0400261B RID: 9755
		[ProtoMember(14)]
		public int Price = 0;

		// Token: 0x0400261C RID: 9756
		[ProtoMember(15)]
		public int Random = 0;

		// Token: 0x0400261D RID: 9757
		[ProtoMember(16)]
		public int OrigPrice = 0;

		// Token: 0x0400261E RID: 9758
		[ProtoMember(17)]
		public int Type = 0;
	}
}
