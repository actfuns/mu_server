using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x02000021 RID: 33
	public class PlayerOnlineEventObject : EventObject
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00004AF8 File Offset: 0x00002CF8
		public PlayerOnlineEventObject(DBRoleInfo player) : base(2)
		{
			this.player = player;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004B0C File Offset: 0x00002D0C
		public DBRoleInfo getPlayer()
		{
			return this.player;
		}

		// Token: 0x0400005D RID: 93
		private DBRoleInfo player;
	}
}
