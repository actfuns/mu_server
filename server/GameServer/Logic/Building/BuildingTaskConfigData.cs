using System;

namespace GameServer.Logic.Building
{
	
	public class BuildingTaskConfigData
	{
		
		public int TaskID = 0;

		
		public int BuildID = 0;

		
		public BuildingQuality quality = BuildingQuality.Null;

		
		public double SumNum = 0.0;

		
		public double ExpNum = 0.0;

		
		public int Time = 0;

		
		public bool RandSkip = false;
	}
}
