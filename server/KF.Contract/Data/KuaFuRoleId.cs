using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	// Token: 0x0200000F RID: 15
	public struct KuaFuRoleId : IEqualityComparer<KuaFuRoleId>
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002134 File Offset: 0x00000334
		public bool Equals(KuaFuRoleId x, KuaFuRoleId y)
		{
			return x.RoleId == y.RoleId && x.ServerId == y.ServerId;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002170 File Offset: 0x00000370
		public int GetHashCode(KuaFuRoleId obj)
		{
			return obj.RoleId;
		}

		// Token: 0x0400003A RID: 58
		public int ServerId;

		// Token: 0x0400003B RID: 59
		public int RoleId;
	}
}
