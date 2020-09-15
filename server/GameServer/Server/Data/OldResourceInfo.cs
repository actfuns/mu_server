using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000576 RID: 1398
	[ProtoContract]
	public class OldResourceInfo
	{
		// Token: 0x040025B5 RID: 9653
		[ProtoMember(1)]
		public int type = 1;

		// Token: 0x040025B6 RID: 9654
		[ProtoMember(2)]
		public int exp = 0;

		// Token: 0x040025B7 RID: 9655
		[ProtoMember(3)]
		public int bandmoney = 0;

		// Token: 0x040025B8 RID: 9656
		[ProtoMember(4)]
		public int mojing = 0;

		// Token: 0x040025B9 RID: 9657
		[ProtoMember(5)]
		public int chengjiu = 0;

		// Token: 0x040025BA RID: 9658
		[ProtoMember(6)]
		public int shengwang = 0;

		// Token: 0x040025BB RID: 9659
		[ProtoMember(7)]
		public int zhangong = 0;

		// Token: 0x040025BC RID: 9660
		[ProtoMember(8)]
		public int leftCount = 0;

		// Token: 0x040025BD RID: 9661
		[ProtoMember(9)]
		public int roleId;

		// Token: 0x040025BE RID: 9662
		[ProtoMember(10)]
		public int bandDiamond = 0;

		// Token: 0x040025BF RID: 9663
		[ProtoMember(11)]
		public int xinghun = 0;

		// Token: 0x040025C0 RID: 9664
		[ProtoMember(12)]
		public int yuanSuFenMo = 0;

		// Token: 0x040025C1 RID: 9665
		[ProtoMember(13)]
		public int HasGetOffsetDay;
	}
}
