using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E3 RID: 227
	public class MonsterBirthOnEventObject : EventObject
	{
		// Token: 0x060003AF RID: 943 RVA: 0x0003D1E8 File Offset: 0x0003B3E8
		public MonsterBirthOnEventObject(Monster monster) : base(16)
		{
			this.monster = monster;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0003D1FC File Offset: 0x0003B3FC
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x04000518 RID: 1304
		private Monster monster;
	}
}
