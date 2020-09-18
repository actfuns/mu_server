using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterToMonsterDeadEventObject : EventObject
	{
		
		public MonsterToMonsterDeadEventObject(Monster monster, Monster monsterAttack) : base(35)
		{
			this.monster = monster;
			this.monsterAttack = monsterAttack;
		}

		
		public Monster getMonster()
		{
			return this.monster;
		}

		
		public Monster getMonsterAttack()
		{
			return this.monsterAttack;
		}

		
		private Monster monsterAttack;

		
		private Monster monster;
	}
}
