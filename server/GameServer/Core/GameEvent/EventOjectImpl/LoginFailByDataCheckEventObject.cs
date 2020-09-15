using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F2 RID: 242
	public class LoginFailByDataCheckEventObject : IpEventBase
	{
		// Token: 0x060003D3 RID: 979 RVA: 0x0003D566 File Offset: 0x0003B766
		public LoginFailByDataCheckEventObject(long ip, string uid) : base(46)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
