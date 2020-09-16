using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiChallengeInfoData
	{
		
		[ProtoMember(1)]
		public int pkId;

		
		[ProtoMember(2)]
		public int roleId;

		
		[ProtoMember(3)]
		public int zhanbaoType;

		
		[ProtoMember(4)]
		public string challengeName;

		
		[ProtoMember(5)]
		public int value;
	}
}
