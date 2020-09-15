using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F9 RID: 249
	public class PlayerLogoutEventObject : EventObject
	{
		// Token: 0x060003DC RID: 988 RVA: 0x0003D640 File Offset: 0x0003B840
		public PlayerLogoutEventObject(GameClient player) : base(12)
		{
			this.player = player;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0003D654 File Offset: 0x0003B854
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x04000533 RID: 1331
		private GameClient player;
	}
}
