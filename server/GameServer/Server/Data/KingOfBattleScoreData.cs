using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200033C RID: 828
	[ProtoContract]
	public class KingOfBattleScoreData
	{
		// Token: 0x040015AA RID: 5546
		[ProtoMember(1)]
		public int Score1;

		// Token: 0x040015AB RID: 5547
		[ProtoMember(2)]
		public int Score2;
	}
}
