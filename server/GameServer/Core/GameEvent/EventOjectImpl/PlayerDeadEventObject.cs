using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerDeadEventObject : EventObject
	{
		
		public PlayerDeadEventObject(GameClient player, Monster attacker) : base(10)
		{
			this.player = player;
			this.attacker = attacker;
			this.Type = PlayerDeadEventTypes.ByMonster;
		}

		
		public PlayerDeadEventObject(GameClient player, GameClient attacker) : base(10)
		{
			this.player = player;
			this.attackerRole = attacker;
			this.Type = PlayerDeadEventTypes.ByRole;
		}

		
		public Monster getAttacker()
		{
			return this.attacker;
		}

		
		public GameClient getPlayer()
		{
			return this.player;
		}

		
		public GameClient getAttackerRole()
		{
			return this.attackerRole;
		}

		
		private GameClient attackerRole;

		
		private Monster attacker;

		
		private GameClient player;

		
		public PlayerDeadEventTypes Type;
	}
}
