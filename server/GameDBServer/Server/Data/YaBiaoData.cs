using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CB RID: 203
	[ProtoContract]
	public class YaBiaoData
	{
		// Token: 0x0400058D RID: 1421
		[ProtoMember(1)]
		public int YaBiaoID = 0;

		// Token: 0x0400058E RID: 1422
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x0400058F RID: 1423
		[ProtoMember(3)]
		public int State = 0;

		// Token: 0x04000590 RID: 1424
		[ProtoMember(4)]
		public int LineID = 0;

		// Token: 0x04000591 RID: 1425
		[ProtoMember(5)]
		public int TouBao = 0;

		// Token: 0x04000592 RID: 1426
		[ProtoMember(6)]
		public int YaBiaoDayID = 0;

		// Token: 0x04000593 RID: 1427
		[ProtoMember(7)]
		public int YaBiaoNum = 0;

		// Token: 0x04000594 RID: 1428
		[ProtoMember(8)]
		public int TakeGoods = 0;
	}
}
