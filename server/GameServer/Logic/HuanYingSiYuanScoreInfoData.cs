using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class HuanYingSiYuanScoreInfoData
	{
		
		[ProtoMember(1)]
		public int Score1;

		
		[ProtoMember(2)]
		public int Score2;

		
		[ProtoMember(3)]
		public long Count1;

		
		[ProtoMember(4)]
		public int Count2;
	}
}
