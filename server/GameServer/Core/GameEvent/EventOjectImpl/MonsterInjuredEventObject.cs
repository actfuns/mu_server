using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E7 RID: 231
	public class MonsterInjuredEventObject : EventObject
	{
		// Token: 0x060003BA RID: 954 RVA: 0x0003D2F8 File Offset: 0x0003B4F8
		public MonsterInjuredEventObject(Monster monster, GameClient attacker, int injure) : base(17)
		{
			this.monster = monster;
			this.attacker = attacker;
			this.injure = injure;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0003D31C File Offset: 0x0003B51C
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0003D334 File Offset: 0x0003B534
		public GameClient getAttacker()
		{
			return this.attacker;
		}

		// Token: 0x0400051F RID: 1311
		private Monster monster;

		// Token: 0x04000520 RID: 1312
		private GameClient attacker;

		// Token: 0x04000521 RID: 1313
		public int injure;
	}
}
