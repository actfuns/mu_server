using System;

namespace GameServer.Core.GameEvent
{
	
	public class EventObject
	{
		
		public EventObject(int eventType)
		{
			this.eventType = eventType;
		}

		
		public EventObject(int eventType, params object[] args)
		{
			this.eventType = eventType;
			this.Params = args;
		}

		
		public EventObject(int eventType, bool asyncEvent)
		{
			this.eventType = eventType;
			this.AsyncEvent = asyncEvent;
		}

		
		public int getEventType()
		{
			return this.eventType;
		}

		
		protected int eventType = -1;

		
		public bool AsyncEvent;

		
		public object[] Params;
	}
}
