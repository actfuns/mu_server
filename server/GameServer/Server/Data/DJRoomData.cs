using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJRoomData
	{
		
		[ProtoMember(1)]
		public int RoomID = 0;

		
		[ProtoMember(2)]
		public int CreateRoleID = 0;

		
		[ProtoMember(3)]
		public string CreateRoleName = "";

		
		[ProtoMember(4)]
		public string RoomName = "";

		
		[ProtoMember(5)]
		public int VSMode = 0;

		
		[ProtoMember(6)]
		public int PKState = 0;

		
		[ProtoMember(7)]
		public int PKRoleNum = 0;

		
		[ProtoMember(8)]
		public int ViewRoleNum = 0;

		
		[ProtoMember(9)]
		public long StartFightTicks = 0L;

		
		[ProtoMember(10)]
		public int DJFightState = 0;
	}
}
