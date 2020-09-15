using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F4 RID: 244
	public class LoginFailByTimeoutEventObject : IpEventBase
	{
		// Token: 0x060003D5 RID: 981 RVA: 0x0003D59C File Offset: 0x0003B79C
		public LoginFailByTimeoutEventObject(long ip) : base(48)
		{
			this.ipAsInt = ip;
		}
	}
}
