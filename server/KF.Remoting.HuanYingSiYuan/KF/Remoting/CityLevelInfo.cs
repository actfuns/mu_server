using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	// Token: 0x02000089 RID: 137
	public class CityLevelInfo
	{
		// Token: 0x040003D0 RID: 976
		public int ID;

		// Token: 0x040003D1 RID: 977
		public int CityLevel;

		// Token: 0x040003D2 RID: 978
		public int CityNum;

		// Token: 0x040003D3 RID: 979
		public int MaxNum;

		// Token: 0x040003D4 RID: 980
		public int[] AttackWeekDay;

		// Token: 0x040003D5 RID: 981
		public List<TimeSpan> BaoMingTime = new List<TimeSpan>();

		// Token: 0x040003D6 RID: 982
		public List<TimeSpan> AttackTime = new List<TimeSpan>();
	}
}
