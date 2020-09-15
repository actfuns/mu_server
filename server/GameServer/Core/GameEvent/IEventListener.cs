using System;

namespace GameServer.Core.GameEvent
{
	// Token: 0x0200003D RID: 61
	public interface IEventListener
	{
		// Token: 0x06000086 RID: 134
		void processEvent(EventObject eventObject);
	}
}
