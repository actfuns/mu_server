using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MarryPartyData
	{
		
		[ProtoMember(1)]
		public int RoleID = -1;

		
		[ProtoMember(2)]
		public int PartyType = -1;

		
		[ProtoMember(3)]
		public int JoinCount = 0;

		
		[ProtoMember(4)]
		public long StartTime = -1L;

		
		[ProtoMember(5)]
		public string HusbandName = "";

		
		[ProtoMember(6)]
		public string WifeName = "";

		
		[ProtoMember(7)]
		public int HusbandRoleID = -1;

		
		[ProtoMember(8)]
		public int WifeRoleID = -1;
	}
}
