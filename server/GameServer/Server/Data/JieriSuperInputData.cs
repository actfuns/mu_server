using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000129 RID: 297
	[ProtoContract]
	public class JieriSuperInputData
	{
		// Token: 0x04000669 RID: 1641
		[ProtoMember(1)]
		public int ID;

		// Token: 0x0400066A RID: 1642
		[ProtoMember(2)]
		public int MutiNum;

		// Token: 0x0400066B RID: 1643
		[ProtoMember(3)]
		public int PurchaseNum;

		// Token: 0x0400066C RID: 1644
		[ProtoMember(4)]
		public int FullPurchaseNum;

		// Token: 0x0400066D RID: 1645
		[ProtoMember(5)]
		public DateTime BeginTime;

		// Token: 0x0400066E RID: 1646
		[ProtoMember(6)]
		public DateTime EndTime;
	}
}
