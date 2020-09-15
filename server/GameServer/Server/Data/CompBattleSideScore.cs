using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200027D RID: 637
	[ProtoContract]
	public class CompBattleSideScore
	{
		// Token: 0x04000FE3 RID: 4067
		[ProtoMember(1)]
		public int CompType;

		// Token: 0x04000FE4 RID: 4068
		[ProtoMember(2)]
		public int ZhuJiangNum;

		// Token: 0x04000FE5 RID: 4069
		[ProtoMember(3)]
		public HashSet<int> StrongholdSet = new HashSet<int>();

		// Token: 0x04000FE6 RID: 4070
		public int Rank;

		// Token: 0x04000FE7 RID: 4071
		public double Rate;
	}
}
