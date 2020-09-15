using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F7 RID: 247
	public class PlayerLeaveFuBenEventObject : EventObject
	{
		// Token: 0x060003D8 RID: 984 RVA: 0x0003D5E6 File Offset: 0x0003B7E6
		public PlayerLeaveFuBenEventObject(GameClient player) : base(13)
		{
			this.player = player;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0003D5FC File Offset: 0x0003B7FC
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x04000531 RID: 1329
		private GameClient player;
	}
}
