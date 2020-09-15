using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200080A RID: 2058
	[ProtoContract]
	public class YaoSaiWorldData
	{
		// Token: 0x04004419 RID: 17433
		[ProtoMember(1)]
		public int state;

		// Token: 0x0400441A RID: 17434
		[ProtoMember(2)]
		public PrisonRoleData Mine = new PrisonRoleData();

		// Token: 0x0400441B RID: 17435
		[ProtoMember(3)]
		public PrisonRoleData Master = new PrisonRoleData();

		// Token: 0x0400441C RID: 17436
		[ProtoMember(4)]
		public int zhenfuCount;

		// Token: 0x0400441D RID: 17437
		[ProtoMember(5)]
		public int zhenfuLeftCount;

		// Token: 0x0400441E RID: 17438
		[ProtoMember(6)]
		public int jiejiuCount;

		// Token: 0x0400441F RID: 17439
		[ProtoMember(7)]
		public List<PrisonFuLuData> FuLuList = new List<PrisonFuLuData>();
	}
}
