using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class LimitAnalysisData
	{
		
		public DateTime Timestamp = TimeUtil.NowDateTime();

		
		public Dictionary<string, int> dict = new Dictionary<string, int>();
	}
}
