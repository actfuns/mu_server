using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiMiniData
	{
		
		[ProtoMember(1)]
		public int roleId;

		
		[ProtoMember(2)]
		public string roleName;

		
		[ProtoMember(3)]
		public int occupationId;

		
		[ProtoMember(4)]
		public int combatForce;

		
		[ProtoMember(5)]
		public int ranking;

		
		[ProtoMember(6)]
		public int sex;
	}
}
