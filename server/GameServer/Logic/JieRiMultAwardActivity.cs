using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class JieRiMultAwardActivity : Activity
	{
		
		public JieRiMultConfig GetConfig(int type)
		{
			JieRiMultConfig config = null;
			if (this.activityDict.ContainsKey(type))
			{
				config = this.activityDict[type];
			}
			return config;
		}

		
		public Dictionary<int, JieRiMultConfig> activityDict = new Dictionary<int, JieRiMultConfig>();
	}
}
