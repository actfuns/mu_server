using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003DD RID: 989
	[ProtoContract]
	public class RebornBossAttackLog
	{
		// Token: 0x04001A4B RID: 6731
		[ProtoMember(1)]
		public int UserPtID;

		// Token: 0x04001A4C RID: 6732
		[ProtoMember(2)]
		public int RoleID;

		// Token: 0x04001A4D RID: 6733
		[ProtoMember(3)]
		public string Param;

		// Token: 0x04001A4E RID: 6734
		[ProtoMember(4)]
		public int DamagePct;

		// Token: 0x04001A4F RID: 6735
		[ProtoMember(5)]
		public string RoleName;

		// Token: 0x04001A50 RID: 6736
		public long InjureSum;

		// Token: 0x04001A51 RID: 6737
		public bool NotifySelf;

		// Token: 0x04001A52 RID: 6738
		public int RankNum;

		// Token: 0x04001A53 RID: 6739
		public int ServerID;

		// Token: 0x04001A54 RID: 6740
		public int LocalRoleID;

		// Token: 0x04001A55 RID: 6741
		public int ServerPtID;
	}
}
