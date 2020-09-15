using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020002B5 RID: 693
	[ProtoContract]
	public class ElementWarAwardsData
	{
		// Token: 0x040011AE RID: 4526
		[ProtoMember(1)]
		public long Exp;

		// Token: 0x040011AF RID: 4527
		[ProtoMember(2)]
		public int Money;

		// Token: 0x040011B0 RID: 4528
		[ProtoMember(3)]
		public int Light;

		// Token: 0x040011B1 RID: 4529
		[ProtoMember(4)]
		public int Wave;

		// Token: 0x040011B2 RID: 4530
		[ProtoMember(5)]
		public int ysfm;
	}
}
