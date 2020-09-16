using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class CityLevelInfo
	{
		
		public int ID;

		
		public int CityLevel;

		
		public int CityNum;

		
		public int MaxNum;

		
		public List<TimeSpan> BaoMingTime = new List<TimeSpan>();

		
		public List<int> AttackWeekDay;

		
		public List<TimeSpan> AttackTime = new List<TimeSpan>();

		
		public AwardsItemList Award = new AwardsItemList();

		
		public AwardsItemList DayAward = new AwardsItemList();

		
		public int ZhanMengZiJin;
	}
}
