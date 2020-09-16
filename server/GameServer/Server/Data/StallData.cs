using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class StallData
	{
		
		[ProtoMember(1)]
		public int StallID;

		
		[ProtoMember(2)]
		public int RoleID;

		
		[ProtoMember(3)]
		public string StallName;

		
		[ProtoMember(4)]
		public string StallMessage;

		
		[ProtoMember(5)]
		public List<GoodsData> GoodsList;

		
		[ProtoMember(6)]
		public Dictionary<int, int> GoodsPriceDict;

		
		[ProtoMember(7)]
		public long AddDateTime;

		
		[ProtoMember(8)]
		public int Start;
	}
}
