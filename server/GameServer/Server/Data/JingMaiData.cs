using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000564 RID: 1380
	[ProtoContract]
	public class JingMaiData
	{
		// Token: 0x04002530 RID: 9520
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x04002531 RID: 9521
		[ProtoMember(2)]
		public int JingMaiID = 0;

		// Token: 0x04002532 RID: 9522
		[ProtoMember(3)]
		public int JingMaiLevel = 0;

		// Token: 0x04002533 RID: 9523
		[ProtoMember(4)]
		public int JingMaiBodyLevel = 0;
	}
}
