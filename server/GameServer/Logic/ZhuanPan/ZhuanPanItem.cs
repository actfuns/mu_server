using System;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	// Token: 0x0200082D RID: 2093
	[ProtoContract]
	public class ZhuanPanItem
	{
		// Token: 0x04004566 RID: 17766
		[ProtoMember(1)]
		public int ID;

		// Token: 0x04004567 RID: 17767
		[ProtoMember(2)]
		public string GoodsID;

		// Token: 0x04004568 RID: 17768
		[ProtoMember(3)]
		public int AwardLevel;

		// Token: 0x04004569 RID: 17769
		[ProtoMember(4)]
		public int GongGao;

		// Token: 0x0400456A RID: 17770
		[ProtoMember(5)]
		public int AwardLabel;
	}
}
