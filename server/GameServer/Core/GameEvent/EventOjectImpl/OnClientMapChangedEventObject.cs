using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class OnClientMapChangedEventObject : EventObject
	{
		
		public OnClientMapChangedEventObject(GameClient client, int lastMapCode, int currentMapCode) : base(59)
		{
			this.Client = client;
			this.LastMapCode = lastMapCode;
			this.CurrentMapCode = currentMapCode;
		}

		
		public GameClient Client;

		
		public int LastMapCode;

		
		public int CurrentMapCode;
	}
}
