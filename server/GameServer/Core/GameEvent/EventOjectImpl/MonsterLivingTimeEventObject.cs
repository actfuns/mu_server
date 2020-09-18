using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterLivingTimeEventObject : EventObject
	{
		
		public MonsterLivingTimeEventObject(Monster monster) : base(20)
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
