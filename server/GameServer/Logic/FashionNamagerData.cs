using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004E5 RID: 1253
	public class FashionNamagerData
	{
		// Token: 0x04002138 RID: 8504
		public object Mutex = new object();

		// Token: 0x04002139 RID: 8505
		public Dictionary<int, FashionTabData> FashionTabDict = new Dictionary<int, FashionTabData>();

		// Token: 0x0400213A RID: 8506
		public Dictionary<int, FashionData> FashingDict = new Dictionary<int, FashionData>();

		// Token: 0x0400213B RID: 8507
		public Dictionary<KeyValuePair<int, int>, FashionBagData> FashionBagDict = new Dictionary<KeyValuePair<int, int>, FashionBagData>();

		// Token: 0x0400213C RID: 8508
		public Dictionary<int, int> SpecialTitleDict = new Dictionary<int, int>();

		// Token: 0x0400213D RID: 8509
		public int LuoLanChengZhuRoleID = 0;
	}
}
