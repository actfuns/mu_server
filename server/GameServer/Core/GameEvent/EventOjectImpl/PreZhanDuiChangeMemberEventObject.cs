using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreZhanDuiChangeMemberEventObject : EventObjectEx
	{
		
		public PreZhanDuiChangeMemberEventObject(GameClient player, int bhid, int _operator) : base(63)
		{
			this.Player = player;
			this.ZhanDuiID = bhid;
			this.Operator = _operator;
			this.Result = true;
		}

		
		public GameClient Player;

		
		public int ZhanDuiID;

		
		public int Operator;
	}
}
