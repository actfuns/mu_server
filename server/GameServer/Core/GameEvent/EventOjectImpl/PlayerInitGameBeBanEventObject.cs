using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000F0 RID: 240
	public class PlayerInitGameBeBanEventObject : IpEventBase
	{
		// Token: 0x060003D1 RID: 977 RVA: 0x0003D524 File Offset: 0x0003B724
		public PlayerInitGameBeBanEventObject(long ip, string uid, int rid) : base(42)
		{
			this.ipAsInt = ip;
			this.userid = uid;
			this.roleid = this.roleid;
		}
	}
}
