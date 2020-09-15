using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F1 RID: 241
	public class LoginSuccessEventObject : IpEventBase
	{
		// Token: 0x060003D2 RID: 978 RVA: 0x0003D54B File Offset: 0x0003B74B
		public LoginSuccessEventObject(long ip, string uid) : base(45)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
