using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerLogoutEventObject : EventObject
	{
		
		public PlayerLogoutEventObject(GameClient player) : base(12)
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
