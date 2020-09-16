using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ExchangeData
	{
		
		[ProtoMember(1)]
		public int ExchangeID;

		
		[ProtoMember(2)]
		public int RequestRoleID;

		
		[ProtoMember(3)]
		public int AgreeRoleID;

		
		[ProtoMember(4)]
		public Dictionary<int, List<GoodsData>> GoodsDict;

		
		[ProtoMember(5)]
		public Dictionary<int, int> MoneyDict;

		
		[ProtoMember(6)]
		public Dictionary<int, int> LockDict;

		
		[ProtoMember(7)]
		public Dictionary<int, int> DoneDict;

		
		[ProtoMember(8)]
		public long AddDateTime;

		
		[ProtoMember(9)]
		public int Done;

		
		[ProtoMember(10)]
		public Dictionary<int, int> YuanBaoDict;
	}
}
