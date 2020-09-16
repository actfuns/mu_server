using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TeamData
	{
		
		public TeamMemberData GetLeader()
		{
			TeamMemberData result;
			if (this.TeamRoles == null || this.TeamRoles.Count < 1)
			{
				result = null;
			}
			else
			{
				result = this.TeamRoles.Find((TeamMemberData _x) => _x.RoleID == this.LeaderRoleID);
			}
			return result;
		}

		
		[ProtoMember(1)]
		public int TeamID = 0;

		
		[ProtoMember(2)]
		public int LeaderRoleID = 0;

		
		[ProtoMember(3)]
		public List<TeamMemberData> TeamRoles;

		
		[ProtoMember(4)]
		public long AddDateTime = 0L;

		
		[ProtoMember(5)]
		public int GetThingOpt = 0;

		
		[ProtoMember(6)]
		public int PosX = 0;

		
		[ProtoMember(7)]
		public int PosY = 0;
	}
}
