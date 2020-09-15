using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002ED RID: 749
	[ProtoContract]
	public class JieriHongBaoKingItemData
	{
		// Token: 0x0400133D RID: 4925
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400133E RID: 4926
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x0400133F RID: 4927
		[ProtoMember(3)]
		public int TotalRecv;

		// Token: 0x04001340 RID: 4928
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x04001341 RID: 4929
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x04001342 RID: 4930
		[ProtoMember(6)]
		public int ZoneID;
	}
}
