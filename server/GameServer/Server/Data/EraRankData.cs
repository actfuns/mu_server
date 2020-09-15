using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200029C RID: 668
	[ProtoContract]
	public class EraRankData
	{
		// Token: 0x04001070 RID: 4208
		[ProtoMember(1)]
		public int RankValue;

		// Token: 0x04001071 RID: 4209
		[ProtoMember(2)]
		public int JunTuanID;

		// Token: 0x04001072 RID: 4210
		[ProtoMember(3)]
		public string JunTuanName;

		// Token: 0x04001073 RID: 4211
		[ProtoMember(4)]
		public byte EraStage;

		// Token: 0x04001074 RID: 4212
		[ProtoMember(5)]
		public int EraStageProcess;
	}
}
