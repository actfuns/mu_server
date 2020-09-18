using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PlayerInitGameBeBanEventObject : IpEventBase
	{
		
		public PlayerInitGameBeBanEventObject(long ip, string uid, int rid) : base(42)
		{
			this.ipAsInt = ip;
			this.userid = uid;
			this.roleid = this.roleid;
		}
	}
}
