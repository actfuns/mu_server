using System;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	// Token: 0x02000831 RID: 2097
	[ProtoContract]
	public class ZhuanPanChouJiangData
	{
		// Token: 0x0400457D RID: 17789
		[ProtoMember(1)]
		public int Result;

		// Token: 0x0400457E RID: 17790
		[ProtoMember(2)]
		public ZhuanPanItem GoodsItem;

		// Token: 0x0400457F RID: 17791
		[ProtoMember(3)]
		public DateTime FreeTime;

		// Token: 0x04004580 RID: 17792
		[ProtoMember(4)]
		public int LeftFuLiCount;

		// Token: 0x04004581 RID: 17793
		[ProtoMember(5)]
		public int AwardType;
	}
}
