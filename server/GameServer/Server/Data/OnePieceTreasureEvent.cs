using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class OnePieceTreasureEvent
	{
		
		[ProtoMember(1)]
		public int EventID = 0;

		
		[ProtoMember(2)]
		public int EventValue = 0;

		
		[ProtoMember(3)]
		public List<int> BoxIDList = null;

		
		[ProtoMember(4)]
		public int ErrCode = 0;
	}
}
