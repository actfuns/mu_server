using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DA RID: 218
	[ProtoContract]
	public class ZhengBaWaitYaZhuAwardData
	{
		// Token: 0x04000603 RID: 1539
		[ProtoMember(1)]
		public int Month;

		// Token: 0x04000604 RID: 1540
		[ProtoMember(2)]
		public int RankOfDay;

		// Token: 0x04000605 RID: 1541
		[ProtoMember(3)]
		public int FromRoleId;

		// Token: 0x04000606 RID: 1542
		[ProtoMember(4)]
		public int UnionGroup;

		// Token: 0x04000607 RID: 1543
		[ProtoMember(5)]
		public int Group;
	}
}
