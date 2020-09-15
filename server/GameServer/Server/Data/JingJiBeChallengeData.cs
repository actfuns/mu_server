using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200055F RID: 1375
	[ProtoContract]
	public class JingJiBeChallengeData
	{
		// Token: 0x04002509 RID: 9481
		[ProtoMember(1)]
		public int state;

		// Token: 0x0400250A RID: 9482
		[ProtoMember(2)]
		public PlayerJingJiData beChallengerData = null;
	}
}
