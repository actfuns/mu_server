using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000590 RID: 1424
	[ProtoContract]
	public class ShiLianTaAwardsInfoData
	{
		// Token: 0x0400281A RID: 10266
		[ProtoMember(1)]
		public int CurrentFloorTotalMonsterNum = 0;

		// Token: 0x0400281B RID: 10267
		[ProtoMember(2)]
		public int CurrentFloorExperienceAward = 0;

		// Token: 0x0400281C RID: 10268
		[ProtoMember(3)]
		public int NextFloorNeedGoodsID = 0;

		// Token: 0x0400281D RID: 10269
		[ProtoMember(4)]
		public int NextFloorNeedGoodsNum = 0;

		// Token: 0x0400281E RID: 10270
		[ProtoMember(5)]
		public int NextFloorExperienceAward = 0;
	}
}
