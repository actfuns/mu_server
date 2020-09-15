using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	// Token: 0x02000546 RID: 1350
	[ProtoContract]
	public class CopyTeamData
	{
		// Token: 0x060019C1 RID: 6593 RVA: 0x00190E38 File Offset: 0x0018F038
		public CopyTeamData SimpleClone()
		{
			return new CopyTeamData
			{
				TeamID = this.TeamID,
				LeaderRoleID = this.LeaderRoleID,
				StartTime = this.StartTime,
				GetThingOpt = this.GetThingOpt,
				SceneIndex = this.SceneIndex,
				FuBenSeqID = this.FuBenSeqID,
				MinZhanLi = this.MinZhanLi,
				AutoStart = this.AutoStart,
				TeamRoles = null,
				MemberCount = this.MemberCount,
				TeamName = this.TeamName
			};
		}

		// Token: 0x04002425 RID: 9253
		[ProtoMember(1)]
		public int TeamID = 0;

		// Token: 0x04002426 RID: 9254
		[ProtoMember(2)]
		public int LeaderRoleID = 0;

		// Token: 0x04002427 RID: 9255
		[ProtoMember(3)]
		public List<CopyTeamMemberData> TeamRoles;

		// Token: 0x04002428 RID: 9256
		[ProtoMember(4)]
		public long StartTime = 0L;

		// Token: 0x04002429 RID: 9257
		[ProtoMember(5)]
		public int GetThingOpt = 0;

		// Token: 0x0400242A RID: 9258
		[ProtoMember(6)]
		public int SceneIndex = 0;

		// Token: 0x0400242B RID: 9259
		[ProtoMember(7)]
		public int FuBenSeqID = 0;

		// Token: 0x0400242C RID: 9260
		[ProtoMember(8)]
		public int MinZhanLi = 0;

		// Token: 0x0400242D RID: 9261
		[ProtoMember(9)]
		public bool AutoStart = false;

		// Token: 0x0400242E RID: 9262
		[ProtoMember(10)]
		public int MemberCount = 0;

		// Token: 0x0400242F RID: 9263
		[ProtoMember(11)]
		public string TeamName = null;
	}
}
