using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000EC RID: 236
	public class PlayerDeadEventObject : EventObject
	{
		// Token: 0x060003C3 RID: 963 RVA: 0x0003D3D0 File Offset: 0x0003B5D0
		public PlayerDeadEventObject(GameClient player, Monster attacker) : base(10)
		{
			this.player = player;
			this.attacker = attacker;
			this.Type = PlayerDeadEventTypes.ByMonster;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0003D3F2 File Offset: 0x0003B5F2
		public PlayerDeadEventObject(GameClient player, GameClient attacker) : base(10)
		{
			this.player = player;
			this.attackerRole = attacker;
			this.Type = PlayerDeadEventTypes.ByRole;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0003D414 File Offset: 0x0003B614
		public Monster getAttacker()
		{
			return this.attacker;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0003D42C File Offset: 0x0003B62C
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0003D444 File Offset: 0x0003B644
		public GameClient getAttackerRole()
		{
			return this.attackerRole;
		}

		// Token: 0x04000528 RID: 1320
		private GameClient attackerRole;

		// Token: 0x04000529 RID: 1321
		private Monster attacker;

		// Token: 0x0400052A RID: 1322
		private GameClient player;

		// Token: 0x0400052B RID: 1323
		public PlayerDeadEventTypes Type;
	}
}
