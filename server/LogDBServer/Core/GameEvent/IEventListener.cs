using System;

namespace LogDBServer.Core.GameEvent
{
	// Token: 0x0200000D RID: 13
	public interface IEventListener
	{
		// Token: 0x0600002D RID: 45
		void processEvent(EventObject eventObject);
	}
}
