using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200043C RID: 1084
	[ProtoContract]
	public class BossLifeLog
	{
		// Token: 0x04001D3C RID: 7484
		[ProtoMember(1)]
		public long InjureSum;

		// Token: 0x04001D3D RID: 7485
		[ProtoMember(2)]
		public List<BHAttackLog> BHAttackRank;

		// Token: 0x04001D3E RID: 7486
		[ProtoMember(3)]
		public BHAttackLog SelfBHAttack;
	}
}
