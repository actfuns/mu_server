using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200000E RID: 14
	[ProtoContract]
	public class FuMoMailData
	{
		// Token: 0x0400005B RID: 91
		[ProtoMember(1)]
		public int MaillID = 0;

		// Token: 0x0400005C RID: 92
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x0400005D RID: 93
		[ProtoMember(3)]
		public string SenderRName = "";

		// Token: 0x0400005E RID: 94
		[ProtoMember(4)]
		public int SenderJob = 0;

		// Token: 0x0400005F RID: 95
		[ProtoMember(5)]
		public string SendTime = "";

		// Token: 0x04000060 RID: 96
		[ProtoMember(6)]
		public int ReceiverRID = 0;

		// Token: 0x04000061 RID: 97
		[ProtoMember(7)]
		public int IsRead = 0;

		// Token: 0x04000062 RID: 98
		[ProtoMember(8)]
		public string ReadTime = "1900-01-01 12:00:00";

		// Token: 0x04000063 RID: 99
		[ProtoMember(9)]
		public int FuMoMoney = 0;

		// Token: 0x04000064 RID: 100
		[ProtoMember(10)]
		public string Content = "";
	}
}
