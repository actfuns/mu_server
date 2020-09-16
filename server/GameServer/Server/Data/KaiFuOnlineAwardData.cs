using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KaiFuOnlineAwardData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int ZoneID = 0;

		
		[ProtoMember(3)]
		public string RoleName = "";

		
		[ProtoMember(4)]
		public int DayID = 0;

		
		[ProtoMember(5)]
		public int TotalRoleNum = 0;
	}
}
