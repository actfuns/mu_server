using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class HeFuAwardTimesActivity : Activity
	{
		
		public bool InActivityList(int value)
		{
			return this.activityList.Contains(value);
		}

		
		public List<int> activityList = new List<int>();

		
		public float activityTimes;

		
		public int specialTimeID;
	}
}
