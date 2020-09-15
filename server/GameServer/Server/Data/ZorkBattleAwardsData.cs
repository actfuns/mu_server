using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000847 RID: 2119
	[ProtoContract]
	public class ZorkBattleAwardsData
	{
		// Token: 0x04004639 RID: 17977
		[ProtoMember(1)]
		public int Success;

		// Token: 0x0400463A RID: 17978
		[ProtoMember(2)]
		public int RankNum;

		// Token: 0x0400463B RID: 17979
		[ProtoMember(3)]
		public int AwardID;

		// Token: 0x0400463C RID: 17980
		[ProtoMember(4)]
		public int SelfJiFen;

		// Token: 0x0400463D RID: 17981
		[ProtoMember(5)]
		public int JiFen;

		// Token: 0x0400463E RID: 17982
		[ProtoMember(6)]
		public int WinToDay;

		// Token: 0x0400463F RID: 17983
		[ProtoMember(7)]
		public List<AwardsItemData> BossAwardGoodsDataList;
	}
}
