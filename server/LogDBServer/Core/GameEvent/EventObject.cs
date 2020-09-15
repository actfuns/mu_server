using System;

namespace LogDBServer.Core.GameEvent
{
	// Token: 0x02000009 RID: 9
	public abstract class EventObject
	{
		// Token: 0x06000023 RID: 35 RVA: 0x0000281B File Offset: 0x00000A1B
		protected EventObject(int eventType)
		{
			this.eventType = eventType;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002834 File Offset: 0x00000A34
		public int getEventType()
		{
			return this.eventType;
		}

		// Token: 0x04000014 RID: 20
		protected int eventType = -1;
	}
}
