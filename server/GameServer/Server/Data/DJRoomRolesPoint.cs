using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJRoomRolesPoint
	{
		
		[ProtoMember(1)]
		public int RoomID = 0;

		
		[ProtoMember(2)]
		public string RoomName = "";

		
		[ProtoMember(3)]
		public List<DJRoomRolePoint> RolePoints;
	}
}
