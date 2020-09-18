using System;

namespace GameDBServer.Core.GameEvent
{
	
	public interface IEventListener
	{
		
		void processEvent(EventObject eventObject);
	}
}
