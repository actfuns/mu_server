using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200003F RID: 63
	[ProtoContract]
	public class JieriSuperInputData
	{
		// Token: 0x04000145 RID: 325
		[ProtoMember(1)]
		public int ID;

		// Token: 0x04000146 RID: 326
		[ProtoMember(2)]
		public int MutiNum;

		// Token: 0x04000147 RID: 327
		[ProtoMember(3)]
		public int PurchaseNum;

		// Token: 0x04000148 RID: 328
		[ProtoMember(4)]
		public int FullPurchaseNum;

		// Token: 0x04000149 RID: 329
		[ProtoMember(5)]
		public DateTime BeginTime;

		// Token: 0x0400014A RID: 330
		[ProtoMember(6)]
		public DateTime EndTime;
	}
}
