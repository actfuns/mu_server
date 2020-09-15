using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011C RID: 284
	[ProtoContract]
	public class BangHuiLingDiItemData
	{
		// Token: 0x04000621 RID: 1569
		[ProtoMember(1)]
		public int LingDiID = 0;

		// Token: 0x04000622 RID: 1570
		[ProtoMember(2)]
		public int BHID = 0;

		// Token: 0x04000623 RID: 1571
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x04000624 RID: 1572
		[ProtoMember(4)]
		public string BHName = "";

		// Token: 0x04000625 RID: 1573
		[ProtoMember(5)]
		public int LingDiTax = 0;

		// Token: 0x04000626 RID: 1574
		[ProtoMember(6)]
		public string WarRequest = "";

		// Token: 0x04000627 RID: 1575
		[ProtoMember(7)]
		public int AwardFetchDay = -1;
	}
}
