using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000567 RID: 1383
	[ProtoContract]
	public class KaiFuActivityData
	{
		// Token: 0x0400254D RID: 9549
		[ProtoMember(1)]
		public int[] LevelUpAwardRemainQuota;

		// Token: 0x0400254E RID: 9550
		[ProtoMember(2)]
		public int LevelUpGetAwardState;

		// Token: 0x0400254F RID: 9551
		[ProtoMember(3)]
		public int KillBossNum;
	}
}
