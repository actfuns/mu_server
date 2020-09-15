using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D9 RID: 217
	[ProtoContract]
	public class ZhengBaSupportFlagData
	{
		// Token: 0x040005FD RID: 1533
		[ProtoMember(1)]
		public int UnionGroup;

		// Token: 0x040005FE RID: 1534
		[ProtoMember(2)]
		public int Group;

		// Token: 0x040005FF RID: 1535
		[ProtoMember(3)]
		public bool IsOppose;

		// Token: 0x04000600 RID: 1536
		[ProtoMember(4)]
		public bool IsSupport;

		// Token: 0x04000601 RID: 1537
		[ProtoMember(5)]
		public bool IsYaZhu;

		// Token: 0x04000602 RID: 1538
		[ProtoMember(6)]
		public int SupportDay;
	}
}
