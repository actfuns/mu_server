using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000ED RID: 237
	public class PlayerInitGameEventObject : EventObject
	{
		// Token: 0x060003C8 RID: 968 RVA: 0x0003D45C File Offset: 0x0003B65C
		public PlayerInitGameEventObject(GameClient player) : base(14)
		{
			this.player = player;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0003D470 File Offset: 0x0003B670
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x0400052C RID: 1324
		private GameClient player;
	}
}
