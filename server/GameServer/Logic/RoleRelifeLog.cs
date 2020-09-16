using System;

namespace GameServer.Logic
{
	
	public class RoleRelifeLog
	{
		
		public RoleRelifeLog(int roleId, string roleName, int mapcode, string reason)
		{
			this.RoleId = roleId;
			this.Rolename = roleName;
			this.MapCode = mapcode;
			this.Reason = reason;
		}

		
		public int RoleId;

		
		public string Rolename;

		
		public int MapCode;

		
		public string Reason;

		
		public bool hpModify;

		
		public int oldHp;

		
		public int newHp;

		
		public bool mpModify;

		
		public int oldMp;

		
		public int newMp;
	}
}
