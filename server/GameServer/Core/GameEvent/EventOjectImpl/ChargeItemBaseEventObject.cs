using System;
using GameServer.Logic;
using Server.Data;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class ChargeItemBaseEventObject : EventObject
	{
		
		public ChargeItemBaseEventObject(GameClient player, ChargeItemData Config) : base(36)
		{
			this.Player = player;
			this.ChargeItemConfig = Config;
		}

		
		public GameClient Player;

		
		public ChargeItemData ChargeItemConfig;

		
		public int ChargeMoney;
	}
}
