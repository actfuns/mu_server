using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020002BD RID: 701
	[ProtoContract]
	public class FundItem
	{
		// Token: 0x0400120E RID: 4622
		[ProtoMember(1, IsRequired = true)]
		public int FundType = 0;

		// Token: 0x0400120F RID: 4623
		[ProtoMember(2, IsRequired = true)]
		public int BuyType = 0;

		// Token: 0x04001210 RID: 4624
		[ProtoMember(3, IsRequired = true)]
		public DateTime BuyTime = DateTime.MinValue;

		// Token: 0x04001211 RID: 4625
		[ProtoMember(4, IsRequired = true)]
		public int FundID = 0;

		// Token: 0x04001212 RID: 4626
		[ProtoMember(5, IsRequired = true)]
		public int AwardID = 0;

		// Token: 0x04001213 RID: 4627
		[ProtoMember(6, IsRequired = true)]
		public int AwardType = 0;

		// Token: 0x04001214 RID: 4628
		[ProtoMember(7, IsRequired = true)]
		public int Value1 = 0;

		// Token: 0x04001215 RID: 4629
		[ProtoMember(8, IsRequired = true)]
		public int Value2 = 0;
	}
}
