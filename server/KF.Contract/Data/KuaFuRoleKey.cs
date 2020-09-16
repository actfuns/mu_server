using System;

namespace KF.Contract.Data
{
	
	public class KuaFuRoleKey : IEquatable<KuaFuRoleKey>
	{
		
		public static KuaFuRoleKey Get(int serverId, int roleId)
		{
			return new KuaFuRoleKey(serverId, roleId);
		}

		
		private KuaFuRoleKey(int serverId, int roleId)
		{
			this.ServerId = serverId;
			this.RoleId = roleId;
		}

		
		public bool Equals(KuaFuRoleKey other)
		{
			return this.RoleId == other.RoleId && this.ServerId == other.ServerId;
		}

		
		public override int GetHashCode()
		{
			return this.RoleId;
		}

		
		public override bool Equals(object other)
		{
			KuaFuRoleKey obj = other as KuaFuRoleKey;
			return null != obj && this.RoleId == obj.RoleId && this.ServerId == obj.ServerId;
		}

		
		private int RoleId;

		
		private int ServerId;
	}
}
