using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreGotoLastMapEventObject : EventObjectEx
	{
		
		public PreGotoLastMapEventObject(GameClient player, int sceneType) : base(21)
		{
			this.Player = player;
			this.SceneType = sceneType;
		}

		
		public GameClient Player;

		
		public int SceneType;
	}
}
