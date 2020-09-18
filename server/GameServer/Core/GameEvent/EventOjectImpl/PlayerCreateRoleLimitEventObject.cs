using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerCreateRoleLimitEventObject : IpEventBase
	{
		
		public PlayerCreateRoleLimitEventObject(long ip, string uid) : base(44)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
