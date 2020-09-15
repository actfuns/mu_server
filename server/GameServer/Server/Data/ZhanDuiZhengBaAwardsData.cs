using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200082A RID: 2090
	[ProtoContract]
	public class ZhanDuiZhengBaAwardsData
	{
		// Token: 0x04004553 RID: 17747
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04004554 RID: 17748
		[ProtoMember(2)]
		public int NewGrade;

		// Token: 0x04004555 RID: 17749
		[ProtoMember(3)]
		public string Awards;
	}
}
