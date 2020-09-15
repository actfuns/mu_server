using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002EB RID: 747
	[ProtoContract]
	public class SingleHongBaoRankInfo
	{
		// Token: 0x04001336 RID: 4918
		[ProtoMember(1)]
		public string roleName;

		// Token: 0x04001337 RID: 4919
		[ProtoMember(2)]
		public int diamondNum;

		// Token: 0x04001338 RID: 4920
		[ProtoMember(3)]
		public int zuiJia;

		// Token: 0x04001339 RID: 4921
		[ProtoMember(4)]
		public DateTime recvTime;
	}
}
