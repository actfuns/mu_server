using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F5 RID: 245
	public class PlayerCreateRoleBeBanEventObject : IpEventBase
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x0003D5B0 File Offset: 0x0003B7B0
		public PlayerCreateRoleBeBanEventObject(long ip, string uid) : base(43)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
