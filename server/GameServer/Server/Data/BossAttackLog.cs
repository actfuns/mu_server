using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class BossAttackLog
	{
		
		public long InjureSum;

		
		public Dictionary<long, BHAttackLog> BHInjure;

		
		public List<BHAttackLog> BHAttackRank;
	}
}
