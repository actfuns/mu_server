using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000090 RID: 144
	[ProtoContract]
	public class EscapeBattleJoinRoleInfo
	{
		// Token: 0x0400036C RID: 876
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400036D RID: 877
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x0400036E RID: 878
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x0400036F RID: 879
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		// Token: 0x04000370 RID: 880
		[ProtoMember(5)]
		public long CombatForce;

		// Token: 0x04000371 RID: 881
		[ProtoMember(6)]
		public bool Join;

		// Token: 0x04000372 RID: 882
		[ProtoMember(7)]
		public bool IsLeader;
	}
}
