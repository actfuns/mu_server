using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000035 RID: 53
	[ProtoContract]
	public class BHLingDiOwnData
	{
		// Token: 0x04000117 RID: 279
		[ProtoMember(1)]
		public int LingDiID = 0;

		// Token: 0x04000118 RID: 280
		[ProtoMember(2)]
		public int ZoneID = 0;

		// Token: 0x04000119 RID: 281
		[ProtoMember(3)]
		public int BHID = 0;

		// Token: 0x0400011A RID: 282
		[ProtoMember(4)]
		public string BHName = "";

		// Token: 0x0400011B RID: 283
		[ProtoMember(5)]
		public string BangQiName = "";

		// Token: 0x0400011C RID: 284
		[ProtoMember(6)]
		public int BangQiLevel = 0;
	}
}
