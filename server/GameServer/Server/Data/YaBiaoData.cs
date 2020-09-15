using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007EA RID: 2026
	[ProtoContract]
	public class YaBiaoData
	{
		// Token: 0x0400432E RID: 17198
		[ProtoMember(1)]
		public int YaBiaoID = 0;

		// Token: 0x0400432F RID: 17199
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x04004330 RID: 17200
		[ProtoMember(3)]
		public int State = 0;

		// Token: 0x04004331 RID: 17201
		[ProtoMember(4)]
		public int LineID = 0;

		// Token: 0x04004332 RID: 17202
		[ProtoMember(5)]
		public int TouBao = 0;

		// Token: 0x04004333 RID: 17203
		[ProtoMember(6)]
		public int YaBiaoDayID = 0;

		// Token: 0x04004334 RID: 17204
		[ProtoMember(7)]
		public int YaBiaoNum = 0;

		// Token: 0x04004335 RID: 17205
		[ProtoMember(8)]
		public int TakeGoods = 0;
	}
}
