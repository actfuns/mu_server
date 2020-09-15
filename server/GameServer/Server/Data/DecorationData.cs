using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054B RID: 1355
	[ProtoContract]
	public class DecorationData
	{
		// Token: 0x0400245E RID: 9310
		[ProtoMember(1)]
		public int AutoID = 0;

		// Token: 0x0400245F RID: 9311
		[ProtoMember(2)]
		public int DecoID = 0;

		// Token: 0x04002460 RID: 9312
		[ProtoMember(3)]
		public int PosX = 0;

		// Token: 0x04002461 RID: 9313
		[ProtoMember(4)]
		public int PosY = 0;

		// Token: 0x04002462 RID: 9314
		[ProtoMember(5)]
		public int MapCode = -1;

		// Token: 0x04002463 RID: 9315
		[ProtoMember(6)]
		public long StartTicks = 0L;

		// Token: 0x04002464 RID: 9316
		[ProtoMember(7)]
		public int MaxLiveTicks = 0;

		// Token: 0x04002465 RID: 9317
		[ProtoMember(8)]
		public int AlphaTicks = 0;
	}
}
