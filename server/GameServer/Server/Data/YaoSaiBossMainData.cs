using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F3 RID: 2035
	[ProtoContract]
	public class YaoSaiBossMainData
	{
		// Token: 0x04004385 RID: 17285
		[ProtoMember(1)]
		public YaoSaiBossData BossInfo;

		// Token: 0x04004386 RID: 17286
		[ProtoMember(2)]
		public int TaoFaCount;

		// Token: 0x04004387 RID: 17287
		[ProtoMember(3)]
		public int HaveZhaoHuanCount;

		// Token: 0x04004388 RID: 17288
		[ProtoMember(4)]
		public int ZhaoHuanBossID;

		// Token: 0x04004389 RID: 17289
		[ProtoMember(5)]
		public int OtherID;

		// Token: 0x0400438A RID: 17290
		[ProtoMember(6)]
		public int FightCount;
	}
}
