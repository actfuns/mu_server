using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E9 RID: 745
	[ProtoContract]
	public class ShowHongBaoData
	{
		// Token: 0x04001323 RID: 4899
		[ProtoMember(1)]
		public int type;

		// Token: 0x04001324 RID: 4900
		[ProtoMember(2)]
		public int hongBaoID;

		// Token: 0x04001325 RID: 4901
		[ProtoMember(3)]
		public string sender;

		// Token: 0x04001326 RID: 4902
		[ProtoMember(4)]
		public string message;

		// Token: 0x04001327 RID: 4903
		[ProtoMember(5)]
		public int yiLingNum;

		// Token: 0x04001328 RID: 4904
		[ProtoMember(6)]
		public int SumHongBaoNum;

		// Token: 0x04001329 RID: 4905
		[ProtoMember(7)]
		public int result;

		// Token: 0x0400132A RID: 4906
		[ProtoMember(8)]
		public int tips;
	}
}
