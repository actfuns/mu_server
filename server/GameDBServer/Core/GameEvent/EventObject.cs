using System;

namespace GameDBServer.Core.GameEvent
{
	// Token: 0x0200001D RID: 29
	public abstract class EventObject
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00004A60 File Offset: 0x00002C60
		protected EventObject(int eventType)
		{
			this.eventType = eventType;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004A7C File Offset: 0x00002C7C
		public int getEventType()
		{
			return this.eventType;
		}

		// Token: 0x0400005A RID: 90
		protected int eventType = -1;
	}
}
