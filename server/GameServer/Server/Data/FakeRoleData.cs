using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000552 RID: 1362
	[ProtoContract]
	public class FakeRoleData
	{
		// Token: 0x0400248E RID: 9358
		[ProtoMember(1)]
		public int FakeRoleID = 0;

		// Token: 0x0400248F RID: 9359
		[ProtoMember(2)]
		public int FakeRoleType = 0;

		// Token: 0x04002490 RID: 9360
		[ProtoMember(3)]
		public int ToExtensionID = 0;

		// Token: 0x04002491 RID: 9361
		[ProtoMember(4)]
		public RoleDataMini MyRoleDataMini = null;
	}
}
