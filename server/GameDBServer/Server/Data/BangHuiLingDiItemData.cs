using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000031 RID: 49
	[ProtoContract]
	public class BangHuiLingDiItemData
	{
		// Token: 0x040000F8 RID: 248
		[ProtoMember(1)]
		public int LingDiID = 0;

		// Token: 0x040000F9 RID: 249
		[ProtoMember(2)]
		public int BHID = 0;

		// Token: 0x040000FA RID: 250
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x040000FB RID: 251
		[ProtoMember(4)]
		public string BHName = "";

		// Token: 0x040000FC RID: 252
		[ProtoMember(5)]
		public int LingDiTax = 0;

		// Token: 0x040000FD RID: 253
		[ProtoMember(6)]
		public string WarRequest = "";

		// Token: 0x040000FE RID: 254
		[ProtoMember(7)]
		public int AwardFetchDay = -1;
	}
}
