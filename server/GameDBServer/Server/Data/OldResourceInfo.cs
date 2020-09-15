using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008A RID: 138
	[ProtoContract]
	public class OldResourceInfo
	{
		// Token: 0x040002E4 RID: 740
		[ProtoMember(1)]
		public int type = 1;

		// Token: 0x040002E5 RID: 741
		[ProtoMember(2)]
		public int exp = 0;

		// Token: 0x040002E6 RID: 742
		[ProtoMember(3)]
		public int bandmoney = 0;

		// Token: 0x040002E7 RID: 743
		[ProtoMember(4)]
		public int mojing = 0;

		// Token: 0x040002E8 RID: 744
		[ProtoMember(5)]
		public int chengjiu = 0;

		// Token: 0x040002E9 RID: 745
		[ProtoMember(6)]
		public int shengwang = 0;

		// Token: 0x040002EA RID: 746
		[ProtoMember(7)]
		public int zhangong = 0;

		// Token: 0x040002EB RID: 747
		[ProtoMember(8)]
		public int leftCount = 0;

		// Token: 0x040002EC RID: 748
		[ProtoMember(9)]
		public int roleId;

		// Token: 0x040002ED RID: 749
		[ProtoMember(10)]
		public int bandDiamond = 0;

		// Token: 0x040002EE RID: 750
		[ProtoMember(11)]
		public int xinghun = 0;

		// Token: 0x040002EF RID: 751
		[ProtoMember(12)]
		public int yuanSuFenMo = 0;
	}
}
