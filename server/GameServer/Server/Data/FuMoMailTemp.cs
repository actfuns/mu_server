using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000010 RID: 16
	[ProtoContract]
	public class FuMoMailTemp
	{
		// Token: 0x04000069 RID: 105
		[ProtoMember(1)]
		public int TodayID = 0;

		// Token: 0x0400006A RID: 106
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x0400006B RID: 107
		[ProtoMember(3)]
		public string ReceiverRID = "";

		// Token: 0x0400006C RID: 108
		[ProtoMember(4)]
		public int Accept = 0;

		// Token: 0x0400006D RID: 109
		[ProtoMember(5)]
		public int Give = 0;
	}
}
