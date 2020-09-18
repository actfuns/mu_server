using System;
using GameDBServer.DB;

namespace GameDBServer.Core.GameEvent.EventObjectImpl
{
	
	public class PlayerLogoutEventObject : EventObject
	{
		
		public PlayerLogoutEventObject(DBRoleInfo roleInfo) : base(1)
		{
			this.roleInfo = roleInfo;
		}

		
		
		public DBRoleInfo RoleInfo
		{
			get
			{
				return this.roleInfo;
			}
		}

		
		private DBRoleInfo roleInfo;
	}
}
