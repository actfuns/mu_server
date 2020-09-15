using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000DA RID: 218
	public class OnStartPlayGameEventObject : EventObject
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x0003D0B0 File Offset: 0x0003B2B0
		public OnStartPlayGameEventObject(GameClient client) : base(28)
		{
			this.Client = client;
		}

		// Token: 0x04000506 RID: 1286
		public GameClient Client;
	}
}
