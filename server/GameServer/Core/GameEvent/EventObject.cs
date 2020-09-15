using System;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000006 RID: 6
	public class EventObject
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00005949 File Offset: 0x00003B49
		public EventObject(int eventType)
		{
			this.eventType = eventType;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00005962 File Offset: 0x00003B62
		public EventObject(int eventType, params object[] args)
		{
			this.eventType = eventType;
			this.Params = args;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00005982 File Offset: 0x00003B82
		public EventObject(int eventType, bool asyncEvent)
		{
			this.eventType = eventType;
			this.AsyncEvent = asyncEvent;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000059A4 File Offset: 0x00003BA4
		public int getEventType()
		{
			return this.eventType;
		}

		// Token: 0x04000033 RID: 51
		protected int eventType = -1;

		// Token: 0x04000034 RID: 52
		public bool AsyncEvent;

		// Token: 0x04000035 RID: 53
		public object[] Params;
	}
}
