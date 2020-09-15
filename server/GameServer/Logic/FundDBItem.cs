using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020002BE RID: 702
	[ProtoContract]
	public class FundDBItem
	{
		// Token: 0x04001216 RID: 4630
		[ProtoMember(1)]
		public int zoneID = 0;

		// Token: 0x04001217 RID: 4631
		[ProtoMember(2)]
		public string UserID = "";

		// Token: 0x04001218 RID: 4632
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x04001219 RID: 4633
		[ProtoMember(4)]
		public int FundType = 0;

		// Token: 0x0400121A RID: 4634
		[ProtoMember(5)]
		public int FundID = 0;

		// Token: 0x0400121B RID: 4635
		[ProtoMember(6)]
		public DateTime BuyTime = DateTime.MinValue;

		// Token: 0x0400121C RID: 4636
		[ProtoMember(7)]
		public int AwardID = 0;

		// Token: 0x0400121D RID: 4637
		[ProtoMember(8)]
		public int Value1 = 0;

		// Token: 0x0400121E RID: 4638
		[ProtoMember(9)]
		public int Value2 = 0;

		// Token: 0x0400121F RID: 4639
		[ProtoMember(10)]
		public int State = 0;
	}
}
