using System;

namespace GameServer.Logic
{
	
	public class MonsterFlags
	{
		
		public void Copy(MonsterFlags flags)
		{
			if (null != flags)
			{
				this.InjureEvent = flags.InjureEvent;
			}
		}

		
		public static readonly MonsterFlags AllFlags = new MonsterFlags
		{
			InjureEvent = true
		};

		
		public bool InjureEvent;
	}
}
