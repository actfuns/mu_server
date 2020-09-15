using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000EF RID: 239
	public class PlayerInitGameAsyncEventObject : IpEventBase
	{
		// Token: 0x060003CF RID: 975 RVA: 0x0003D4F8 File Offset: 0x0003B6F8
		public PlayerInitGameAsyncEventObject(GameClient player) : base(15)
		{
			this.player = player;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0003D50C File Offset: 0x0003B70C
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x04000530 RID: 1328
		private GameClient player;
	}
}
