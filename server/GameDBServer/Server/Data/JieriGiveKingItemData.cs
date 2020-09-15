using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006F RID: 111
	[ProtoContract]
	public class JieriGiveKingItemData
	{
		// Token: 0x04000268 RID: 616
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000269 RID: 617
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x0400026A RID: 618
		[ProtoMember(3)]
		public int TotalGive;

		// Token: 0x0400026B RID: 619
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x0400026C RID: 620
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x0400026D RID: 621
		[ProtoMember(6)]
		public int ZoneID;
	}
}
