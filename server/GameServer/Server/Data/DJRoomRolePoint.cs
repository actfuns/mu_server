using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJRoomRolePoint
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int FightPoint = 0;
	}
}
