using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F8 RID: 248
	public class PlayerLevelupEventObject : EventObject
	{
		// Token: 0x060003DA RID: 986 RVA: 0x0003D614 File Offset: 0x0003B814
		public PlayerLevelupEventObject(GameClient player) : base(9)
		{
			this.player = player;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060003DB RID: 987 RVA: 0x0003D628 File Offset: 0x0003B828
		public GameClient Player
		{
			get
			{
				return this.player;
			}
		}

		// Token: 0x04000532 RID: 1330
		private GameClient player;
	}
}
