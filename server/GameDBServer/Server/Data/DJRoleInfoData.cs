using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJRoleInfoData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int Level = 0;

		
		[ProtoMember(4)]
		public int Occupation = 0;

		
		[ProtoMember(5)]
		public int OnlineState = 0;
	}
}
