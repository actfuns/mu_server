using System;

namespace LogDBServer.Core.GameEvent
{
	
	public interface IEventListener
	{
		
		void processEvent(EventObject eventObject);
	}
}
