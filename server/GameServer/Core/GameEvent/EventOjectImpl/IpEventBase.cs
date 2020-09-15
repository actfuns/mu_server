using System;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000EE RID: 238
	public class IpEventBase : EventObject
	{
		// Token: 0x060003CA RID: 970 RVA: 0x0003D488 File Offset: 0x0003B688
		public IpEventBase(int eventType) : base(eventType, true)
		{
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0003D495 File Offset: 0x0003B695
		public IpEventBase(int eventType, long _ipAsInt, string _userid) : base(eventType)
		{
			this.ipAsInt = _ipAsInt;
			this.userid = _userid;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0003D4B0 File Offset: 0x0003B6B0
		public long getIpAsInt()
		{
			return this.ipAsInt;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0003D4C8 File Offset: 0x0003B6C8
		public string getUserID()
		{
			return this.userid;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0003D4E0 File Offset: 0x0003B6E0
		public int getRoleID()
		{
			return this.roleid;
		}

		// Token: 0x0400052D RID: 1325
		protected long ipAsInt;

		// Token: 0x0400052E RID: 1326
		protected string userid;

		// Token: 0x0400052F RID: 1327
		protected int roleid;
	}
}
