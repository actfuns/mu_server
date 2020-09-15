using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000090 RID: 144
	[ProtoContract]
	public class MailData
	{
		// Token: 0x04000334 RID: 820
		[ProtoMember(1)]
		public int MailID = 0;

		// Token: 0x04000335 RID: 821
		[ProtoMember(2)]
		public int SenderRID = 0;

		// Token: 0x04000336 RID: 822
		[ProtoMember(3)]
		public string SenderRName = "";

		// Token: 0x04000337 RID: 823
		[ProtoMember(4)]
		public string SendTime = "";

		// Token: 0x04000338 RID: 824
		[ProtoMember(5)]
		public int ReceiverRID = 0;

		// Token: 0x04000339 RID: 825
		[ProtoMember(6)]
		public string ReveiverRName = "";

		// Token: 0x0400033A RID: 826
		[ProtoMember(7)]
		public string ReadTime = "1900-01-01 12:00:00";

		// Token: 0x0400033B RID: 827
		[ProtoMember(8)]
		public int IsRead = 0;

		// Token: 0x0400033C RID: 828
		[ProtoMember(9)]
		public int MailType = 0;

		// Token: 0x0400033D RID: 829
		[ProtoMember(10)]
		public int Hasfetchattachment = 0;

		// Token: 0x0400033E RID: 830
		[ProtoMember(11)]
		public string Subject = "";

		// Token: 0x0400033F RID: 831
		[ProtoMember(12)]
		public string Content = "";

		// Token: 0x04000340 RID: 832
		[ProtoMember(13)]
		public int Yinliang = 0;

		// Token: 0x04000341 RID: 833
		[ProtoMember(14)]
		public int Tongqian = 0;

		// Token: 0x04000342 RID: 834
		[ProtoMember(15)]
		public int YuanBao = 0;

		// Token: 0x04000343 RID: 835
		[ProtoMember(16)]
		public List<MailGoodsData> GoodsList = null;
	}
}
