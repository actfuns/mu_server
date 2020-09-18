using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class LoginSuccessEventObject : IpEventBase
	{
		
		public LoginSuccessEventObject(long ip, string uid) : base(45)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
