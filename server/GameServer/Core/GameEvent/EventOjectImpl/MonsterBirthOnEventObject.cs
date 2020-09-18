using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterBirthOnEventObject : EventObject
	{
		
		public MonsterBirthOnEventObject(Monster monster) : base(16)
		{
			this.monster = monster;
		}

		
		public Monster getMonster()
		{
			return this.monster;
		}

		
		private Monster monster;
	}
}
