using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000060 RID: 96
	[ProtoContract]
	public class HeFuPKKingData
	{
		// Token: 0x04000218 RID: 536
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000219 RID: 537
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x0400021A RID: 538
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x0400021B RID: 539
		[ProtoMember(4)]
		public int State = 0;
	}
}
