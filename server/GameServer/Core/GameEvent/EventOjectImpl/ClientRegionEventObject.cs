using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class ClientRegionEventObject : EventObject
	{
		
		
		
		public GameClient Client { get; private set; }

		
		
		
		public int EventType { get; private set; }

		
		
		
		public int Flag { get; private set; }

		
		
		
		public int AreaLuaID { get; private set; }

		
		public ClientRegionEventObject(GameClient client, int eventType, int flag, int areaLuaID) : base(31)
		{
			this.Client = client;
			this.EventType = eventType;
			this.Flag = flag;
			this.AreaLuaID = areaLuaID;
		}
	}
}
