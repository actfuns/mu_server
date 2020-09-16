using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class YaoSaiBossRunTimeData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, YaoSaiBossData> RoleBossCacheDict = new Dictionary<int, YaoSaiBossData>();

		
		public Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>> BossZhanDouLogDict = new Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>>();
	}
}
