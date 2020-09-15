using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000584 RID: 1412
	[ProtoContract]
	public class QiZhenGeItemData
	{
		// Token: 0x0400261F RID: 9759
		[ProtoMember(1)]
		public int ItemID = 0;

		// Token: 0x04002620 RID: 9760
		[ProtoMember(2)]
		public int GoodsID = 0;

		// Token: 0x04002621 RID: 9761
		[ProtoMember(3)]
		public int OrigPrice = 0;

		// Token: 0x04002622 RID: 9762
		[ProtoMember(4)]
		public int Price = 0;

		// Token: 0x04002623 RID: 9763
		[ProtoMember(5)]
		public string Description = "";

		// Token: 0x04002624 RID: 9764
		[ProtoMember(6)]
		public int BaseProbability = 0;

		// Token: 0x04002625 RID: 9765
		[ProtoMember(7)]
		public int SelfProbability = 0;
	}
}
