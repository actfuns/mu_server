using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	// Token: 0x02000059 RID: 89
	[ProtoContract]
	public class GiftCodeAwardData
	{
		// Token: 0x040001E1 RID: 481
		[ProtoMember(1)]
		public int Dbid = 0;

		// Token: 0x040001E2 RID: 482
		[ProtoMember(2)]
		public string UserId = "";

		// Token: 0x040001E3 RID: 483
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x040001E4 RID: 484
		[ProtoMember(4)]
		public string GiftId = "";

		// Token: 0x040001E5 RID: 485
		[ProtoMember(5)]
		public string CodeNo = "";
	}
}
