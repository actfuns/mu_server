using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000E9 RID: 233
	public class MonsterToMonsterDeadEventObject : EventObject
	{
		// Token: 0x060003BF RID: 959 RVA: 0x0003D378 File Offset: 0x0003B578
		public MonsterToMonsterDeadEventObject(Monster monster, Monster monsterAttack) : base(35)
		{
			this.monster = monster;
			this.monsterAttack = monsterAttack;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0003D394 File Offset: 0x0003B594
		public Monster getMonster()
		{
			return this.monster;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0003D3AC File Offset: 0x0003B5AC
		public Monster getMonsterAttack()
		{
			return this.monsterAttack;
		}

		// Token: 0x04000523 RID: 1315
		private Monster monsterAttack;

		// Token: 0x04000524 RID: 1316
		private Monster monster;
	}
}
