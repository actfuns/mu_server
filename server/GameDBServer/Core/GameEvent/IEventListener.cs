using System;

namespace GameDBServer.Core.GameEvent
{
	// Token: 0x02000007 RID: 7
	public interface IEventListener
	{
		// Token: 0x06000012 RID: 18
		void processEvent(EventObject eventObject);
	}
}
