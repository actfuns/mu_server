using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterInjuredEventObject : EventObject
	{
		
		public MonsterInjuredEventObject(Monster monster, GameClient attacker, int injure) : base(17)
		{
			this.monster = monster;
			this.attacker = attacker;
			this.injure = injure;
		}

		
		public Monster getMonster()
		{
			return this.monster;
		}

		
		public GameClient getAttacker()
		{
			return this.attacker;
		}

		
		private Monster monster;

		
		private GameClient attacker;

		
		public int injure;
	}
}
