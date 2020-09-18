using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreBangHuiChangeZhiWuEventObject : EventObjectEx
	{
		
		public PreBangHuiChangeZhiWuEventObject(GameClient player, int bhid, int targetRoleId, int targetZhiWu) : base(25)
		{
			this.Player = player;
			this.BHID = bhid;
			this.TargetRoleId = targetRoleId;
			this.TargetZhiWu = targetZhiWu;
		}

		
		public GameClient Player;

		
		public int BHID;

		
		public int TargetRoleId;

		
		public int TargetZhiWu;

		
		public int ErrorCode;
	}
}
