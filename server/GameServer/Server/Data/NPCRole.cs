using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AC RID: 1452
	[ProtoContract]
	public class NPCRole
	{
		// Token: 0x040028E9 RID: 10473
		[ProtoMember(1)]
		public int NpcID = 0;

		// Token: 0x040028EA RID: 10474
		[ProtoMember(2)]
		public int PosX = 0;

		// Token: 0x040028EB RID: 10475
		[ProtoMember(3)]
		public int PosY = 0;

		// Token: 0x040028EC RID: 10476
		[ProtoMember(4)]
		public int MapCode = -1;

		// Token: 0x040028ED RID: 10477
		[ProtoMember(5)]
		public string RoleString = "";

		// Token: 0x040028EE RID: 10478
		[ProtoMember(6)]
		public int Dir = 0;
	}
}
