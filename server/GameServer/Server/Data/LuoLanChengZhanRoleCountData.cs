using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LuoLanChengZhanRoleCountData
	{
		
		[ProtoMember(1)]
		public int BHID;

		
		[ProtoMember(2)]
		public int RoleCount;
	}
}
