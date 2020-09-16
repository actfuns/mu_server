using System;

namespace GameServer.Logic
{
	
	public class MagicHelperItem
	{
		
		
		
		public MagicActionIDs MagicActionID { get; set; }

		
		
		
		public double[] MagicActionParams { get; set; }

		
		
		
		public long StartedTicks { get; set; }

		
		
		
		public long LastTicks { get; set; }

		
		
		
		public int ExecutedNum { get; set; }

		
		
		
		public int ObjectID { get; set; }
	}
}
