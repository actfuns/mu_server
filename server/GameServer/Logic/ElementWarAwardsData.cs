using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ElementWarAwardsData
	{
		
		[ProtoMember(1)]
		public long Exp;

		
		[ProtoMember(2)]
		public int Money;

		
		[ProtoMember(3)]
		public int Light;

		
		[ProtoMember(4)]
		public int Wave;

		
		[ProtoMember(5)]
		public int ysfm;
	}
}
