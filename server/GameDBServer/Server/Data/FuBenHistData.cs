using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000056 RID: 86
	[ProtoContract]
	public class FuBenHistData
	{
		// Token: 0x040001CE RID: 462
		[ProtoMember(1)]
		public int FuBenID = 0;

		// Token: 0x040001CF RID: 463
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040001D0 RID: 464
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x040001D1 RID: 465
		[ProtoMember(4)]
		public int UsedSecs = 0;
	}
}
