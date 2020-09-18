using System;

namespace GameServer.Logic.ActivityNew
{
	
	public class EveryActGoalData
	{
		
		public bool IsValid()
		{
			return this.NumOne > 0 || this.NumTwo > 0;
		}

		
		public int NumOne = 0;

		
		public int NumTwo = 0;
	}
}
