using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DB RID: 219
	[ProtoContract]
	public class PlatGiftData
	{
		// Token: 0x04000608 RID: 1544
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000609 RID: 1545
		[ProtoMember(2)]
		public string UserID = null;

		// Token: 0x0400060A RID: 1546
		[ProtoMember(3)]
		public int Type = 0;

		// Token: 0x0400060B RID: 1547
		[ProtoMember(4)]
		public int ID = 0;

		// Token: 0x0400060C RID: 1548
		[ProtoMember(5)]
		public string ExtraData = null;

		// Token: 0x0400060D RID: 1549
		[ProtoMember(6)]
		public string Message = null;

		// Token: 0x0400060E RID: 1550
		[ProtoMember(7)]
		public long Time = 0L;

		// Token: 0x0400060F RID: 1551
		[ProtoMember(8)]
		public string RoleName = null;

		// Token: 0x04000610 RID: 1552
		[ProtoMember(9)]
		public string Sign = null;
	}
}
