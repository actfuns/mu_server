using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x0200000A RID: 10
	public class PlayerLoginGameEventObject : EventObject
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00005B46 File Offset: 0x00003D46
		public PlayerLoginGameEventObject(GameClient player) : base(41)
		{
			this.player = player;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00005B5C File Offset: 0x00003D5C
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x04000042 RID: 66
		private GameClient player;
	}
}
