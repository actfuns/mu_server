using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C6 RID: 198
	[ProtoContract]
	public class TianTiLogItemData
	{
		// Token: 0x0400054E RID: 1358
		[ProtoMember(1)]
		public int ZoneId1;

		// Token: 0x0400054F RID: 1359
		[ProtoMember(2)]
		public string RoleName1;

		// Token: 0x04000550 RID: 1360
		[ProtoMember(3)]
		public int ZoneId2;

		// Token: 0x04000551 RID: 1361
		[ProtoMember(4)]
		public string RoleName2;

		// Token: 0x04000552 RID: 1362
		[ProtoMember(5)]
		public int Success;

		// Token: 0x04000553 RID: 1363
		[ProtoMember(6)]
		public int DuanWeiJiFenAward;

		// Token: 0x04000554 RID: 1364
		[ProtoMember(7)]
		public int RongYaoAward;

		// Token: 0x04000555 RID: 1365
		[ProtoMember(8)]
		public int RoleId;

		// Token: 0x04000556 RID: 1366
		[ProtoMember(9)]
		public DateTime EndTime;
	}
}
