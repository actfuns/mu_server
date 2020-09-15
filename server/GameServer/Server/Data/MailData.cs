using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056F RID: 1391
	[ProtoContract]
	public class MailData
	{
		// Token: 0x0400256B RID: 9579
		[ProtoMember(1)]
		public int MailID = 0;

		// Token: 0x0400256C RID: 9580
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x0400256D RID: 9581
		[ProtoMember(3)]
		public string SenderRName = "";

		// Token: 0x0400256E RID: 9582
		[ProtoMember(4)]
		public string SendTime = "";

		// Token: 0x0400256F RID: 9583
		[ProtoMember(5)]
		public int ReceiverRID = 0;

		// Token: 0x04002570 RID: 9584
		[ProtoMember(6)]
		public string ReveiverRName = "";

		// Token: 0x04002571 RID: 9585
		[ProtoMember(7)]
		public string ReadTime = "1900-01-01 12:00:00";

		// Token: 0x04002572 RID: 9586
		[ProtoMember(8)]
		public int IsRead = 0;

		// Token: 0x04002573 RID: 9587
		[ProtoMember(9)]
		public int MailType = 0;

		// Token: 0x04002574 RID: 9588
		[ProtoMember(10)]
		public int Hasfetchattachment = 0;

		// Token: 0x04002575 RID: 9589
		[ProtoMember(11)]
		public string Subject = "";

		// Token: 0x04002576 RID: 9590
		[ProtoMember(12)]
		public string Content = "";

		// Token: 0x04002577 RID: 9591
		[ProtoMember(13)]
		public int Yinliang = 0;

		// Token: 0x04002578 RID: 9592
		[ProtoMember(14)]
		public int Tongqian = 0;

		// Token: 0x04002579 RID: 9593
		[ProtoMember(15)]
		public int YuanBao = 0;

		// Token: 0x0400257A RID: 9594
		[ProtoMember(16)]
		public List<MailGoodsData> GoodsList = null;
	}
}
