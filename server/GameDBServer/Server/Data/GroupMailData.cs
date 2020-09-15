using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005F RID: 95
	[ProtoContract]
	public class GroupMailData
	{
		// Token: 0x0400020E RID: 526
		[ProtoMember(1)]
		public int GMailID = 0;

		// Token: 0x0400020F RID: 527
		[ProtoMember(2)]
		public string Subject = "";

		// Token: 0x04000210 RID: 528
		[ProtoMember(3)]
		public string Content = "";

		// Token: 0x04000211 RID: 529
		[ProtoMember(4)]
		public string Conditions = "";

		// Token: 0x04000212 RID: 530
		[ProtoMember(5)]
		public long InputTime = 0L;

		// Token: 0x04000213 RID: 531
		[ProtoMember(6)]
		public long EndTime = 0L;

		// Token: 0x04000214 RID: 532
		[ProtoMember(7)]
		public int Yinliang = 0;

		// Token: 0x04000215 RID: 533
		[ProtoMember(8)]
		public int Tongqian = 0;

		// Token: 0x04000216 RID: 534
		[ProtoMember(9)]
		public int YuanBao = 0;

		// Token: 0x04000217 RID: 535
		[ProtoMember(10)]
		public List<GoodsData> GoodsList = null;
	}
}
