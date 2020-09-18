using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterDeadEventObject : EventObject
	{
		
		public MonsterDeadEventObject(Monster monster, GameClient attacker) : base(11)
		{
			this.monster = monster;
			this.attacker = attacker;
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
	}
}
