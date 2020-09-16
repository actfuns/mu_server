using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiChallengeResultData
	{
		
		[ProtoMember(1)]
		public int playerId;

		
		[ProtoMember(2)]
		public int robotId;

		
		[ProtoMember(3)]
		public bool isWin;
	}
}
