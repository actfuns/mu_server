using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000555 RID: 1365
	[ProtoContract]
	public class FuBenHistData
	{
		// Token: 0x040024A5 RID: 9381
		[ProtoMember(1)]
		public int FuBenID = 0;

		// Token: 0x040024A6 RID: 9382
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040024A7 RID: 9383
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x040024A8 RID: 9384
		[ProtoMember(4)]
		public int UsedSecs = 0;
	}
}
