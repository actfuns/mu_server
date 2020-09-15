using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000806 RID: 2054
	[ProtoContract]
	public class PrisonFuLuData
	{
		// Token: 0x040043FB RID: 17403
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040043FC RID: 17404
		[ProtoMember(2)]
		public string Name;

		// Token: 0x040043FD RID: 17405
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x040043FE RID: 17406
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		// Token: 0x040043FF RID: 17407
		[ProtoMember(5)]
		public int ZoneID = 0;

		// Token: 0x04004400 RID: 17408
		[ProtoMember(6)]
		public int LaoDongState = 0;

		// Token: 0x04004401 RID: 17409
		[ProtoMember(7)]
		public long LaoDongTime = 0L;

		// Token: 0x04004402 RID: 17410
		[ProtoMember(8)]
		public int Occupation = 0;

		// Token: 0x04004403 RID: 17411
		[ProtoMember(9)]
		public int RoleSex = 0;
	}
}
