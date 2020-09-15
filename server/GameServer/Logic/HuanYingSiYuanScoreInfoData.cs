using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020004FF RID: 1279
	[ProtoContract]
	public class HuanYingSiYuanScoreInfoData
	{
		// Token: 0x040021D3 RID: 8659
		[ProtoMember(1)]
		public int Score1;

		// Token: 0x040021D4 RID: 8660
		[ProtoMember(2)]
		public int Score2;

		// Token: 0x040021D5 RID: 8661
		[ProtoMember(3)]
		public long Count1;

		// Token: 0x040021D6 RID: 8662
		[ProtoMember(4)]
		public int Count2;
	}
}
