using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000143 RID: 323
	[ProtoContract]
	public class FacebookAwardData
	{
		// Token: 0x0400073E RID: 1854
		[ProtoMember(1)]
		public int AwardID = 0;

		// Token: 0x0400073F RID: 1855
		[ProtoMember(2)]
		public string AwardName = "";

		// Token: 0x04000740 RID: 1856
		[ProtoMember(3)]
		public string DbKey = "";

		// Token: 0x04000741 RID: 1857
		[ProtoMember(4)]
		public int DayMaxNum = 0;

		// Token: 0x04000742 RID: 1858
		[ProtoMember(5)]
		public int OnlyNum = 0;

		// Token: 0x04000743 RID: 1859
		[ProtoMember(6)]
		public List<GoodsData> AwardGoods = null;

		// Token: 0x04000744 RID: 1860
		[ProtoMember(7)]
		public string MailTitle = "";

		// Token: 0x04000745 RID: 1861
		[ProtoMember(8)]
		public string MailContent = "";

		// Token: 0x04000746 RID: 1862
		[ProtoMember(9)]
		public int State = 0;

		// Token: 0x04000747 RID: 1863
		[ProtoMember(10)]
		public int DbID = 0;

		// Token: 0x04000748 RID: 1864
		[ProtoMember(11)]
		public int RoleID = 0;

		// Token: 0x04000749 RID: 1865
		[ProtoMember(12)]
		public string MailUser = "";
	}
}
