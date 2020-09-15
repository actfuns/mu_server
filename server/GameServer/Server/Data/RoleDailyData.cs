using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000585 RID: 1413
	[ProtoContract]
	public class RoleDailyData
	{
		// Token: 0x04002626 RID: 9766
		[ProtoMember(1)]
		public int ExpDayID = 0;

		// Token: 0x04002627 RID: 9767
		[ProtoMember(2)]
		public int TodayExp = 0;

		// Token: 0x04002628 RID: 9768
		[ProtoMember(3)]
		public int LingLiDayID = 0;

		// Token: 0x04002629 RID: 9769
		[ProtoMember(4)]
		public int TodayLingLi = 0;

		// Token: 0x0400262A RID: 9770
		[ProtoMember(5)]
		public int KillBossDayID = 0;

		// Token: 0x0400262B RID: 9771
		[ProtoMember(6)]
		public int TodayKillBoss = 0;

		// Token: 0x0400262C RID: 9772
		[ProtoMember(7)]
		public int FuBenDayID = 0;

		// Token: 0x0400262D RID: 9773
		[ProtoMember(8)]
		public int TodayFuBenNum = 0;

		// Token: 0x0400262E RID: 9774
		[ProtoMember(9)]
		public int WuXingDayID = 0;

		// Token: 0x0400262F RID: 9775
		[ProtoMember(10)]
		public int WuXingNum = 0;

		// Token: 0x04002630 RID: 9776
		[ProtoMember(11)]
		public int RebornExpDayID = 0;

		// Token: 0x04002631 RID: 9777
		[ProtoMember(12)]
		public int RebornExpMonster = 0;

		// Token: 0x04002632 RID: 9778
		[ProtoMember(13)]
		public int RebornExpSale = 0;
	}
}
