using System;

namespace KF.Contract.Data
{
	// Token: 0x0200001C RID: 28
	public class KuaFuRoleKey : IEquatable<KuaFuRoleKey>
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00003EB8 File Offset: 0x000020B8
		public static KuaFuRoleKey Get(int serverId, int roleId)
		{
			return new KuaFuRoleKey(serverId, roleId);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00003ED1 File Offset: 0x000020D1
		private KuaFuRoleKey(int serverId, int roleId)
		{
			this.ServerId = serverId;
			this.RoleId = roleId;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00003EEC File Offset: 0x000020EC
		public bool Equals(KuaFuRoleKey other)
		{
			return this.RoleId == other.RoleId && this.ServerId == other.ServerId;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00003F20 File Offset: 0x00002120
		public override int GetHashCode()
		{
			return this.RoleId;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00003F38 File Offset: 0x00002138
		public override bool Equals(object other)
		{
			KuaFuRoleKey obj = other as KuaFuRoleKey;
			return null != obj && this.RoleId == obj.RoleId && this.ServerId == obj.ServerId;
		}

		// Token: 0x04000094 RID: 148
		private int RoleId;

		// Token: 0x04000095 RID: 149
		private int ServerId;
	}
}
