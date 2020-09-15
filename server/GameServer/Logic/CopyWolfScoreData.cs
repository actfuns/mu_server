using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020007BB RID: 1979
	[ProtoContract]
	public class CopyWolfScoreData
	{
		// Token: 0x04003F76 RID: 16246
		[ProtoMember(1)]
		public int Wave = 0;

		// Token: 0x04003F77 RID: 16247
		[ProtoMember(2)]
		public long EndTime = 0L;

		// Token: 0x04003F78 RID: 16248
		[ProtoMember(3)]
		public int FortLifeNow = 0;

		// Token: 0x04003F79 RID: 16249
		[ProtoMember(4)]
		public int FortLifeMax = 0;

		// Token: 0x04003F7A RID: 16250
		[ProtoMember(5)]
		public Dictionary<int, int> RoleMonsterScore = new Dictionary<int, int>();

		// Token: 0x04003F7B RID: 16251
		[ProtoMember(6)]
		public int MonsterCount = 0;
	}
}
