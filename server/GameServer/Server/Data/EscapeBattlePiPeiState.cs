using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008F RID: 143
	[ProtoContract]
	public class EscapeBattlePiPeiState
	{
		// Token: 0x04000365 RID: 869
		[ProtoMember(1)]
		public List<EscapeBattleJoinRoleInfo> RoleList = new List<EscapeBattleJoinRoleInfo>();

		// Token: 0x04000366 RID: 870
		public int EscapeJiFen;

		// Token: 0x04000367 RID: 871
		public int ReadyNum;

		// Token: 0x04000368 RID: 872
		public int GameID;

		// Token: 0x04000369 RID: 873
		public int State;

		// Token: 0x0400036A RID: 874
		public DateTime FightTime;

		// Token: 0x0400036B RID: 875
		public DateTime EndTime;
	}
}
