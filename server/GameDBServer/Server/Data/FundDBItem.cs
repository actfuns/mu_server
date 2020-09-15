using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200012F RID: 303
	[ProtoContract]
	public class FundDBItem
	{
		// Token: 0x040007CE RID: 1998
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x040007CF RID: 1999
		[ProtoMember(2)]
		public string UserID = "";

		// Token: 0x040007D0 RID: 2000
		[ProtoMember(3)]
		public int RoleID = 0;

		// Token: 0x040007D1 RID: 2001
		[ProtoMember(4)]
		public int FundType = 0;

		// Token: 0x040007D2 RID: 2002
		[ProtoMember(5)]
		public int FundID = 0;

		// Token: 0x040007D3 RID: 2003
		[ProtoMember(6)]
		public DateTime BuyTime = DateTime.MinValue;

		// Token: 0x040007D4 RID: 2004
		[ProtoMember(7)]
		public int AwardID = 0;

		// Token: 0x040007D5 RID: 2005
		[ProtoMember(8)]
		public int Value1 = 0;

		// Token: 0x040007D6 RID: 2006
		[ProtoMember(9)]
		public int Value2 = 0;

		// Token: 0x040007D7 RID: 2007
		[ProtoMember(10)]
		public int State = 0;
	}
}
