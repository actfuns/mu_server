using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E2 RID: 226
	public class MonsterAttackedEventObject : EventObject
	{
		// Token: 0x060003AC RID: 940 RVA: 0x0003D19C File Offset: 0x0003B39C
		public MonsterAttackedEventObject(Monster monster, int enemy) : base(19)
		{
			this.monster = monster;
			this.enemy = enemy;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0003D1B8 File Offset: 0x0003B3B8
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0003D1D0 File Offset: 0x0003B3D0
		public int getEnemy()
		{
			return this.enemy;
		}

		// Token: 0x04000516 RID: 1302
		private Monster monster;

		// Token: 0x04000517 RID: 1303
		private int enemy;
	}
}
