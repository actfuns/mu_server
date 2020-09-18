using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class OnStartPlayGameEventObject : EventObject
	{
		
		public OnStartPlayGameEventObject(GameClient client) : base(28)
		{
			this.Client = client;
		}

		
		public GameClient Client;
	}
}
