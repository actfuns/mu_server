using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000491 RID: 1169
	[ProtoContract]
	public class TianTiLogItemData
	{
		// Token: 0x04001EEE RID: 7918
		[ProtoMember(1)]
		public int ZoneId1;

		// Token: 0x04001EEF RID: 7919
		[ProtoMember(2)]
		public string RoleName1;

		// Token: 0x04001EF0 RID: 7920
		[ProtoMember(3)]
		public int ZoneId2;

		// Token: 0x04001EF1 RID: 7921
		[ProtoMember(4)]
		public string RoleName2;

		// Token: 0x04001EF2 RID: 7922
		[ProtoMember(5)]
		public int Success;

		// Token: 0x04001EF3 RID: 7923
		[ProtoMember(6)]
		public int DuanWeiJiFenAward;

		// Token: 0x04001EF4 RID: 7924
		[ProtoMember(7)]
		public int RongYaoAward;

		// Token: 0x04001EF5 RID: 7925
		[ProtoMember(8)]
		public int RoleId;

		// Token: 0x04001EF6 RID: 7926
		[ProtoMember(9)]
		public DateTime EndTime;
	}
}
