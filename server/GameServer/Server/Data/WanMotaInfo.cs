using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056D RID: 1389
	[ProtoContract]
	public class WanMotaInfo
	{
		// Token: 0x04002560 RID: 9568
		[ProtoMember(1)]
		public int nRoleID;

		// Token: 0x04002561 RID: 9569
		[ProtoMember(2)]
		public string strRoleName;

		// Token: 0x04002562 RID: 9570
		[ProtoMember(3)]
		public long lFlushTime;

		// Token: 0x04002563 RID: 9571
		[ProtoMember(4)]
		public int nPassLayerCount;

		// Token: 0x04002564 RID: 9572
		[ProtoMember(5)]
		public int nSweepLayer;

		// Token: 0x04002565 RID: 9573
		[ProtoMember(6)]
		public string strSweepReward = "";

		// Token: 0x04002566 RID: 9574
		[ProtoMember(7)]
		public long lSweepBeginTime;

		// Token: 0x04002567 RID: 9575
		[ProtoMember(8)]
		public int nTopPassLayerCount;
	}
}
