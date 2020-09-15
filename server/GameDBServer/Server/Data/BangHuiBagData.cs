using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002B RID: 43
	[ProtoContract]
	public class BangHuiBagData
	{
		// Token: 0x040000B7 RID: 183
		[ProtoMember(1)]
		public int Goods1Num = 0;

		// Token: 0x040000B8 RID: 184
		[ProtoMember(2)]
		public int Goods2Num = 0;

		// Token: 0x040000B9 RID: 185
		[ProtoMember(3)]
		public int Goods3Num = 0;

		// Token: 0x040000BA RID: 186
		[ProtoMember(4)]
		public int Goods4Num = 0;

		// Token: 0x040000BB RID: 187
		[ProtoMember(5)]
		public int Goods5Num = 0;

		// Token: 0x040000BC RID: 188
		[ProtoMember(6)]
		public int TongQian = 0;

		// Token: 0x040000BD RID: 189
		[ProtoMember(7)]
		public List<BangGongHistData> BbangGongHistList = null;
	}
}
