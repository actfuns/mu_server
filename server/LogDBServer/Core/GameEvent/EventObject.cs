using System;

namespace LogDBServer.Core.GameEvent
{
	
	public abstract class EventObject
	{
		
		protected EventObject(int eventType)
		{
			this.eventType = eventType;
		}

		
		public int getEventType()
		{
			return this.eventType;
		}

		
		protected int eventType = -1;
	}
}
