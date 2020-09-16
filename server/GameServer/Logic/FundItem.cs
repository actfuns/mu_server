using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class FundItem
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int FundType = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public int BuyType = 0;

		
		[ProtoMember(3, IsRequired = true)]
		public DateTime BuyTime = DateTime.MinValue;

		
		[ProtoMember(4, IsRequired = true)]
		public int FundID = 0;

		
		[ProtoMember(5, IsRequired = true)]
		public int AwardID = 0;

		
		[ProtoMember(6, IsRequired = true)]
		public int AwardType = 0;

		
		[ProtoMember(7, IsRequired = true)]
		public int Value1 = 0;

		
		[ProtoMember(8, IsRequired = true)]
		public int Value2 = 0;
	}
}
