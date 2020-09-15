using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F6 RID: 246
	public class PlayerCreateRoleLimitEventObject : IpEventBase
	{
		// Token: 0x060003D7 RID: 983 RVA: 0x0003D5CB File Offset: 0x0003B7CB
		public PlayerCreateRoleLimitEventObject(long ip, string uid) : base(44)
		{
			this.ipAsInt = ip;
			this.userid = uid;
		}
	}
}
