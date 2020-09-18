using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerLevelupEventObject : EventObject
	{
		
		public PlayerLevelupEventObject(GameClient player) : base(9)
		{
			this.player = player;
		}

		
		
		public GameClient Player
		{
			get
			{
				return this.player;
			}
		}

		
		private GameClient player;
	}
}
