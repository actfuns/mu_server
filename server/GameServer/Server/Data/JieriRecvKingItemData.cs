using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000150 RID: 336
	[ProtoContract]
	public class JieriRecvKingItemData
	{
		// Token: 0x04000778 RID: 1912
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000779 RID: 1913
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x0400077A RID: 1914
		[ProtoMember(3)]
		public int TotalRecv;

		// Token: 0x0400077B RID: 1915
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x0400077C RID: 1916
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x0400077D RID: 1917
		[ProtoMember(6)]
		public int ZoneID;
	}
}
