using System;
using GameDBServer.DB;

namespace GameDBServer.Core.GameEvent.EventObjectImpl
{
	
	public class PlayerLoginEventObject : EventObject
	{
		
		public PlayerLoginEventObject(DBRoleInfo roleInfo) : base(0)
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
