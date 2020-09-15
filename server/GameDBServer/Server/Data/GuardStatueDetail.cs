using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005E RID: 94
	[ProtoContract]
	public class GuardStatueDetail
	{
		// Token: 0x04000209 RID: 521
		[ProtoMember(1, IsRequired = true)]
		public bool IsActived = false;

		// Token: 0x0400020A RID: 522
		[ProtoMember(2, IsRequired = true)]
		public int LastdayRecoverPoint = 0;

		// Token: 0x0400020B RID: 523
		[ProtoMember(3, IsRequired = true)]
		public int LastdayRecoverOffset = 0;

		// Token: 0x0400020C RID: 524
		[ProtoMember(4, IsRequired = true)]
		public int ActiveSoulSlot = 0;

		// Token: 0x0400020D RID: 525
		[ProtoMember(5, IsRequired = true)]
		public GuardStatueData GuardStatue = new GuardStatueData();
	}
}
