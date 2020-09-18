using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class MonsterBlooadChangedEventObject : EventObject
	{
		
		public MonsterBlooadChangedEventObject(Monster monster, GameClient client = null, int injure = 0) : base(18)
		{
			this.monster = monster;
			this.client = client;
		}

		
		public Monster getMonster()
		{
			return this.monster;
		}

		
		public GameClient getGameClient()
		{
			return this.client;
		}

		
		private Monster monster;

		
		private GameClient client;
	}
}
