using System;

namespace GameServer.Logic
{
	// Token: 0x020005DA RID: 1498
	public class BroadcastInfoItem
	{
		// Token: 0x04002A16 RID: 10774
		public int ID = 0;

		// Token: 0x04002A17 RID: 10775
		public int InfoClass = 0;

		// Token: 0x04002A18 RID: 10776
		public int HintErrID = -1;

		// Token: 0x04002A19 RID: 10777
		public int TimeType = 0;

		// Token: 0x04002A1A RID: 10778
		public int KaiFuStartDay = -1;

		// Token: 0x04002A1B RID: 10779
		public int KaiFuShowType = -1;

		// Token: 0x04002A1C RID: 10780
		public string WeekDays = "";

		// Token: 0x04002A1D RID: 10781
		public BroadcastTimeItem[] Times = null;

		// Token: 0x04002A1E RID: 10782
		public DateTimeRange[] OnlineNoticeTimeRanges = null;

		// Token: 0x04002A1F RID: 10783
		public string Text = "";

		// Token: 0x04002A20 RID: 10784
		public int MinZhuanSheng = 0;

		// Token: 0x04002A21 RID: 10785
		public int MinLevel = 0;
	}
}
