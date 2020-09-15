using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000423 RID: 1059
	[ProtoContract]
	public class ZhengBaWaitYaZhuAwardData
	{
		// Token: 0x04001C79 RID: 7289
		[ProtoMember(1)]
		public int Month;

		// Token: 0x04001C7A RID: 7290
		[ProtoMember(2)]
		public int RankOfDay;

		// Token: 0x04001C7B RID: 7291
		[ProtoMember(3)]
		public int FromRoleId;

		// Token: 0x04001C7C RID: 7292
		[ProtoMember(4)]
		public int UnionGroup;

		// Token: 0x04001C7D RID: 7293
		[ProtoMember(5)]
		public int Group;
	}
}
