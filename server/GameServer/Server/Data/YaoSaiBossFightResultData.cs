using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F5 RID: 2037
	[ProtoContract]
	public class YaoSaiBossFightResultData
	{
		// Token: 0x0400438F RID: 17295
		[ProtoMember(1)]
		public int Result;

		// Token: 0x04004390 RID: 17296
		[ProtoMember(2)]
		public int FightLife;

		// Token: 0x04004391 RID: 17297
		[ProtoMember(3)]
		public YaoSaiBossData BossInfo;

		// Token: 0x04004392 RID: 17298
		[ProtoMember(4)]
		public bool NeedNotifyAward;
	}
}
