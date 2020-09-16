using System;

namespace GameServer.Logic
{
	
	public class InterestingData
	{
		
		public InterestingData()
		{
			this.itemArray = new InterestingData.Item[2];
			for (int i = 0; i < 2; i++)
			{
				this.itemArray[i] = new InterestingData.Item();
			}
		}

		
		public InterestingData.Item[] itemArray = null;

		
		public double Speed = 0.0;

		
		public class Item
		{
			
			public int RequestCount = 0;

			
			public int ResponseCount = 0;

			
			public long LastRequestMs = 0L;

			
			public long LastResponseMs = 0L;

			
			public int InvalidCount = 0;
		}
	}
}
