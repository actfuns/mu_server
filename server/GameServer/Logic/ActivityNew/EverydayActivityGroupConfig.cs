using System;
using System.Collections.Generic;

namespace GameServer.Logic.ActivityNew
{
	
	public class EverydayActivityGroupConfig
	{
		
		public int GroupID = 0;

		
		public int TypeID = 0;

		
		public int NeedType = 0;

		
		public EveryActLimitData NeedNum = new EveryActLimitData();

		
		public List<int> ActivityIDList = new List<int>();
	}
}
