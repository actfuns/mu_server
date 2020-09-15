using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F5 RID: 245
	[ProtoContract]
	public class HongBaoRankItemData
	{
		// Token: 0x040006EB RID: 1771
		[ProtoMember(1)]
		public int rankID;

		// Token: 0x040006EC RID: 1772
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x040006ED RID: 1773
		[ProtoMember(3)]
		public int daimondNum;

		// Token: 0x040006EE RID: 1774
		[ProtoMember(4)]
		public int roleId;
	}
}
