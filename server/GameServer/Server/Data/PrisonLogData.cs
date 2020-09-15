using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000808 RID: 2056
	[ProtoContract]
	public class PrisonLogData
	{
		// Token: 0x0400440C RID: 17420
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x0400440D RID: 17421
		[ProtoMember(2)]
		public string Name1;

		// Token: 0x0400440E RID: 17422
		[ProtoMember(3)]
		public string Name2;

		// Token: 0x0400440F RID: 17423
		[ProtoMember(4)]
		public int JiangLiType = 0;

		// Token: 0x04004410 RID: 17424
		[ProtoMember(5)]
		public int JiangLiCount = 0;
	}
}
