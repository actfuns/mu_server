using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerInitGameEventObject : EventObject
	{
		
		public PlayerInitGameEventObject(GameClient player) : base(14)
		{
			this.player = player;
		}

		
		public GameClient getPlayer()
		{
			return this.player;
		}

		
		private GameClient player;
	}
}
