using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerOnlineEventObject : EventObject
	{
		
		public PlayerOnlineEventObject(DBRoleInfo player) : base(2)
		{
			this.player = player;
		}

		
		public DBRoleInfo getPlayer()
		{
			return this.player;
		}

		
		private DBRoleInfo player;
	}
}
