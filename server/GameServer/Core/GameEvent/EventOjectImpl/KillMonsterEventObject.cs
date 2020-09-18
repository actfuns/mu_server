using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class KillMonsterEventObject : EventObject
	{
		
		public KillMonsterEventObject(Monster monster, GameClient attacker) : base(56)
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
