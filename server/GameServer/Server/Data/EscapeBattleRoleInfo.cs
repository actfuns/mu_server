using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000091 RID: 145
	[ProtoContract]
	public class EscapeBattleRoleInfo
	{
		// Token: 0x04000373 RID: 883
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000374 RID: 884
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04000375 RID: 885
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04000376 RID: 886
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		// Token: 0x04000377 RID: 887
		[ProtoMember(5)]
		public int ZoneID = 0;

		// Token: 0x04000378 RID: 888
		[ProtoMember(6)]
		public int Occupation = 0;

		// Token: 0x04000379 RID: 889
		[ProtoMember(7)]
		public int RoleSex = 0;

		// Token: 0x0400037A RID: 890
		[ProtoMember(8)]
		public int LifeV = 0;

		// Token: 0x0400037B RID: 891
		[ProtoMember(9)]
		public int MaxLifeV = 0;

		// Token: 0x0400037C RID: 892
		public int ZhanDuiID;

		// Token: 0x0400037D RID: 893
		public bool OnLine;

		// Token: 0x0400037E RID: 894
		public int ReliveCount;

		// Token: 0x0400037F RID: 895
		public int KillRoleNum;
	}
}
