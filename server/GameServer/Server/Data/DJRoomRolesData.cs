using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJRoomRolesData
	{
		
		[ProtoMember(1)]
		public int RoomID = 0;

		
		[ProtoMember(2)]
		public List<DJRoomRoleData> Team1;

		
		[ProtoMember(3)]
		public List<DJRoomRoleData> Team2;

		
		[ProtoMember(4)]
		public Dictionary<int, int> TeamStates;

		
		[ProtoMember(5)]
		public int Locked;

		
		[ProtoMember(6)]
		public int Removed;

		
		[ProtoMember(7)]
		public List<DJRoomRoleData> ViewRoles;

		
		[ProtoMember(8)]
		public Dictionary<int, int> RoleStates;
	}
}
