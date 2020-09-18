using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreBangHuiRemoveMemberEventObject : EventObjectEx
	{
		
		public PreBangHuiRemoveMemberEventObject(GameClient player, int bhid) : base(24)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		
		public GameClient Player;

		
		public int BHID;
	}
}
