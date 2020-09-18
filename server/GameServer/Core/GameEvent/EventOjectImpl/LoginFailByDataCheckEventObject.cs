using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class LoginFailByDataCheckEventObject : IpEventBase
	{
		
		public LoginFailByDataCheckEventObject(long ip, string uid) : base(46)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
