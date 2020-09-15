using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x0200036C RID: 876
	[ProtoContract]
	public class CoupleWishWishReqData
	{
		// Token: 0x04001728 RID: 5928
		[ProtoMember(1)]
		public bool IsWishRankRole;

		// Token: 0x04001729 RID: 5929
		[ProtoMember(2)]
		public int ToRankCoupleId;

		// Token: 0x0400172A RID: 5930
		[ProtoMember(3)]
		public string ToLocalRoleName;

		// Token: 0x0400172B RID: 5931
		[ProtoMember(4)]
		public int WishType;

		// Token: 0x0400172C RID: 5932
		[ProtoMember(5)]
		public string WishTxt;

		// Token: 0x0400172D RID: 5933
		[ProtoMember(6)]
		public int CostType;

		// Token: 0x0200036D RID: 877
		public enum ECostType
		{
			// Token: 0x0400172F RID: 5935
			Goods = 1,
			// Token: 0x04001730 RID: 5936
			ZuanShi
		}
	}
}
