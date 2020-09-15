using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000BE RID: 190
	[ProtoContract]
	public class TianTi5v5LogItemData
	{
		// Token: 0x0400047B RID: 1147
		[ProtoMember(1)]
		public int ZoneId1;

		// Token: 0x0400047C RID: 1148
		[ProtoMember(2)]
		public string RoleName1;

		// Token: 0x0400047D RID: 1149
		[ProtoMember(3)]
		public int ZoneId2;

		// Token: 0x0400047E RID: 1150
		[ProtoMember(4)]
		public string RoleName2;

		// Token: 0x0400047F RID: 1151
		[ProtoMember(5)]
		public int Success;

		// Token: 0x04000480 RID: 1152
		[ProtoMember(6)]
		public int DuanWeiJiFenAward;

		// Token: 0x04000481 RID: 1153
		[ProtoMember(7)]
		public int RongYaoAward;

		// Token: 0x04000482 RID: 1154
		[ProtoMember(8)]
		public int RoleId;

		// Token: 0x04000483 RID: 1155
		[ProtoMember(9)]
		public DateTime EndTime;
	}
}
