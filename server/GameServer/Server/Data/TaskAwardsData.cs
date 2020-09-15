using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A3 RID: 1443
	[ProtoContract]
	public class TaskAwardsData
	{
		// Token: 0x0400289B RID: 10395
		[ProtoMember(1)]
		public List<AwardsItemData> TaskawardList = null;

		// Token: 0x0400289C RID: 10396
		[ProtoMember(2)]
		public List<AwardsItemData> OtherTaskawardList = null;

		// Token: 0x0400289D RID: 10397
		[ProtoMember(3)]
		public int Moneyaward = 0;

		// Token: 0x0400289E RID: 10398
		[ProtoMember(4)]
		public long Experienceaward = 0L;

		// Token: 0x0400289F RID: 10399
		[ProtoMember(5)]
		public int YinLiangaward = 0;

		// Token: 0x040028A0 RID: 10400
		[ProtoMember(6)]
		public int LingLiaward = 0;

		// Token: 0x040028A1 RID: 10401
		[ProtoMember(7)]
		public int BindYuanBaoaward = 0;

		// Token: 0x040028A2 RID: 10402
		[ProtoMember(8)]
		public int ZhenQiaward = 0;

		// Token: 0x040028A3 RID: 10403
		[ProtoMember(9)]
		public int LieShaaward = 0;

		// Token: 0x040028A4 RID: 10404
		[ProtoMember(10)]
		public int WuXingaward = 0;

		// Token: 0x040028A5 RID: 10405
		[ProtoMember(11)]
		public int NeedYuanBao = 0;

		// Token: 0x040028A6 RID: 10406
		[ProtoMember(12)]
		public int JunGongaward = 0;

		// Token: 0x040028A7 RID: 10407
		[ProtoMember(13)]
		public int RongYuaward = 0;

		// Token: 0x040028A8 RID: 10408
		[ProtoMember(14)]
		public int AddExperienceForDailyCircleTask = 0;

		// Token: 0x040028A9 RID: 10409
		[ProtoMember(15)]
		public int AddMoJingForDailyCircleTask = 0;

		// Token: 0x040028AA RID: 10410
		[ProtoMember(16)]
		public string AddGoodsForDailyCircleTask = "";

		// Token: 0x040028AB RID: 10411
		[ProtoMember(17)]
		public int MoJingaward = 0;

		// Token: 0x040028AC RID: 10412
		[ProtoMember(18)]
		public int XingHunaward = 0;

		// Token: 0x040028AD RID: 10413
		[ProtoMember(19)]
		public int FenMoAward = 0;

		// Token: 0x040028AE RID: 10414
		[ProtoMember(20)]
		public int ShengwangAward = 0;

		// Token: 0x040028AF RID: 10415
		[ProtoMember(21)]
		public int CompDonate = 0;

		// Token: 0x040028B0 RID: 10416
		[ProtoMember(22)]
		public int CompJunXian = 0;
	}
}
