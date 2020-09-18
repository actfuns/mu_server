using System;

namespace GameServer.Logic.ActivityNew
{
	
	public class SpecActLimitData
	{
		
		public bool IfValid()
		{
			return this.MinFirst > 0 || this.MaxFirst > 0 || this.MinSecond > 0 || this.MaxSecond > 0;
		}

		
		public int MinFirst = -1;

		
		public int MaxFirst = -1;

		
		public int MinSecond = -1;

		
		public int MaxSecond = -1;
	}
}
