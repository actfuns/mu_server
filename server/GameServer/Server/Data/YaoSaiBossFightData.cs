using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F4 RID: 2036
	[ProtoContract]
	public class YaoSaiBossFightData
	{
		// Token: 0x0400438B RID: 17291
		[ProtoMember(1)]
		public YaoSaiBossData BossMiniInfo;

		// Token: 0x0400438C RID: 17292
		[ProtoMember(2)]
		public string JingLingZhenRong;

		// Token: 0x0400438D RID: 17293
		[ProtoMember(3)]
		public int HaveFightTime;

		// Token: 0x0400438E RID: 17294
		[ProtoMember(4)]
		public int ZuanShiFightCost;
	}
}
