using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000422 RID: 1058
	[ProtoContract]
	public class ZhengBaSupportFlagData
	{
		// Token: 0x04001C73 RID: 7283
		[ProtoMember(1)]
		public int UnionGroup;

		// Token: 0x04001C74 RID: 7284
		[ProtoMember(2)]
		public int Group;

		// Token: 0x04001C75 RID: 7285
		[ProtoMember(3)]
		public bool IsOppose;

		// Token: 0x04001C76 RID: 7286
		[ProtoMember(4)]
		public bool IsSupport;

		// Token: 0x04001C77 RID: 7287
		[ProtoMember(5)]
		public bool IsYaZhu;

		// Token: 0x04001C78 RID: 7288
		[ProtoMember(6)]
		public int RankOfDay;
	}
}
