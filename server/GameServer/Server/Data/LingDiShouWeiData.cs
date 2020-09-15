using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200030C RID: 780
	[ProtoContract]
	public class LingDiShouWeiData
	{
		// Token: 0x0400140E RID: 5134
		[ProtoMember(1)]
		public int State;

		// Token: 0x0400140F RID: 5135
		[ProtoMember(2)]
		public DateTime FreeBuShuTime;

		// Token: 0x04001410 RID: 5136
		[ProtoMember(3)]
		public int ZuanShiCost;
	}
}
