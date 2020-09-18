using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class LoginFailByUserBanEventObject : IpEventBase
	{
		
		public LoginFailByUserBanEventObject(long ip, string uid) : base(47)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
