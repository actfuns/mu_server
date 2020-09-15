using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056E RID: 1390
	[ProtoContract]
	public class LingDiMapInfoData
	{
		// Token: 0x04002568 RID: 9576
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		// Token: 0x04002569 RID: 9577
		[ProtoMember(2)]
		public long FightingStartTime = 0L;

		// Token: 0x0400256A RID: 9578
		[ProtoMember(3)]
		public Dictionary<int, string> BHNameDict = null;
	}
}
