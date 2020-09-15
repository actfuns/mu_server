using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000561 RID: 1377
	[ProtoContract]
	public class JingJiChallengeResultData
	{
		// Token: 0x04002510 RID: 9488
		[ProtoMember(1)]
		public int playerId;

		// Token: 0x04002511 RID: 9489
		[ProtoMember(2)]
		public int robotId;

		// Token: 0x04002512 RID: 9490
		[ProtoMember(3)]
		public bool isWin;
	}
}
