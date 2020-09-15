using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F3 RID: 243
	public class LoginFailByUserBanEventObject : IpEventBase
	{
		// Token: 0x060003D4 RID: 980 RVA: 0x0003D581 File Offset: 0x0003B781
		public LoginFailByUserBanEventObject(long ip, string uid) : base(47)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
