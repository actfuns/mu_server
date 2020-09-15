using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000FA RID: 250
	[ProtoContract]
	public class SingleHongBaoRankInfo
	{
		// Token: 0x0400070C RID: 1804
		[ProtoMember(1)]
		public string roleName;

		// Token: 0x0400070D RID: 1805
		[ProtoMember(2)]
		public int diamondNum;

		// Token: 0x0400070E RID: 1806
		[ProtoMember(3)]
		public int zuiJia;
	}
}
