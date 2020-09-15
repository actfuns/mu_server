using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Ten
{
	// Token: 0x02000431 RID: 1073
	[ProtoContract]
	public class TenAwardData
	{
		// Token: 0x04001CFC RID: 7420
		[ProtoMember(1)]
		public int AwardID = 0;

		// Token: 0x04001CFD RID: 7421
		[ProtoMember(2)]
		public string AwardName = "";

		// Token: 0x04001CFE RID: 7422
		[ProtoMember(3)]
		public string DbKey = "";

		// Token: 0x04001CFF RID: 7423
		[ProtoMember(4)]
		public int DayMaxNum = 0;

		// Token: 0x04001D00 RID: 7424
		[ProtoMember(5)]
		public int OnlyNum = 0;

		// Token: 0x04001D01 RID: 7425
		[ProtoMember(6)]
		public List<GoodsData> AwardGoods = null;

		// Token: 0x04001D02 RID: 7426
		[ProtoMember(7)]
		public string MailTitle = "";

		// Token: 0x04001D03 RID: 7427
		[ProtoMember(8)]
		public string MailContent = "";

		// Token: 0x04001D04 RID: 7428
		[ProtoMember(9)]
		public int State = 0;

		// Token: 0x04001D05 RID: 7429
		[ProtoMember(10)]
		public int DbID = 0;

		// Token: 0x04001D06 RID: 7430
		[ProtoMember(11)]
		public int RoleID = 0;

		// Token: 0x04001D07 RID: 7431
		[ProtoMember(12)]
		public string MailUser = "";

		// Token: 0x04001D08 RID: 7432
		[ProtoMember(13)]
		public DateTime BeginTime = DateTime.MinValue;

		// Token: 0x04001D09 RID: 7433
		[ProtoMember(14)]
		public DateTime EndTime = DateTime.MinValue;

		// Token: 0x04001D0A RID: 7434
		[ProtoMember(15)]
		public int RoleLevel = 0;

		// Token: 0x04001D0B RID: 7435
		[ProtoMember(16)]
		public string UserID = "";
	}
}
