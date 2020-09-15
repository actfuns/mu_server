using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200081D RID: 2077
	[ProtoContract]
	public class YongZheZhanChangAwardsData
	{
		// Token: 0x040044C2 RID: 17602
		[ProtoMember(1)]
		public int Success;

		// Token: 0x040044C3 RID: 17603
		[ProtoMember(2)]
		public int BindJinBi;

		// Token: 0x040044C4 RID: 17604
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x040044C5 RID: 17605
		[ProtoMember(4)]
		public List<AwardsItemData> AwardsItemDataList;

		// Token: 0x040044C6 RID: 17606
		[ProtoMember(5)]
		public int SideScore1;

		// Token: 0x040044C7 RID: 17607
		[ProtoMember(6)]
		public int SideScore2;

		// Token: 0x040044C8 RID: 17608
		[ProtoMember(7)]
		public int SelfScore;

		// Token: 0x040044C9 RID: 17609
		[ProtoMember(8)]
		public string MvpRoleName;

		// Token: 0x040044CA RID: 17610
		[ProtoMember(9)]
		public int MvpOccupation;

		// Token: 0x040044CB RID: 17611
		[ProtoMember(10)]
		public int MvpRoleSex;
	}
}
