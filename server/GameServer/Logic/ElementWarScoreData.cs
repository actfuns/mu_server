using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ElementWarScoreData
	{
		
		[ProtoMember(1)]
		public int Wave = 0;

		
		[ProtoMember(2)]
		public long EndTime = 0L;

		
		[ProtoMember(3)]
		public long MonsterCount = 0L;
	}
}
