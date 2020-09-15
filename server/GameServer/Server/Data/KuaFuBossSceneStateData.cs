using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000346 RID: 838
	[ProtoContract]
	public class KuaFuBossSceneStateData
	{
		// Token: 0x04001607 RID: 5639
		[ProtoMember(1)]
		public int BossNum;

		// Token: 0x04001608 RID: 5640
		[ProtoMember(2)]
		public int TotalBossNum;

		// Token: 0x04001609 RID: 5641
		[ProtoMember(3)]
		public int MonsterNum;

		// Token: 0x0400160A RID: 5642
		[ProtoMember(4)]
		public int TotalNormalNum;
	}
}
