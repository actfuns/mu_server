using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005B RID: 91
	[ProtoContract]
	public class GoodsLimitData
	{
		// Token: 0x04000200 RID: 512
		[ProtoMember(1)]
		public int GoodsID;

		// Token: 0x04000201 RID: 513
		[ProtoMember(2)]
		public int DayID;

		// Token: 0x04000202 RID: 514
		[ProtoMember(3)]
		public int UsedNum;
	}
}
