using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000DB RID: 219
	public class OnClientMapChangedEventObject : EventObject
	{
		// Token: 0x060003A4 RID: 932 RVA: 0x0003D0C4 File Offset: 0x0003B2C4
		public OnClientMapChangedEventObject(GameClient client, int lastMapCode, int currentMapCode) : base(59)
		{
			this.Client = client;
			this.LastMapCode = lastMapCode;
			this.CurrentMapCode = currentMapCode;
		}

		// Token: 0x04000507 RID: 1287
		public GameClient Client;

		// Token: 0x04000508 RID: 1288
		public int LastMapCode;

		// Token: 0x04000509 RID: 1289
		public int CurrentMapCode;
	}
}
