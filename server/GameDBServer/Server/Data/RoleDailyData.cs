using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A3 RID: 163
	[ProtoContract]
	public class RoleDailyData
	{
		// Token: 0x04000396 RID: 918
		[ProtoMember(1)]
		public int ExpDayID = 0;

		// Token: 0x04000397 RID: 919
		[ProtoMember(2)]
		public int TodayExp = 0;

		// Token: 0x04000398 RID: 920
		[ProtoMember(3)]
		public int LingLiDayID = 0;

		// Token: 0x04000399 RID: 921
		[ProtoMember(4)]
		public int TodayLingLi = 0;

		// Token: 0x0400039A RID: 922
		[ProtoMember(5)]
		public int KillBossDayID = 0;

		// Token: 0x0400039B RID: 923
		[ProtoMember(6)]
		public int TodayKillBoss = 0;

		// Token: 0x0400039C RID: 924
		[ProtoMember(7)]
		public int FuBenDayID = 0;

		// Token: 0x0400039D RID: 925
		[ProtoMember(8)]
		public int TodayFuBenNum = 0;

		// Token: 0x0400039E RID: 926
		[ProtoMember(9)]
		public int WuXingDayID = 0;

		// Token: 0x0400039F RID: 927
		[ProtoMember(10)]
		public int WuXingNum = 0;

		// Token: 0x040003A0 RID: 928
		[ProtoMember(11)]
		public int RebornExpDayID = 0;

		// Token: 0x040003A1 RID: 929
		[ProtoMember(12)]
		public int RebornExpMonster = 0;

		// Token: 0x040003A2 RID: 930
		[ProtoMember(13)]
		public int RebornExpSale = 0;
	}
}
