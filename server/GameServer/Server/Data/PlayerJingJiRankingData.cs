using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiRankingData
	{
		
		[ProtoMember(1)]
		public int roleId;

		
		[ProtoMember(2)]
		public string roleName;

		
		[ProtoMember(3)]
		public int combatForce;

		
		[ProtoMember(4)]
		public int ranking;
	}
}
