using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200033D RID: 829
	[ProtoContract]
	public class KingOfBattleAwardsData
	{
		// Token: 0x040015AC RID: 5548
		[ProtoMember(1)]
		public int Success;

		// Token: 0x040015AD RID: 5549
		[ProtoMember(2)]
		public int BindJinBi;

		// Token: 0x040015AE RID: 5550
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x040015AF RID: 5551
		[ProtoMember(4)]
		public List<AwardsItemData> AwardsItemDataList;

		// Token: 0x040015B0 RID: 5552
		[ProtoMember(5)]
		public int SideScore1;

		// Token: 0x040015B1 RID: 5553
		[ProtoMember(6)]
		public int SideScore2;

		// Token: 0x040015B2 RID: 5554
		[ProtoMember(7)]
		public int SelfScore;

		// Token: 0x040015B3 RID: 5555
		[ProtoMember(8)]
		public string MvpRoleName;

		// Token: 0x040015B4 RID: 5556
		[ProtoMember(9)]
		public int MvpOccupation;

		// Token: 0x040015B5 RID: 5557
		[ProtoMember(10)]
		public int MvpRoleSex;
	}
}
