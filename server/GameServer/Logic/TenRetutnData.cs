using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class TenRetutnData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, TenRetutnAwardsData> _tenAwardDic = new Dictionary<int, TenRetutnAwardsData>();

		
		public bool SystemOpen;
	}
}
