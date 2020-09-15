using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000147 RID: 327
	[ProtoContract]
	public class GuardStatueDetail
	{
		// Token: 0x04000754 RID: 1876
		[ProtoMember(1, IsRequired = true)]
		public bool IsActived = false;

		// Token: 0x04000755 RID: 1877
		[ProtoMember(2, IsRequired = true)]
		public int LastdayRecoverPoint = 0;

		// Token: 0x04000756 RID: 1878
		[ProtoMember(3, IsRequired = true)]
		public int LastdayRecoverOffset = 0;

		// Token: 0x04000757 RID: 1879
		[ProtoMember(4, IsRequired = true)]
		public int ActiveSoulSlot = 0;

		// Token: 0x04000758 RID: 1880
		[ProtoMember(5, IsRequired = true)]
		public GuardStatueData GuardStatue = new GuardStatueData();
	}
}
