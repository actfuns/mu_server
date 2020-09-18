using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterAttackedEventObject : EventObject
	{
		
		public MonsterAttackedEventObject(Monster monster, int enemy) : base(19)
		{
			this.monster = monster;
			this.enemy = enemy;
		}

		
		public Monster getMonster()
		{
			return this.monster;
		}

		
		public int getEnemy()
		{
			return this.enemy;
		}

		
		private Monster monster;

		
		private int enemy;
	}
}
