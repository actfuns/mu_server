using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreBangHuiAddMemberEventObject : EventObjectEx
	{
		
		public PreBangHuiAddMemberEventObject(GameClient player, int bhid) : base(23)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		
		public GameClient Player;

		
		public int BHID;
	}
}
