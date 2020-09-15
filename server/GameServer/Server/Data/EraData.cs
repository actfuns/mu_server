using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200029D RID: 669
	[ProtoContract]
	public class EraData
	{
		// Token: 0x04001075 RID: 4213
		[ProtoMember(1)]
		public int EraID;

		// Token: 0x04001076 RID: 4214
		[ProtoMember(2)]
		public byte EraStage;

		// Token: 0x04001077 RID: 4215
		[ProtoMember(3)]
		public int EraStageProcess;

		// Token: 0x04001078 RID: 4216
		[ProtoMember(4)]
		public byte FastEraStage;

		// Token: 0x04001079 RID: 4217
		[ProtoMember(5)]
		public int FastEraStateProcess;

		// Token: 0x0400107A RID: 4218
		[ProtoMember(6)]
		public List<EraTaskData> EraTaskList = new List<EraTaskData>();

		// Token: 0x0400107B RID: 4219
		[ProtoMember(7)]
		public List<EraRankData> EraRankList = new List<EraRankData>();

		// Token: 0x0400107C RID: 4220
		[ProtoMember(8)]
		public Dictionary<int, int> EraAwardStateDict = new Dictionary<int, int>();
	}
}
