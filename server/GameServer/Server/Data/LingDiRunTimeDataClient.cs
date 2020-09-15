using System;

namespace Server.Data
{
	// Token: 0x02000312 RID: 786
	public class LingDiRunTimeDataClient
	{
		// Token: 0x04001425 RID: 5157
		public object Mutex = new object();

		// Token: 0x04001426 RID: 5158
		public bool[] DoubleOpenState;

		// Token: 0x04001427 RID: 5159
		public DoubleOpenItem[] DoubleOpenTime;
	}
}
