using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000FA RID: 250
	public class PlayerLogoutFinishEventObject : EventObject
	{
		// Token: 0x060003DE RID: 990 RVA: 0x0003D66C File Offset: 0x0003B86C
		public PlayerLogoutFinishEventObject(GameClient player) : base(55)
		{
			this.player = player;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0003D680 File Offset: 0x0003B880
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x04000534 RID: 1332
		private GameClient player;
	}
}
