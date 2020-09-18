using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerInitGameAsyncEventObject : IpEventBase
	{
		
		public PlayerInitGameAsyncEventObject(GameClient player) : base(15)
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
