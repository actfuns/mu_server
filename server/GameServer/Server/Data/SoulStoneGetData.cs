using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SoulStoneGetData
	{
		
		[ProtoMember(1)]
		public int Error;

		
		[ProtoMember(2)]
		public int RequestTimes;

		
		[ProtoMember(3)]
		public int RealDoTimes;

		
		[ProtoMember(4)]
		public int NewRandId;

		
		[ProtoMember(5)]
		public List<int> Stones;

		
		[ProtoMember(6)]
		public List<int> ExtGoods;
	}
}
