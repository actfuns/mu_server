using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E8 RID: 232
	public class MonsterLivingTimeEventObject : EventObject
	{
		// Token: 0x060003BD RID: 957 RVA: 0x0003D34C File Offset: 0x0003B54C
		public MonsterLivingTimeEventObject(Monster monster) : base(20)
		{
			this.monster = monster;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0003D360 File Offset: 0x0003B560
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x04000522 RID: 1314
		private Monster monster;
	}
}
