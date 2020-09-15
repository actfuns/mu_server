using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000014 RID: 20
	[ProtoContract]
	public class MazingerStoreData
	{
		// Token: 0x0400007A RID: 122
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400007B RID: 123
		[ProtoMember(2)]
		public int Type = 0;

		// Token: 0x0400007C RID: 124
		[ProtoMember(3)]
		public int Stage = 0;

		// Token: 0x0400007D RID: 125
		[ProtoMember(4)]
		public int StarLevel = 0;

		// Token: 0x0400007E RID: 126
		[ProtoMember(5)]
		public int Exp = 0;
	}
}
