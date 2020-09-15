using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000713 RID: 1811
	public class HeFuRechargeActivity : Activity
	{
		// Token: 0x06002B5B RID: 11099 RVA: 0x00267F9C File Offset: 0x0026619C
		public HeFuRechargeData getDataByDay(int rank)
		{
			HeFuRechargeData data = null;
			if (this.ConfigDict.ContainsKey(rank))
			{
				data = this.ConfigDict[rank];
			}
			return data;
		}

		// Token: 0x04003A4C RID: 14924
		public Dictionary<int, HeFuRechargeData> ConfigDict = new Dictionary<int, HeFuRechargeData>();

		// Token: 0x04003A4D RID: 14925
		public string strcoe;
	}
}
