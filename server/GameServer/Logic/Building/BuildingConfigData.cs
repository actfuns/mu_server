using System;
using System.Collections.Generic;

namespace GameServer.Logic.Building
{
	
	public class BuildingConfigData
	{
		
		public int BuildID = 0;

		
		public int MaxLevel = 0;

		
		public List<BuildingRandomData> FreeRandomList = new List<BuildingRandomData>();

		
		public List<BuildingRandomData> RandomList = new List<BuildingRandomData>();
	}
}
