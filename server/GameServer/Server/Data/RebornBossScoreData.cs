using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003DE RID: 990
	[ProtoContract]
	public class RebornBossScoreData
	{
		// Token: 0x04001A56 RID: 6742
		[ProtoMember(1)]
		public int LeftLifePct;

		// Token: 0x04001A57 RID: 6743
		[ProtoMember(2)]
		public List<RebornBossAttackLog> rankList = new List<RebornBossAttackLog>();

		// Token: 0x04001A58 RID: 6744
		[ProtoMember(3)]
		public int SelfRankNum;

		// Token: 0x04001A59 RID: 6745
		[ProtoMember(4)]
		public int SelfDamagePct;

		// Token: 0x04001A5A RID: 6746
		[ProtoMember(5)]
		public string NextTime = "";

		// Token: 0x04001A5B RID: 6747
		public int BossExtensionID;

		// Token: 0x04001A5C RID: 6748
		public DateTime BossRefreshTime = DateTime.MinValue;

		// Token: 0x04001A5D RID: 6749
		public DateTime BossBeAttackTm;

		// Token: 0x04001A5E RID: 6750
		public double VLife;

		// Token: 0x04001A5F RID: 6751
		public double VLifeMax;

		// Token: 0x04001A60 RID: 6752
		public int MonsterID;
	}
}
