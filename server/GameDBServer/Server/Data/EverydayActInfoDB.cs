using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200004C RID: 76
	[ProtoContract]
	public class EverydayActInfoDB
	{
		// Token: 0x0400018C RID: 396
		[ProtoMember(1)]
		public int GroupID = 0;

		// Token: 0x0400018D RID: 397
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x0400018E RID: 398
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x0400018F RID: 399
		[ProtoMember(4)]
		public int CountNum = 0;

		// Token: 0x04000190 RID: 400
		[ProtoMember(5)]
		public int ActiveDay = 0;
	}
}
