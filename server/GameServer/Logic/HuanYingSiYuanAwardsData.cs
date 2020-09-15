using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000500 RID: 1280
	[ProtoContract]
	public class HuanYingSiYuanAwardsData
	{
		// Token: 0x040021D7 RID: 8663
		[ProtoMember(1)]
		public int SuccessSide;

		// Token: 0x040021D8 RID: 8664
		[ProtoMember(2)]
		public long Exp;

		// Token: 0x040021D9 RID: 8665
		[ProtoMember(3)]
		public int ShengWang;

		// Token: 0x040021DA RID: 8666
		[ProtoMember(4)]
		public int ChengJiuAward;

		// Token: 0x040021DB RID: 8667
		[ProtoMember(5)]
		public int AwardsRate;

		// Token: 0x040021DC RID: 8668
		[ProtoMember(6)]
		public string AwardGoods;
	}
}
