using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E5 RID: 229
	public class MonsterDeadEventObject : EventObject
	{
		// Token: 0x060003B4 RID: 948 RVA: 0x0003D260 File Offset: 0x0003B460
		public MonsterDeadEventObject(Monster monster, GameClient attacker) : base(11)
		{
			this.monster = monster;
			this.attacker = attacker;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0003D27C File Offset: 0x0003B47C
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0003D294 File Offset: 0x0003B494
		public GameClient getAttacker()
		{
			return this.attacker;
		}

		// Token: 0x0400051B RID: 1307
		private Monster monster;

		// Token: 0x0400051C RID: 1308
		private GameClient attacker;
	}
}
