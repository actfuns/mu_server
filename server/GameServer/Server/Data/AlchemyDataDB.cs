using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AlchemyDataDB
	{
		
		[ProtoMember(1)]
		public AlchemyData BaseData = new AlchemyData();

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public Dictionary<int, int> HistCost = new Dictionary<int, int>();

		
		[ProtoMember(4)]
		public int ElementDayID = 0;

		
		[ProtoMember(5)]
		public string rollbackType = "";
	}
}
