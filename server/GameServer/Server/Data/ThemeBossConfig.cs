using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x02000444 RID: 1092
	public class ThemeBossConfig
	{
		// Token: 0x04001D76 RID: 7542
		public const int ApplyOverTime = 180;

		// Token: 0x04001D77 RID: 7543
		public int ID;

		// Token: 0x04001D78 RID: 7544
		public int MonstersID;

		// Token: 0x04001D79 RID: 7545
		public int MaxUnionLevel;

		// Token: 0x04001D7A RID: 7546
		public int MapCode;

		// Token: 0x04001D7B RID: 7547
		public int PosX;

		// Token: 0x04001D7C RID: 7548
		public int PosY;

		// Token: 0x04001D7D RID: 7549
		public int Radius;

		// Token: 0x04001D7E RID: 7550
		public int Num;

		// Token: 0x04001D7F RID: 7551
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04001D80 RID: 7552
		public List<double> SecondsOfDay = new List<double>();
	}
}
