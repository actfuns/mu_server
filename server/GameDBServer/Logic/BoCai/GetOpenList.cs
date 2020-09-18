using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	
	[ProtoContract]
	public class GetOpenList
	{
		
		[ProtoMember(1)]
		public List<OpenLottery> ItemList;

		
		[ProtoMember(2)]
		public bool Flag;

		
		[ProtoMember(3)]
		public long MaxDataPeriods;
	}
}
