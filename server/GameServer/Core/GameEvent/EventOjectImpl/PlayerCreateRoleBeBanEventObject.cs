using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerCreateRoleBeBanEventObject : IpEventBase
	{
		
		public PlayerCreateRoleBeBanEventObject(long ip, string uid) : base(43)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
