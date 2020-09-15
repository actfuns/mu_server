using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D2 RID: 210
	[ProtoContract]
	public class QiangGouItemData
	{
		// Token: 0x040005B5 RID: 1461
		[ProtoMember(1)]
		public int QiangGouID = 0;

		// Token: 0x040005B6 RID: 1462
		[ProtoMember(2)]
		public int Group = 0;

		// Token: 0x040005B7 RID: 1463
		[ProtoMember(3)]
		public int ItemID = 0;

		// Token: 0x040005B8 RID: 1464
		[ProtoMember(4)]
		public int GoodsID = 0;

		// Token: 0x040005B9 RID: 1465
		[ProtoMember(5)]
		public string StartTime = "";

		// Token: 0x040005BA RID: 1466
		[ProtoMember(6)]
		public string EndTime = "";

		// Token: 0x040005BB RID: 1467
		[ProtoMember(7)]
		public int IsTimeOver = 0;

		// Token: 0x040005BC RID: 1468
		[ProtoMember(8)]
		public int SinglePurchase = 0;

		// Token: 0x040005BD RID: 1469
		[ProtoMember(9)]
		public int FullPurchase = 0;

		// Token: 0x040005BE RID: 1470
		[ProtoMember(10)]
		public int FullHasPurchase = 0;

		// Token: 0x040005BF RID: 1471
		[ProtoMember(11)]
		public int SingleHasPurchase = 0;

		// Token: 0x040005C0 RID: 1472
		[ProtoMember(12)]
		public int CurrentRoleID = 0;

		// Token: 0x040005C1 RID: 1473
		[ProtoMember(13)]
		public int DaysTime = 0;

		// Token: 0x040005C2 RID: 1474
		[ProtoMember(14)]
		public int Price = 0;

		// Token: 0x040005C3 RID: 1475
		[ProtoMember(15)]
		public int Random = 0;

		// Token: 0x040005C4 RID: 1476
		[ProtoMember(16)]
		public int OrigPrice = 0;
	}
}
