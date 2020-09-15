using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000556 RID: 1366
	[ProtoContract]
	public class FuBenTongGuanData
	{
		// Token: 0x040024A9 RID: 9385
		[ProtoMember(1)]
		public int FuBenID;

		// Token: 0x040024AA RID: 9386
		[ProtoMember(2)]
		public int TotalScore;

		// Token: 0x040024AB RID: 9387
		[ProtoMember(3)]
		public int KillNum;

		// Token: 0x040024AC RID: 9388
		[ProtoMember(4)]
		public int KillScore;

		// Token: 0x040024AD RID: 9389
		[ProtoMember(5)]
		public int MaxKillScore;

		// Token: 0x040024AE RID: 9390
		[ProtoMember(6)]
		public int UsedSecs;

		// Token: 0x040024AF RID: 9391
		[ProtoMember(7)]
		public int TimeScore;

		// Token: 0x040024B0 RID: 9392
		[ProtoMember(8)]
		public int MaxTimeScore;

		// Token: 0x040024B1 RID: 9393
		[ProtoMember(9)]
		public int DieCount;

		// Token: 0x040024B2 RID: 9394
		[ProtoMember(10)]
		public int DieScore;

		// Token: 0x040024B3 RID: 9395
		[ProtoMember(11)]
		public int MaxDieScore;

		// Token: 0x040024B4 RID: 9396
		[ProtoMember(12)]
		public List<int> GoodsIDList;

		// Token: 0x040024B5 RID: 9397
		[ProtoMember(13)]
		public int AwardExp;

		// Token: 0x040024B6 RID: 9398
		[ProtoMember(14)]
		public int AwardJinBi;

		// Token: 0x040024B7 RID: 9399
		[ProtoMember(15)]
		public int AwardXingHun = 0;

		// Token: 0x040024B8 RID: 9400
		[ProtoMember(16)]
		public int ResultMark = 0;

		// Token: 0x040024B9 RID: 9401
		[ProtoMember(17)]
		public int MapCode = 0;

		// Token: 0x040024BA RID: 9402
		[ProtoMember(18)]
		public int AwardZhanGong = 0;

		// Token: 0x040024BB RID: 9403
		[ProtoMember(19)]
		public double AwardRate = 1.0;

		// Token: 0x040024BC RID: 9404
		[ProtoMember(20)]
		public int TreasureEventID = 0;

		// Token: 0x040024BD RID: 9405
		[ProtoMember(21)]
		public int AwardMoJing = 0;
	}
}
