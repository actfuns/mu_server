using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000FC RID: 252
	public class PlayerOnlineEventObject : EventObject
	{
		// Token: 0x060003E2 RID: 994 RVA: 0x0003D6D9 File Offset: 0x0003B8D9
		public PlayerOnlineEventObject(GameClient player) : base(38)
		{
			this.player = player;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0003D6F0 File Offset: 0x0003B8F0
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x0400053B RID: 1339
		private GameClient player;
	}
}
