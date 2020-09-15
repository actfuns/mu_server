using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000071 RID: 113
	[ProtoContract]
	public class JieriRecvKingItemData
	{
		// Token: 0x04000270 RID: 624
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000271 RID: 625
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x04000272 RID: 626
		[ProtoMember(3)]
		public int TotalRecv;

		// Token: 0x04000273 RID: 627
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x04000274 RID: 628
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x04000275 RID: 629
		[ProtoMember(6)]
		public int ZoneID;
	}
}
