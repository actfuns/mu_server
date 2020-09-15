using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015E RID: 350
	[ProtoContract]
	public class LuoLanChengZhanRequestInfoEx
	{
		// Token: 0x040007B5 RID: 1973
		[ProtoMember(1)]
		public int Site = 0;

		// Token: 0x040007B6 RID: 1974
		[ProtoMember(2)]
		public int BHID = 0;

		// Token: 0x040007B7 RID: 1975
		[ProtoMember(3)]
		public int BidMoney = 0;

		// Token: 0x040007B8 RID: 1976
		[ProtoMember(4)]
		public string BHName = "";
	}
}
