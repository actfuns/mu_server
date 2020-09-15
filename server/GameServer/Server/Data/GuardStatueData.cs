using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000146 RID: 326
	[ProtoContract]
	public class GuardStatueData
	{
		// Token: 0x04000750 RID: 1872
		[ProtoMember(1, IsRequired = true)]
		public int Level = 0;

		// Token: 0x04000751 RID: 1873
		[ProtoMember(2, IsRequired = true)]
		public int Suit = 1;

		// Token: 0x04000752 RID: 1874
		[ProtoMember(3, IsRequired = true)]
		public int HasGuardPoint = 0;

		// Token: 0x04000753 RID: 1875
		[ProtoMember(4, IsRequired = true)]
		public List<GuardSoulData> GuardSoulList = new List<GuardSoulData>();
	}
}
