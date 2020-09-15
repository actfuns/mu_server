using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B9 RID: 185
	[ProtoContract]
	public class TaskAwardsData
	{
		// Token: 0x040004DB RID: 1243
		[ProtoMember(1)]
		public List<AwardsItemData> TaskawardList = null;

		// Token: 0x040004DC RID: 1244
		[ProtoMember(2)]
		public List<AwardsItemData> OtherTaskawardList = null;

		// Token: 0x040004DD RID: 1245
		[ProtoMember(3)]
		public int Moneyaward = 0;

		// Token: 0x040004DE RID: 1246
		[ProtoMember(4)]
		public int Experienceaward = 0;

		// Token: 0x040004DF RID: 1247
		[ProtoMember(5)]
		public int YinLiangaward = 0;

		// Token: 0x040004E0 RID: 1248
		[ProtoMember(6)]
		public int LingLiaward = 0;

		// Token: 0x040004E1 RID: 1249
		[ProtoMember(7)]
		public int BindYuanBaoaward = 0;

		// Token: 0x040004E2 RID: 1250
		[ProtoMember(8)]
		public int ZhenQiaward = 0;

		// Token: 0x040004E3 RID: 1251
		[ProtoMember(9)]
		public int LieShaaward = 0;

		// Token: 0x040004E4 RID: 1252
		[ProtoMember(10)]
		public int WuXingaward = 0;

		// Token: 0x040004E5 RID: 1253
		[ProtoMember(11)]
		public int NeedYuanBao = 0;

		// Token: 0x040004E6 RID: 1254
		[ProtoMember(12)]
		public int JunGongaward = 0;

		// Token: 0x040004E7 RID: 1255
		[ProtoMember(13)]
		public int RongYuaward = 0;

		// Token: 0x040004E8 RID: 1256
		[ProtoMember(14)]
		public int AddExperienceForDailyCircleTask = 0;

		// Token: 0x040004E9 RID: 1257
		[ProtoMember(15)]
		public int AddBindYuanBaoForDailyCircleTask = 0;

		// Token: 0x040004EA RID: 1258
		[ProtoMember(16)]
		public int AddGoodsForDailyCircleTask = 0;
	}
}
