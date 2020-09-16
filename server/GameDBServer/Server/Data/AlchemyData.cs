using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AlchemyData
	{
		
		[ProtoMember(1)]
		public int Element = 0;

		
		[ProtoMember(2)]
		public Dictionary<int, int> ToDayCost = new Dictionary<int, int>();

		
		[ProtoMember(3)]
		public Dictionary<int, int> AlchemyValue = new Dictionary<int, int>();
	}
}
