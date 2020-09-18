using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreInstallJunQiEventObject : EventObjectEx
	{
		
		public PreInstallJunQiEventObject(GameClient player, int npcID, int sceneType) : base(22)
		{
			this.Player = player;
			this.NPCID = npcID;
			this.SceneType = sceneType;
		}

		
		public GameClient Player;

		
		public int NPCID;

		
		public int SceneType;
	}
}
