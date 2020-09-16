using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class YaoSaiMissionRunTimeData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, List<YaoSaiMissionData>> RoleMissionCacheDict = new Dictionary<int, List<YaoSaiMissionData>>();

		
		public SortedList<long, RoleMissionData> MissionSortList = new SortedList<long, RoleMissionData>();
	}
}
