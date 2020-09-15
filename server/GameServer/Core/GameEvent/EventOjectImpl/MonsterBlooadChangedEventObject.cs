using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E4 RID: 228
	public class MonsterBlooadChangedEventObject : EventObject
	{
		// Token: 0x060003B1 RID: 945 RVA: 0x0003D214 File Offset: 0x0003B414
		public MonsterBlooadChangedEventObject(Monster monster, GameClient client = null, int injure = 0) : base(18)
		{
			this.monster = monster;
			this.client = client;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0003D230 File Offset: 0x0003B430
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0003D248 File Offset: 0x0003B448
		public GameClient getGameClient()
		{
			return this.client;
		}

		// Token: 0x04000519 RID: 1305
		private Monster monster;

		// Token: 0x0400051A RID: 1306
		private GameClient client;
	}
}
