using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000FC RID: 252
	[ProtoContract]
	public class JieriHongBaoKingItemData
	{
		// Token: 0x04000711 RID: 1809
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000712 RID: 1810
		[ProtoMember(2)]
		public string Rolename;

		// Token: 0x04000713 RID: 1811
		[ProtoMember(3)]
		public int TotalRecv;

		// Token: 0x04000714 RID: 1812
		[ProtoMember(4)]
		public int Rank;

		// Token: 0x04000715 RID: 1813
		[ProtoMember(5)]
		public int GetAwardTimes;

		// Token: 0x04000716 RID: 1814
		[ProtoMember(6)]
		public int ZoneID;
	}
}
