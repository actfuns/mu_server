using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	// Token: 0x02000830 RID: 2096
	[ProtoContract]
	public class ZhuanPanMainData
	{
		// Token: 0x04004577 RID: 17783
		[ProtoMember(1)]
		public List<ZhuanPanItem> ZhuanPanAwardItemList;

		// Token: 0x04004578 RID: 17784
		[ProtoMember(2)]
		public DateTime FreeTime;

		// Token: 0x04004579 RID: 17785
		[ProtoMember(3)]
		public int LeftFuLiCount;

		// Token: 0x0400457A RID: 17786
		[ProtoMember(4)]
		public int[] ZhuanPanCostArray;

		// Token: 0x0400457B RID: 17787
		[ProtoMember(5)]
		public ZhuanPanItem GoodsAward;

		// Token: 0x0400457C RID: 17788
		[ProtoMember(6)]
		public List<ZhuanPanGongGaoData> GongGaoList;
	}
}
