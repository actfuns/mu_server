using System;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	// Token: 0x02000832 RID: 2098
	[ProtoContract]
	public class ZhuanPanGongGaoData
	{
		// Token: 0x04004582 RID: 17794
		[ProtoMember(1)]
		public int ZoneId;

		// Token: 0x04004583 RID: 17795
		[ProtoMember(2)]
		public int Rid;

		// Token: 0x04004584 RID: 17796
		[ProtoMember(3)]
		public string RoleName;

		// Token: 0x04004585 RID: 17797
		[ProtoMember(4)]
		public string GoodsId;

		// Token: 0x04004586 RID: 17798
		[ProtoMember(5)]
		public int GoodsIndex;
	}
}
