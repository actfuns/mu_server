using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerLogoutFinishEventObject : EventObject
	{
		
		public PlayerLogoutFinishEventObject(GameClient player) : base(55)
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
