using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameDBServer.Logic.Ten
{
	// Token: 0x0200017C RID: 380
	[ProtoContract]
	public class TenAwardData
	{
		// Token: 0x040008AE RID: 2222
		[ProtoMember(1)]
		public int AwardID = 0;

		// Token: 0x040008AF RID: 2223
		[ProtoMember(2)]
		public string AwardName = "";

		// Token: 0x040008B0 RID: 2224
		[ProtoMember(3)]
		public string DbKey = "";

		// Token: 0x040008B1 RID: 2225
		[ProtoMember(4)]
		public int DayMaxNum = 0;

		// Token: 0x040008B2 RID: 2226
		[ProtoMember(5)]
		public int OnlyNum = 0;

		// Token: 0x040008B3 RID: 2227
		[ProtoMember(6)]
		public List<GoodsData> AwardGoods = null;

		// Token: 0x040008B4 RID: 2228
		[ProtoMember(7)]
		public string MailTitle = "";

		// Token: 0x040008B5 RID: 2229
		[ProtoMember(8)]
		public string MailContent = "";

		// Token: 0x040008B6 RID: 2230
		[ProtoMember(9)]
		public int State = 0;

		// Token: 0x040008B7 RID: 2231
		[ProtoMember(10)]
		public int DbID = 0;

		// Token: 0x040008B8 RID: 2232
		[ProtoMember(11)]
		public int RoleID = 0;

		// Token: 0x040008B9 RID: 2233
		[ProtoMember(12)]
		public string MailUser = "";

		// Token: 0x040008BA RID: 2234
		[ProtoMember(13)]
		public DateTime BeginTime = DateTime.MinValue;

		// Token: 0x040008BB RID: 2235
		[ProtoMember(14)]
		public DateTime EndTime = DateTime.MinValue;

		// Token: 0x040008BC RID: 2236
		[ProtoMember(15)]
		public int RoleLevel = 0;

		// Token: 0x040008BD RID: 2237
		[ProtoMember(16)]
		public string UserID = "";
	}
}
