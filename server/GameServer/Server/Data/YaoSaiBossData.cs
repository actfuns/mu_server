using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F2 RID: 2034
	[ProtoContract]
	public class YaoSaiBossData
	{
		// Token: 0x04004381 RID: 17281
		[ProtoMember(1)]
		public int BossID;

		// Token: 0x04004382 RID: 17282
		[ProtoMember(2)]
		public double LifeV;

		// Token: 0x04004383 RID: 17283
		[ProtoMember(3)]
		public DateTime DeadTime;

		// Token: 0x04004384 RID: 17284
		[ProtoMember(4)]
		public int OwnerID;
	}
}
