using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SevenDayActQueryData
	{
		
		[ProtoMember(1)]
		public int ActivityType;

		
		[ProtoMember(2)]
		public Dictionary<int, SevenDayItemData> ItemDict;
	}
}
