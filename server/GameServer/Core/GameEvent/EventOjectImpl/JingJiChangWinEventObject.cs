using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class JingJiChangWinEventObject : EventObject
	{
		
		public JingJiChangWinEventObject(GameClient player, Robot robot, int type) : base(53)
		{
			this.player = player;
			this.robot = robot;
			this.type = type;
		}

		
		public int getType()
		{
			return this.type;
		}

		
		public GameClient getPlayer()
		{
			return this.player;
		}

		
		public Robot getRobot()
		{
			return this.robot;
		}

		
		private GameClient player;

		
		private Robot robot;

		
		private int type;
	}
}
