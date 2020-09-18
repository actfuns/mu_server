using System;
using ProtoBuf;

namespace GameDBServer.DB
{
	
	[ProtoContract]
	public class RoleMiniInfo
	{
		
		[ProtoMember(1)]
		public long roleId;

		
		[ProtoMember(2)]
		public int zoneId;

		
		[ProtoMember(3)]
		public string userId;
	}
}
