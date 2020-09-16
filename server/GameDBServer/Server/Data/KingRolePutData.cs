using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KingRolePutData
	{
		
		[ProtoMember(1)]
		public int KingType;

		
		[ProtoMember(2)]
		public RoleDataEx RoleDataEx;
	}
}
