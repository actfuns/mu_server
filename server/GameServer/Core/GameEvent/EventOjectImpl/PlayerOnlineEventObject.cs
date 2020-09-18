using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerOnlineEventObject : EventObject
	{
		
		public PlayerOnlineEventObject(GameClient player) : base(38)
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
