using System;

namespace GameServer.Logic
{
	// Token: 0x02000374 RID: 884
	public class RoleRelifeLog
	{
		// Token: 0x06000F31 RID: 3889 RVA: 0x000EF433 File Offset: 0x000ED633
		public RoleRelifeLog(int roleId, string roleName, int mapcode, string reason)
		{
			this.RoleId = roleId;
			this.Rolename = roleName;
			this.MapCode = mapcode;
			this.Reason = reason;
		}

		// Token: 0x0400175C RID: 5980
		public int RoleId;

		// Token: 0x0400175D RID: 5981
		public string Rolename;

		// Token: 0x0400175E RID: 5982
		public int MapCode;

		// Token: 0x0400175F RID: 5983
		public string Reason;

		// Token: 0x04001760 RID: 5984
		public bool hpModify;

		// Token: 0x04001761 RID: 5985
		public int oldHp;

		// Token: 0x04001762 RID: 5986
		public int newHp;

		// Token: 0x04001763 RID: 5987
		public bool mpModify;

		// Token: 0x04001764 RID: 5988
		public int oldMp;

		// Token: 0x04001765 RID: 5989
		public int newMp;
	}
}
