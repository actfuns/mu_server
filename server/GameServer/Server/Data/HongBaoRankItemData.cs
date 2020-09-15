using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E6 RID: 742
	[ProtoContract]
	public class HongBaoRankItemData
	{
		// Token: 0x04001314 RID: 4884
		[ProtoMember(1)]
		public int rankID;

		// Token: 0x04001315 RID: 4885
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x04001316 RID: 4886
		[ProtoMember(3)]
		public int daimondNum;

		// Token: 0x04001317 RID: 4887
		[ProtoMember(4)]
		public int roleId;
	}
}
