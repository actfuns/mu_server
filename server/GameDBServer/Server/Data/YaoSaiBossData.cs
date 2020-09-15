using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000191 RID: 401
	[ProtoContract]
	public class YaoSaiBossData
	{
		// Token: 0x0400092D RID: 2349
		[ProtoMember(1)]
		public int BossID;

		// Token: 0x0400092E RID: 2350
		[ProtoMember(2)]
		public double LifeV;

		// Token: 0x0400092F RID: 2351
		[ProtoMember(3)]
		public DateTime DeadTime;

		// Token: 0x04000930 RID: 2352
		[ProtoMember(4)]
		public int OwnerID;
	}
}
