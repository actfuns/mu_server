using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A5 RID: 1445
	[ProtoContract]
	public class TeamData
	{
		// Token: 0x06001A4F RID: 6735 RVA: 0x00194E3C File Offset: 0x0019303C
		public TeamMemberData GetLeader()
		{
			TeamMemberData result;
			if (this.TeamRoles == null || this.TeamRoles.Count < 1)
			{
				result = null;
			}
			else
			{
				result = this.TeamRoles.Find((TeamMemberData _x) => _x.RoleID == this.LeaderRoleID);
			}
			return result;
		}

		// Token: 0x040028BC RID: 10428
		[ProtoMember(1)]
		public int TeamID = 0;

		// Token: 0x040028BD RID: 10429
		[ProtoMember(2)]
		public int LeaderRoleID = 0;

		// Token: 0x040028BE RID: 10430
		[ProtoMember(3)]
		public List<TeamMemberData> TeamRoles;

		// Token: 0x040028BF RID: 10431
		[ProtoMember(4)]
		public long AddDateTime = 0L;

		// Token: 0x040028C0 RID: 10432
		[ProtoMember(5)]
		public int GetThingOpt = 0;

		// Token: 0x040028C1 RID: 10433
		[ProtoMember(6)]
		public int PosX = 0;

		// Token: 0x040028C2 RID: 10434
		[ProtoMember(7)]
		public int PosY = 0;
	}
}
