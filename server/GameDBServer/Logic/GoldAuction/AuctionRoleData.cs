using System;
using ProtoBuf;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x0200013B RID: 315
	[ProtoContract]
	public class AuctionRoleData
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x0002B934 File Offset: 0x00029B34
		public AuctionRoleData()
		{
			this.m_RoleID = 0;
			this.Value = 0L;
			this.strUserID = "";
			this.m_RoleName = "";
			this.ZoneID = -1;
			this.ServerId = -1;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0002B974 File Offset: 0x00029B74
		public string getAttackerValue()
		{
			try
			{
				return string.Format("{0},{1},{2},{3},{4},{5}", new object[]
				{
					this.m_RoleID,
					this.Value,
					this.m_RoleName,
					this.ZoneID,
					this.strUserID,
					this.ServerId
				});
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x040007FF RID: 2047
		[ProtoMember(1)]
		public int m_RoleID;

		// Token: 0x04000800 RID: 2048
		[ProtoMember(2)]
		public long Value;

		// Token: 0x04000801 RID: 2049
		[ProtoMember(3)]
		public string m_RoleName;

		// Token: 0x04000802 RID: 2050
		[ProtoMember(4)]
		public int ZoneID;

		// Token: 0x04000803 RID: 2051
		[ProtoMember(5)]
		public string strUserID;

		// Token: 0x04000804 RID: 2052
		[ProtoMember(6)]
		public int ServerId;
	}
}
