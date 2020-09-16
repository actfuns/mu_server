using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	
	public struct KuaFuRoleId : IEqualityComparer<KuaFuRoleId>
	{
		
		public bool Equals(KuaFuRoleId x, KuaFuRoleId y)
		{
			return x.RoleId == y.RoleId && x.ServerId == y.ServerId;
		}

		
		public int GetHashCode(KuaFuRoleId obj)
		{
			return obj.RoleId;
		}

		
		public int ServerId;

		
		public int RoleId;
	}
}
