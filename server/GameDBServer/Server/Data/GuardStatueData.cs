using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005D RID: 93
	[ProtoContract]
	public class GuardStatueData
	{
		// Token: 0x04000205 RID: 517
		[ProtoMember(1, IsRequired = true)]
		public int Level = 0;

		// Token: 0x04000206 RID: 518
		[ProtoMember(2, IsRequired = true)]
		public int Suit = 1;

		// Token: 0x04000207 RID: 519
		[ProtoMember(3, IsRequired = true)]
		public int HasGuardPoint = 0;

		// Token: 0x04000208 RID: 520
		[ProtoMember(4, IsRequired = true)]
		public List<GuardSoulData> GuardSoulList = new List<GuardSoulData>();
	}
}
