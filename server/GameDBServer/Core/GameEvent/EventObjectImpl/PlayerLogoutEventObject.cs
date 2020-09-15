using System;
using GameDBServer.DB;

namespace GameDBServer.Core.GameEvent.EventObjectImpl
{
	// Token: 0x02000020 RID: 32
	public class PlayerLogoutEventObject : EventObject
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00004ACC File Offset: 0x00002CCC
		public PlayerLogoutEventObject(DBRoleInfo roleInfo) : base(1)
		{
			this.roleInfo = roleInfo;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00004AE0 File Offset: 0x00002CE0
		public DBRoleInfo RoleInfo
		{
			get
			{
				return this.roleInfo;
			}
		}

		// Token: 0x0400005C RID: 92
		private DBRoleInfo roleInfo;
	}
}
