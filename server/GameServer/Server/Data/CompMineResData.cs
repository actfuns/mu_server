using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000279 RID: 633
	[ProtoContract]
	public class CompMineResData
	{
		// Token: 0x04000FD4 RID: 4052
		[ProtoMember(1)]
		public int CompType;

		// Token: 0x04000FD5 RID: 4053
		[ProtoMember(2)]
		public int MineRes;

		// Token: 0x04000FD6 RID: 4054
		public int Rank;
	}
}
