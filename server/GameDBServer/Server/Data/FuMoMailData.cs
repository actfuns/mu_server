using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000057 RID: 87
	[ProtoContract]
	public class FuMoMailData
	{
		// Token: 0x040001D2 RID: 466
		[ProtoMember(1)]
		public int MaillID = 0;

		// Token: 0x040001D3 RID: 467
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x040001D4 RID: 468
		[ProtoMember(3)]
		public string SenderRName = "";

		// Token: 0x040001D5 RID: 469
		[ProtoMember(4)]
		public int SenderJob = 0;

		// Token: 0x040001D6 RID: 470
		[ProtoMember(5)]
		public string SendTime = "";

		// Token: 0x040001D7 RID: 471
		[ProtoMember(6)]
		public int ReceiverRID = 0;

		// Token: 0x040001D8 RID: 472
		[ProtoMember(7)]
		public int IsRead = 0;

		// Token: 0x040001D9 RID: 473
		[ProtoMember(8)]
		public string ReadTime = "1900-01-01 12:00:00";

		// Token: 0x040001DA RID: 474
		[ProtoMember(9)]
		public int FuMoMoney = 0;

		// Token: 0x040001DB RID: 475
		[ProtoMember(10)]
		public string Content = "";
	}
}
