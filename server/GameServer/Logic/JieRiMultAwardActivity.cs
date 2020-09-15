using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200070A RID: 1802
	public class JieRiMultAwardActivity : Activity
	{
		// Token: 0x06002B47 RID: 11079 RVA: 0x00267B44 File Offset: 0x00265D44
		public JieRiMultConfig GetConfig(int type)
		{
			JieRiMultConfig config = null;
			if (this.activityDict.ContainsKey(type))
			{
				config = this.activityDict[type];
			}
			return config;
		}

		// Token: 0x04003A3B RID: 14907
		public Dictionary<int, JieRiMultConfig> activityDict = new Dictionary<int, JieRiMultConfig>();
	}
}
