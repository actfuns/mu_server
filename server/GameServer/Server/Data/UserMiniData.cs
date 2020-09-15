using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200019A RID: 410
	[ProtoContract]
	public class UserMiniData
	{
		// Token: 0x04000907 RID: 2311
		[ProtoMember(1)]
		public string UserId;

		// Token: 0x04000908 RID: 2312
		[ProtoMember(2)]
		public int LastRoleId;

		// Token: 0x04000909 RID: 2313
		[ProtoMember(3)]
		public int RealMoney;

		// Token: 0x0400090A RID: 2314
		[ProtoMember(4)]
		public DateTime MinCreateRoleTime;

		// Token: 0x0400090B RID: 2315
		[ProtoMember(5)]
		public DateTime LastLoginTime;

		// Token: 0x0400090C RID: 2316
		[ProtoMember(6)]
		public DateTime LastLogoutTime;

		// Token: 0x0400090D RID: 2317
		[ProtoMember(7)]
		public DateTime RoleCreateTime;

		// Token: 0x0400090E RID: 2318
		[ProtoMember(8)]
		public DateTime RoleLastLoginTime;

		// Token: 0x0400090F RID: 2319
		[ProtoMember(9)]
		public DateTime RoleLastLogoutTime;

		// Token: 0x04000910 RID: 2320
		[ProtoMember(10)]
		public int MaxLevel;

		// Token: 0x04000911 RID: 2321
		[ProtoMember(11)]
		public int MaxChangeLifeCount;
	}
}
