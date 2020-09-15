using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000837 RID: 2103
	[ProtoContract]
	public class WanMoXiaGuScoreData
	{
		// Token: 0x040045A8 RID: 17832
		[ProtoMember(1)]
		public double BossLifePercent;

		// Token: 0x040045A9 RID: 17833
		[ProtoMember(2)]
		public int MonsterID;

		// Token: 0x040045AA RID: 17834
		[ProtoMember(3)]
		public int MonsterCount;

		// Token: 0x040045AB RID: 17835
		[ProtoMember(4)]
		public int Decorations;

		// Token: 0x040045AC RID: 17836
		[ProtoMember(5)]
		public string Intro;
	}
}
