using System;

namespace GameServer.Logic
{
	
	public class BuffItemData
	{
		
		public int buffId;

		
		public int buffSecs;

		
		public long startTicks;

		
		public long buffVal;

		
		public long endTicks;

		
		public bool enabled;

		
		public bool clientEnabledState;

		
		public bool enabledByTime;

		
		public bool enabledByMap;

		
		public bool isUpdateByTime;

		
		public bool isUpdateByVip;

		
		public bool isUpdateByMapCode;

		
		public int flags;

		
		public double buffValEx;
	}
}
