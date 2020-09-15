using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000108 RID: 264
	public class MainTaskProgressEvent : EventObject
	{
		// Token: 0x06000405 RID: 1029 RVA: 0x0003DF9C File Offset: 0x0003C19C
		public MainTaskProgressEvent(GameClient client, int taskId) : base(58)
		{
			this.Client = client;
			this.MainTaskID = taskId;
		}

		// Token: 0x04000594 RID: 1428
		public int MainTaskID;

		// Token: 0x04000595 RID: 1429
		public GameClient Client;
	}
}
