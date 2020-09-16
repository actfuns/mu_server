using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EachRoleChangeName
	{
		
		[ProtoMember(1)]
		public int RoleId = 0;

		
		[ProtoMember(2)]
		public int LeftFreeTimes = 0;

		
		[ProtoMember(3)]
		public int AlreadyZuanShiTimes = 0;
	}
}
