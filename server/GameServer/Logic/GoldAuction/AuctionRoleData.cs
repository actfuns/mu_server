using System;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x0200009E RID: 158
	[ProtoContract]
	public class AuctionRoleData
	{
		// Token: 0x06000276 RID: 630 RVA: 0x0002AA48 File Offset: 0x00028C48
		public AuctionRoleData()
		{
			this.m_RoleID = 0;
			this.Value = 0L;
			this.strUserID = "";
			this.m_RoleName = "";
			this.ZoneID = -1;
			this.ServerId = -1;
		}

		// Token: 0x040003BB RID: 955
		[ProtoMember(1)]
		public int m_RoleID;

		// Token: 0x040003BC RID: 956
		[ProtoMember(2)]
		public long Value;

		// Token: 0x040003BD RID: 957
		[ProtoMember(3)]
		public string m_RoleName;

		// Token: 0x040003BE RID: 958
		[ProtoMember(4)]
		public int ZoneID;

		// Token: 0x040003BF RID: 959
		[ProtoMember(5)]
		public string strUserID;

		// Token: 0x040003C0 RID: 960
		[ProtoMember(6)]
		public int ServerId;
	}
}
