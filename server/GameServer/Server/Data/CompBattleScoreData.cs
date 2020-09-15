using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000284 RID: 644
	[ProtoContract]
	public class CompBattleScoreData
	{
		// Token: 0x04000FFD RID: 4093
		[ProtoMember(1)]
		public long Score1;

		// Token: 0x04000FFE RID: 4094
		[ProtoMember(2)]
		public long Score2;

		// Token: 0x04000FFF RID: 4095
		[ProtoMember(3)]
		public long Score3;

		// Token: 0x04001000 RID: 4096
		[ProtoMember(4)]
		public long BossMaxLifeV;
	}
}
