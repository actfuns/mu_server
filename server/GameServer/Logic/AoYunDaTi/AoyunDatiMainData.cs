using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000201 RID: 513
	[ProtoContract]
	public class AoyunDatiMainData
	{
		// Token: 0x04000B5B RID: 2907
		[ProtoMember(1)]
		public List<AoyunPaiHangRoleData> AoyunPaiHangRoleDataArray;

		// Token: 0x04000B5C RID: 2908
		[ProtoMember(2)]
		public DateTime StartTime;

		// Token: 0x04000B5D RID: 2909
		[ProtoMember(3)]
		public DateTime EndTime;

		// Token: 0x04000B5E RID: 2910
		[ProtoMember(4)]
		public int SelfRank;

		// Token: 0x04000B5F RID: 2911
		[ProtoMember(5)]
		public int TianShiNum;

		// Token: 0x04000B60 RID: 2912
		[ProtoMember(6)]
		public int EMoNum;

		// Token: 0x04000B61 RID: 2913
		[ProtoMember(7)]
		public DateTime NextStartTime;

		// Token: 0x04000B62 RID: 2914
		[ProtoMember(8)]
		public bool IsHaveAward;
	}
}
