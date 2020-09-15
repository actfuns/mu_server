using System;
using GameDBServer.DB;

namespace GameDBServer.Core.GameEvent.EventObjectImpl
{
	// Token: 0x0200001F RID: 31
	public class PlayerLoginEventObject : EventObject
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00004AA0 File Offset: 0x00002CA0
		public PlayerLoginEventObject(DBRoleInfo roleInfo) : base(0)
		{
			this.roleInfo = roleInfo;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00004AB4 File Offset: 0x00002CB4
		public DBRoleInfo RoleInfo
		{
			get
			{
				return this.roleInfo;
			}
		}

		// Token: 0x0400005B RID: 91
		private DBRoleInfo roleInfo;
	}
}
