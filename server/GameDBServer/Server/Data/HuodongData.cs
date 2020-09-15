using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006D RID: 109
	[ProtoContract]
	public class HuodongData
	{
		// Token: 0x0400024F RID: 591
		[ProtoMember(1)]
		public string LastWeekID = "";

		// Token: 0x04000250 RID: 592
		[ProtoMember(2)]
		public string LastDayID = "";

		// Token: 0x04000251 RID: 593
		[ProtoMember(3)]
		public int LoginNum = 0;

		// Token: 0x04000252 RID: 594
		[ProtoMember(4)]
		public int NewStep = 0;

		// Token: 0x04000253 RID: 595
		[ProtoMember(5)]
		public long StepTime = 0L;

		// Token: 0x04000254 RID: 596
		[ProtoMember(6)]
		public int LastMTime = 0;

		// Token: 0x04000255 RID: 597
		[ProtoMember(7)]
		public string CurMID = "";

		// Token: 0x04000256 RID: 598
		[ProtoMember(8)]
		public int CurMTime = 0;

		// Token: 0x04000257 RID: 599
		[ProtoMember(9)]
		public int SongLiID = 0;

		// Token: 0x04000258 RID: 600
		[ProtoMember(10)]
		public int LoginGiftState = 0;

		// Token: 0x04000259 RID: 601
		[ProtoMember(11)]
		public int OnlineGiftState = 0;

		// Token: 0x0400025A RID: 602
		[ProtoMember(12)]
		public int LastLimitTimeHuoDongID = 0;

		// Token: 0x0400025B RID: 603
		[ProtoMember(13)]
		public int LastLimitTimeDayID = 0;

		// Token: 0x0400025C RID: 604
		[ProtoMember(14)]
		public int LimitTimeLoginNum = 0;

		// Token: 0x0400025D RID: 605
		[ProtoMember(15)]
		public int LimitTimeGiftState = 0;

		// Token: 0x0400025E RID: 606
		[ProtoMember(16)]
		public int EveryDayOnLineAwardStep = 0;

		// Token: 0x0400025F RID: 607
		[ProtoMember(17)]
		public int GetEveryDayOnLineAwardDayID = 0;

		// Token: 0x04000260 RID: 608
		[ProtoMember(18)]
		public int SeriesLoginGetAwardStep = 0;

		// Token: 0x04000261 RID: 609
		[ProtoMember(19)]
		public int SeriesLoginAwardDayID = 0;

		// Token: 0x04000262 RID: 610
		[ProtoMember(20)]
		public string SeriesLoginAwardGoodsID = "";

		// Token: 0x04000263 RID: 611
		[ProtoMember(21)]
		public string EveryDayOnLineAwardGoodsID = "";
	}
}
