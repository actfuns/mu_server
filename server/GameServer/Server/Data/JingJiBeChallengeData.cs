using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiBeChallengeData
	{
		
		[ProtoMember(1)]
		public int state;

		
		[ProtoMember(2)]
		public PlayerJingJiData beChallengerData = null;
	}
}
