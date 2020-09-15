using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameDBServer.Data
{
	// Token: 0x02000053 RID: 83
	[ProtoContract]
	public class FacebookAwardData
	{
		// Token: 0x040001AF RID: 431
		[ProtoMember(1)]
		public int AwardID = 0;

		// Token: 0x040001B0 RID: 432
		[ProtoMember(2)]
		public string AwardName = "";

		// Token: 0x040001B1 RID: 433
		[ProtoMember(3)]
		public string DbKey = "";

		// Token: 0x040001B2 RID: 434
		[ProtoMember(4)]
		public int DayMaxNum = 0;

		// Token: 0x040001B3 RID: 435
		[ProtoMember(5)]
		public int OnlyNum = 0;

		// Token: 0x040001B4 RID: 436
		[ProtoMember(6)]
		public List<GoodsData> AwardGoods = null;

		// Token: 0x040001B5 RID: 437
		[ProtoMember(7)]
		public string MailTitle = "";

		// Token: 0x040001B6 RID: 438
		[ProtoMember(8)]
		public string MailContent = "";

		// Token: 0x040001B7 RID: 439
		[ProtoMember(9)]
		public int State = 0;

		// Token: 0x040001B8 RID: 440
		[ProtoMember(10)]
		public int DbID = 0;

		// Token: 0x040001B9 RID: 441
		[ProtoMember(11)]
		public int RoleID = 0;

		// Token: 0x040001BA RID: 442
		[ProtoMember(12)]
		public string MailUser = "";
	}
}
