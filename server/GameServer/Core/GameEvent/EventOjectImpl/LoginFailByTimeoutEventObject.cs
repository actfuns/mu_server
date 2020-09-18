using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class LoginFailByTimeoutEventObject : IpEventBase
	{
		
		public LoginFailByTimeoutEventObject(long ip) : base(48)
		{
			this.ipAsInt = ip;
		}
	}
}
