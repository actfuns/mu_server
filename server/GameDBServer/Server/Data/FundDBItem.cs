using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FundDBItem
	{
		
		[ProtoMember(1)]
		public int ZoneID = 0;

		
		[ProtoMember(2)]
		public string UserID = "";

		
		[ProtoMember(3)]
		public int RoleID = 0;

		
		[ProtoMember(4)]
		public int FundType = 0;

		
		[ProtoMember(5)]
		public int FundID = 0;

		
		[ProtoMember(6)]
		public DateTime BuyTime = DateTime.MinValue;

		
		[ProtoMember(7)]
		public int AwardID = 0;

		
		[ProtoMember(8)]
		public int Value1 = 0;

		
		[ProtoMember(9)]
		public int Value2 = 0;

		
		[ProtoMember(10)]
		public int State = 0;
	}
}
