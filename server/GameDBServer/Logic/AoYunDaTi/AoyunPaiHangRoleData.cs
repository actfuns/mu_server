using System;
using ProtoBuf;

namespace GameDBServer.Logic.AoYunDaTi
{
	// Token: 0x02000113 RID: 275
	[ProtoContract]
	public class AoyunPaiHangRoleData
	{
		// Token: 0x0400076B RID: 1899
		[ProtoMember(1)]
		public int ZoneId = 0;

		// Token: 0x0400076C RID: 1900
		[ProtoMember(2)]
		public int RoleId = 0;

		// Token: 0x0400076D RID: 1901
		[ProtoMember(3)]
		public string RoleName = null;

		// Token: 0x0400076E RID: 1902
		[ProtoMember(4)]
		public int RolePoint = 0;

		// Token: 0x0400076F RID: 1903
		[ProtoMember(5)]
		public int RoleLastPoint = 0;

		// Token: 0x04000770 RID: 1904
		[ProtoMember(6)]
		public int RoleRank = 0;
	}
}
