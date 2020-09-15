using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000073 RID: 115
	[ProtoContract]
	public class JingJiBeChallengeData
	{
		// Token: 0x04000278 RID: 632
		[ProtoMember(1)]
		public int state;

		// Token: 0x04000279 RID: 633
		[ProtoMember(2)]
		public PlayerJingJiData beChallengerData = null;
	}
}
