using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HeFuPKKingData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int ZoneID = 0;

		
		[ProtoMember(4)]
		public int State = 0;
	}
}
