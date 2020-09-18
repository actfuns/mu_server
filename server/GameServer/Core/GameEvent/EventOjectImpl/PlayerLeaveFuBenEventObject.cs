using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerLeaveFuBenEventObject : EventObject
	{
		
		public PlayerLeaveFuBenEventObject(GameClient player) : base(13)
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
