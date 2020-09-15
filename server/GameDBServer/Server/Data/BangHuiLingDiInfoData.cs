using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000030 RID: 48
	[ProtoContract]
	public class BangHuiLingDiInfoData
	{
		// Token: 0x040000EB RID: 235
		[ProtoMember(1)]
		public int LingDiID = 0;

		// Token: 0x040000EC RID: 236
		[ProtoMember(2)]
		public int BHID = 0;

		// Token: 0x040000ED RID: 237
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x040000EE RID: 238
		[ProtoMember(4)]
		public string BHName = "";

		// Token: 0x040000EF RID: 239
		[ProtoMember(5)]
		public int LingDiTax = 0;

		// Token: 0x040000F0 RID: 240
		[ProtoMember(6)]
		public int TakeDayID = 0;

		// Token: 0x040000F1 RID: 241
		[ProtoMember(7)]
		public int TakeDayNum = 0;

		// Token: 0x040000F2 RID: 242
		[ProtoMember(8)]
		public int YestodayTax = 0;

		// Token: 0x040000F3 RID: 243
		[ProtoMember(9)]
		public int TaxDayID = 0;

		// Token: 0x040000F4 RID: 244
		[ProtoMember(10)]
		public int TodayTax = 0;

		// Token: 0x040000F5 RID: 245
		[ProtoMember(11)]
		public int TotalTax = 0;

		// Token: 0x040000F6 RID: 246
		[ProtoMember(12)]
		public string WarRequest = "";

		// Token: 0x040000F7 RID: 247
		[ProtoMember(13)]
		public int AwardFetchDay = 0;
	}
}
