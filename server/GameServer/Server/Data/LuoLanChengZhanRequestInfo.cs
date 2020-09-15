using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000159 RID: 345
	[ProtoContract]
	public class LuoLanChengZhanRequestInfo
	{
		// Token: 0x040007A6 RID: 1958
		[ProtoMember(1)]
		public int Site = 0;

		// Token: 0x040007A7 RID: 1959
		[ProtoMember(2)]
		public int BHID = 0;

		// Token: 0x040007A8 RID: 1960
		[ProtoMember(3)]
		public int BidMoney = 0;
	}
}
