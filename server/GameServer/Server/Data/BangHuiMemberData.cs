using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200000D RID: 13
	[ProtoContract]
	public class BangHuiMemberData
	{
		// Token: 0x0400004A RID: 74
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x0400004B RID: 75
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x0400004C RID: 76
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x0400004D RID: 77
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x0400004E RID: 78
		[ProtoMember(5)]
		public int BHZhiwu = 0;

		// Token: 0x0400004F RID: 79
		[ProtoMember(6)]
		public string ChengHao = "";

		// Token: 0x04000050 RID: 80
		[ProtoMember(7)]
		public int BangGong = 0;

		// Token: 0x04000051 RID: 81
		[ProtoMember(8)]
		public int Level = 0;

		// Token: 0x04000052 RID: 82
		[ProtoMember(9)]
		public int XueWeiNum = 0;

		// Token: 0x04000053 RID: 83
		[ProtoMember(10)]
		public int SkillLearnedNum = 0;

		// Token: 0x04000054 RID: 84
		[ProtoMember(11)]
		public int OnlineState = 0;

		// Token: 0x04000055 RID: 85
		[ProtoMember(12)]
		public int BangHuiMemberCombatForce = 0;

		// Token: 0x04000056 RID: 86
		[ProtoMember(13)]
		public int BangHuiMemberChangeLifeLev = 0;

		// Token: 0x04000057 RID: 87
		[ProtoMember(14)]
		public int JunTuanZhiWu;

		// Token: 0x04000058 RID: 88
		[ProtoMember(15)]
		public int YaoSaiBossState;

		// Token: 0x04000059 RID: 89
		[ProtoMember(16)]
		public int YaoSaiJianYuState;

		// Token: 0x0400005A RID: 90
		[ProtoMember(18)]
		public long LogOffTime;
	}
}
