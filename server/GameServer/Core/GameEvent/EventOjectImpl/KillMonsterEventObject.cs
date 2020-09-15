using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E6 RID: 230
	public class KillMonsterEventObject : EventObject
	{
		// Token: 0x060003B7 RID: 951 RVA: 0x0003D2AC File Offset: 0x0003B4AC
		public KillMonsterEventObject(Monster monster, GameClient attacker) : base(56)
		{
			this.monster = monster;
			this.attacker = attacker;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0003D2C8 File Offset: 0x0003B4C8
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0003D2E0 File Offset: 0x0003B4E0
		public GameClient getAttacker()
		{
			return this.attacker;
		}

		// Token: 0x0400051D RID: 1309
		private Monster monster;

		// Token: 0x0400051E RID: 1310
		private GameClient attacker;
	}
}
