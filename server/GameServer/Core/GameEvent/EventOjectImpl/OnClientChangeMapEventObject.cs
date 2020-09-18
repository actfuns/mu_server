using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class OnClientChangeMapEventObject : EventObjectEx
	{
		
		public OnClientChangeMapEventObject(GameClient client, int teleportID, int toMapCode, int toPosX, int toPosY) : base(29)
		{
			this.Client = client;
			this.TeleportID = teleportID;
			this.ToMapCode = toMapCode;
			this.ToPosX = toPosX;
			this.ToPosY = toPosY;
		}

		
		public GameClient Client;

		
		public int TeleportID;

		
		public int ToMapCode;

		
		public int ToPosX;

		
		public int ToPosY;
	}
}
