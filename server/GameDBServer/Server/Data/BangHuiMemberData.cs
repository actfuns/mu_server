using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000034 RID: 52
	[ProtoContract]
	public class BangHuiMemberData
	{
		// Token: 0x04000108 RID: 264
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x04000109 RID: 265
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x0400010A RID: 266
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x0400010B RID: 267
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x0400010C RID: 268
		[ProtoMember(5)]
		public int BHZhiwu = 0;

		// Token: 0x0400010D RID: 269
		[ProtoMember(6)]
		public string ChengHao = "";

		// Token: 0x0400010E RID: 270
		[ProtoMember(7)]
		public int BangGong = 0;

		// Token: 0x0400010F RID: 271
		[ProtoMember(8)]
		public int Level = 0;

		// Token: 0x04000110 RID: 272
		[ProtoMember(9)]
		public int XueWeiNum = 0;

		// Token: 0x04000111 RID: 273
		[ProtoMember(10)]
		public int SkillLearnedNum = 0;

		// Token: 0x04000112 RID: 274
		[ProtoMember(11)]
		public int OnlineState = 0;

		// Token: 0x04000113 RID: 275
		[ProtoMember(12)]
		public int BangHuiMemberCombatForce = 0;

		// Token: 0x04000114 RID: 276
		[ProtoMember(13)]
		public int BangHuiMemberChangeLifeLev = 0;

		// Token: 0x04000115 RID: 277
		[ProtoMember(14)]
		public int JunTuanZhiWu;

		// Token: 0x04000116 RID: 278
		[ProtoMember(18)]
		public long LogOffTime;
	}
}
