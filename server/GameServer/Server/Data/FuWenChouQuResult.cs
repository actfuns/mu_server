using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuWenChouQuResult
	{
		
		[ProtoMember(1)]
		public int Result;

		
		[ProtoMember(2)]
		public string GoodsList;

		
		[ProtoMember(3)]
		public DateTime FreeTime;

		
		[ProtoMember(4)]
		public int ChouQuCount;
	}
}
