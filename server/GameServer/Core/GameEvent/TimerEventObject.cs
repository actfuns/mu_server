using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent
{
	
	public class TimerEventObject : EventObject
	{
		
		public TimerEventObject() : base(57)
		{
		}

		
		public long LastRunTicks;

		
		public long DeltaTicks;

		
		public long NowTicks;

		
		public GameClient Client;
	}
}
