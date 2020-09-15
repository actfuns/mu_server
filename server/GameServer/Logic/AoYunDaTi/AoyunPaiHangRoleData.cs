using System;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000202 RID: 514
	[ProtoContract]
	public class AoyunPaiHangRoleData
	{
		// Token: 0x04000B63 RID: 2915
		[ProtoMember(1)]
		public int ZoneId = 0;

		// Token: 0x04000B64 RID: 2916
		[ProtoMember(2)]
		public int RoleId = 0;

		// Token: 0x04000B65 RID: 2917
		[ProtoMember(3)]
		public string RoleName = null;

		// Token: 0x04000B66 RID: 2918
		[ProtoMember(4)]
		public int RolePoint = 0;

		// Token: 0x04000B67 RID: 2919
		[ProtoMember(5)]
		public int RoleCurrentPoint;

		// Token: 0x04000B68 RID: 2920
		[ProtoMember(6)]
		public int RoleRank = 0;
	}
}
