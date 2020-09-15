using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000074 RID: 116
	[ProtoContract]
	public class JingJiChallengeResultData
	{
		// Token: 0x0400027A RID: 634
		[ProtoMember(1)]
		public int playerId;

		// Token: 0x0400027B RID: 635
		[ProtoMember(2)]
		public int robotId;

		// Token: 0x0400027C RID: 636
		[ProtoMember(3)]
		public bool isWin;
	}
}
