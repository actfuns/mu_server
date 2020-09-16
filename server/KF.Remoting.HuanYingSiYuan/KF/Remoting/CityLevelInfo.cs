using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	public class CityLevelInfo
	{
		
		public int ID;

		
		public int CityLevel;

		
		public int CityNum;

		
		public int MaxNum;

		
		public int[] AttackWeekDay;

		
		public List<TimeSpan> BaoMingTime = new List<TimeSpan>();

		
		public List<TimeSpan> AttackTime = new List<TimeSpan>();
	}
}
