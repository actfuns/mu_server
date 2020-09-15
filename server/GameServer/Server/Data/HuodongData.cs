using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200055D RID: 1373
	[ProtoContract]
	public class HuodongData
	{
		// Token: 0x040024F2 RID: 9458
		[ProtoMember(1)]
		public string LastWeekID = "";

		// Token: 0x040024F3 RID: 9459
		[ProtoMember(2)]
		public string LastDayID = "";

		// Token: 0x040024F4 RID: 9460
		[ProtoMember(3)]
		public int LoginNum = 0;

		// Token: 0x040024F5 RID: 9461
		[ProtoMember(4)]
		public int NewStep = 0;

		// Token: 0x040024F6 RID: 9462
		[ProtoMember(5)]
		public long StepTime = 0L;

		// Token: 0x040024F7 RID: 9463
		[ProtoMember(6)]
		public int LastMTime = 0;

		// Token: 0x040024F8 RID: 9464
		[ProtoMember(7)]
		public string CurMID = "";

		// Token: 0x040024F9 RID: 9465
		[ProtoMember(8)]
		public int CurMTime = 0;

		// Token: 0x040024FA RID: 9466
		[ProtoMember(9)]
		public int SongLiID = 0;

		// Token: 0x040024FB RID: 9467
		[ProtoMember(10)]
		public int LoginGiftState = 0;

		// Token: 0x040024FC RID: 9468
		[ProtoMember(11)]
		public int OnlineGiftState = 0;

		// Token: 0x040024FD RID: 9469
		[ProtoMember(12)]
		public int LastLimitTimeHuoDongID = 0;

		// Token: 0x040024FE RID: 9470
		[ProtoMember(13)]
		public int LastLimitTimeDayID = 0;

		// Token: 0x040024FF RID: 9471
		[ProtoMember(14)]
		public int LimitTimeLoginNum = 0;

		// Token: 0x04002500 RID: 9472
		[ProtoMember(15)]
		public int LimitTimeGiftState = 0;

		// Token: 0x04002501 RID: 9473
		[ProtoMember(16)]
		public int EveryDayOnLineAwardStep = 0;

		// Token: 0x04002502 RID: 9474
		[ProtoMember(17)]
		public int GetEveryDayOnLineAwardDayID = 0;

		// Token: 0x04002503 RID: 9475
		[ProtoMember(18)]
		public int SeriesLoginGetAwardStep = 0;

		// Token: 0x04002504 RID: 9476
		[ProtoMember(19)]
		public int SeriesLoginAwardDayID = 0;

		// Token: 0x04002505 RID: 9477
		[ProtoMember(20)]
		public string SeriesLoginAwardGoodsID = "";

		// Token: 0x04002506 RID: 9478
		[ProtoMember(21)]
		public string EveryDayOnLineAwardGoodsID = "";
	}
}
