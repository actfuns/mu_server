using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000162 RID: 354
	[ProtoContract]
	public class MarriageData
	{
		// Token: 0x040007C7 RID: 1991
		[ProtoMember(1)]
		public int nSpouseID = -1;

		// Token: 0x040007C8 RID: 1992
		[ProtoMember(2)]
		public sbyte byMarrytype = -1;

		// Token: 0x040007C9 RID: 1993
		[ProtoMember(3)]
		public int nRingID = -1;

		// Token: 0x040007CA RID: 1994
		[ProtoMember(4)]
		public int nGoodwillexp = 0;

		// Token: 0x040007CB RID: 1995
		[ProtoMember(5)]
		public sbyte byGoodwillstar = 0;

		// Token: 0x040007CC RID: 1996
		[ProtoMember(6)]
		public sbyte byGoodwilllevel = 0;

		// Token: 0x040007CD RID: 1997
		[ProtoMember(7)]
		public int nGivenrose = 0;

		// Token: 0x040007CE RID: 1998
		[ProtoMember(8)]
		public string strLovemessage = "";

		// Token: 0x040007CF RID: 1999
		[ProtoMember(9)]
		public sbyte byAutoReject = 0;

		// Token: 0x040007D0 RID: 2000
		[ProtoMember(10)]
		public string ChangTime = "1900-01-01 12:00:00";
	}
}
