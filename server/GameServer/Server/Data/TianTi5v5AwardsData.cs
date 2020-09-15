using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000BB RID: 187
	[ProtoContract]
	public class TianTi5v5AwardsData
	{
		// Token: 0x04000470 RID: 1136
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04000471 RID: 1137
		[ProtoMember(2)]
		public int DuanWeiJiFen;

		// Token: 0x04000472 RID: 1138
		[ProtoMember(3)]
		public int RongYao;

		// Token: 0x04000473 RID: 1139
		[ProtoMember(4)]
		public int LianShengJiFen;

		// Token: 0x04000474 RID: 1140
		[ProtoMember(5)]
		public int DuanWeiId;
	}
}
