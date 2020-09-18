using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PostBangHuiChangeEventObject : EventObjectEx
	{
		
		public PostBangHuiChangeEventObject(GameClient player, int bhid) : base(26)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		
		public GameClient Player;

		
		public int BHID;
	}
}
