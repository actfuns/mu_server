using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002A RID: 42
	[ProtoContract]
	public class BangGongHistData
	{
		// Token: 0x040000A9 RID: 169
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x040000AA RID: 170
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040000AB RID: 171
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x040000AC RID: 172
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040000AD RID: 173
		[ProtoMember(5)]
		public int RoleLevel = 0;

		// Token: 0x040000AE RID: 174
		[ProtoMember(6)]
		public int BHZhiWu = 0;

		// Token: 0x040000AF RID: 175
		[ProtoMember(7)]
		public string BHChengHao = "";

		// Token: 0x040000B0 RID: 176
		[ProtoMember(8)]
		public int Goods1Num = 0;

		// Token: 0x040000B1 RID: 177
		[ProtoMember(9)]
		public int Goods2Num = 0;

		// Token: 0x040000B2 RID: 178
		[ProtoMember(10)]
		public int Goods3Num = 0;

		// Token: 0x040000B3 RID: 179
		[ProtoMember(11)]
		public int Goods4Num = 0;

		// Token: 0x040000B4 RID: 180
		[ProtoMember(12)]
		public int Goods5Num = 0;

		// Token: 0x040000B5 RID: 181
		[ProtoMember(13)]
		public int TongQian = 0;

		// Token: 0x040000B6 RID: 182
		[ProtoMember(14)]
		public int BangGong = 0;
	}
}
