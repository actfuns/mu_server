using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200014E RID: 334
	[ProtoContract]
	public class JieriGiveKingItemData
	{
		// Token: 0x04000770 RID: 1904
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000771 RID: 1905
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x04000772 RID: 1906
		[ProtoMember(3)]
		public int TotalGive;

		// Token: 0x04000773 RID: 1907
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x04000774 RID: 1908
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x04000775 RID: 1909
		[ProtoMember(6)]
		public int ZoneID;
	}
}
