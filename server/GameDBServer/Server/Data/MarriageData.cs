using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007F RID: 127
	[ProtoContract]
	public class MarriageData
	{
		// Token: 0x040002AE RID: 686
		[ProtoMember(1)]
		public int nSpouseID = -1;

		// Token: 0x040002AF RID: 687
		[ProtoMember(2)]
		public sbyte byMarrytype = -1;

		// Token: 0x040002B0 RID: 688
		[ProtoMember(3)]
		public int nRingID = -1;

		// Token: 0x040002B1 RID: 689
		[ProtoMember(4)]
		public int nGoodwillexp = 0;

		// Token: 0x040002B2 RID: 690
		[ProtoMember(5)]
		public sbyte byGoodwillstar = 0;

		// Token: 0x040002B3 RID: 691
		[ProtoMember(6)]
		public sbyte byGoodwilllevel = 0;

		// Token: 0x040002B4 RID: 692
		[ProtoMember(7)]
		public int nGivenrose = 0;

		// Token: 0x040002B5 RID: 693
		[ProtoMember(8)]
		public string strLovemessage = "";

		// Token: 0x040002B6 RID: 694
		[ProtoMember(9)]
		public sbyte byAutoReject = 0;

		// Token: 0x040002B7 RID: 695
		[ProtoMember(10)]
		public string ChangTime = "1900-01-01 12:00:00";
	}
}
