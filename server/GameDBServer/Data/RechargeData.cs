using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	// Token: 0x02000069 RID: 105
	[ProtoContract]
	public class RechargeData
	{
		// Token: 0x0400023C RID: 572
		[ProtoMember(1)]
		public string UserID = "";

		// Token: 0x0400023D RID: 573
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x0400023E RID: 574
		[ProtoMember(3)]
		public int Amount = 0;

		// Token: 0x0400023F RID: 575
		[ProtoMember(4)]
		public int ItemId = 0;

		// Token: 0x04000240 RID: 576
		[ProtoMember(5)]
		public int BudanFlag = 0;

		// Token: 0x04000241 RID: 577
		[ProtoMember(6)]
		public string ChargeTime = "";

		// Token: 0x04000242 RID: 578
		[ProtoMember(7)]
		public string order_no = "";

		// Token: 0x04000243 RID: 579
		[ProtoMember(8)]
		public string cc = "";

		// Token: 0x04000244 RID: 580
		[ProtoMember(9)]
		public string cporder_no = "";

		// Token: 0x04000245 RID: 581
		[ProtoMember(10)]
		public string Sign = "";

		// Token: 0x04000246 RID: 582
		[ProtoMember(11)]
		public long Time = 0L;

		// Token: 0x04000247 RID: 583
		[ProtoMember(12)]
		public int ZoneID = 0;

		// Token: 0x04000248 RID: 584
		[ProtoMember(13)]
		public int Id = 0;
	}
}
