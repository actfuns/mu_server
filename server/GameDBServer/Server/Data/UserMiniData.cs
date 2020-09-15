using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C9 RID: 201
	[ProtoContract]
	public class UserMiniData
	{
		// Token: 0x04000579 RID: 1401
		[ProtoMember(1)]
		public string UserId;

		// Token: 0x0400057A RID: 1402
		[ProtoMember(2)]
		public int LastRoleId;

		// Token: 0x0400057B RID: 1403
		[ProtoMember(3)]
		public int RealMoney;

		// Token: 0x0400057C RID: 1404
		[ProtoMember(4)]
		public DateTime MinCreateRoleTime;

		// Token: 0x0400057D RID: 1405
		[ProtoMember(5)]
		public DateTime LastLoginTime;

		// Token: 0x0400057E RID: 1406
		[ProtoMember(6)]
		public DateTime LastLogoutTime;

		// Token: 0x0400057F RID: 1407
		[ProtoMember(7)]
		public DateTime RoleCreateTime;

		// Token: 0x04000580 RID: 1408
		[ProtoMember(8)]
		public DateTime RoleLastLoginTime;

		// Token: 0x04000581 RID: 1409
		[ProtoMember(9)]
		public DateTime RoleLastLogoutTime;

		// Token: 0x04000582 RID: 1410
		[ProtoMember(10)]
		public int MaxLevel;

		// Token: 0x04000583 RID: 1411
		[ProtoMember(11)]
		public int MaxChangeLifeCount;
	}
}
