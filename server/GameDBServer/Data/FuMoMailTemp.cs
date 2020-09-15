using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	// Token: 0x02000058 RID: 88
	[ProtoContract]
	public class FuMoMailTemp
	{
		// Token: 0x040001DC RID: 476
		[ProtoMember(1)]
		public int TodayID = 0;

		// Token: 0x040001DD RID: 477
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x040001DE RID: 478
		[ProtoMember(3)]
		public string ReceiverRID = "";

		// Token: 0x040001DF RID: 479
		[ProtoMember(4)]
		public int Accept = 0;

		// Token: 0x040001E0 RID: 480
		[ProtoMember(5)]
		public int Give = 0;
	}
}
