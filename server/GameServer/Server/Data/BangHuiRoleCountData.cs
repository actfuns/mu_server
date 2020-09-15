using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C0 RID: 960
	[ProtoContract]
	public class BangHuiRoleCountData
	{
		// Token: 0x0400191B RID: 6427
		[ProtoMember(1)]
		public int BHID;

		// Token: 0x0400191C RID: 6428
		[ProtoMember(2)]
		public int RoleCount;

		// Token: 0x0400191D RID: 6429
		[ProtoMember(3)]
		public int ServerID;
	}
}
