using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200016D RID: 365
	[ProtoContract]
	internal class MonsterRealiveData
	{
		// Token: 0x0400081E RID: 2078
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400081F RID: 2079
		[ProtoMember(2)]
		public int PosX = 0;

		// Token: 0x04000820 RID: 2080
		[ProtoMember(3)]
		public int PosY = 0;

		// Token: 0x04000821 RID: 2081
		[ProtoMember(4)]
		public int Direction = 0;
	}
}
