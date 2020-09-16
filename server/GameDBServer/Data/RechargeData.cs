using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	
	[ProtoContract]
	public class RechargeData
	{
		
		[ProtoMember(1)]
		public string UserID = "";

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public int Amount = 0;

		
		[ProtoMember(4)]
		public int ItemId = 0;

		
		[ProtoMember(5)]
		public int BudanFlag = 0;

		
		[ProtoMember(6)]
		public string ChargeTime = "";

		
		[ProtoMember(7)]
		public string order_no = "";

		
		[ProtoMember(8)]
		public string cc = "";

		
		[ProtoMember(9)]
		public string cporder_no = "";

		
		[ProtoMember(10)]
		public string Sign = "";

		
		[ProtoMember(11)]
		public long Time = 0L;

		
		[ProtoMember(12)]
		public int ZoneID = 0;

		
		[ProtoMember(13)]
		public int Id = 0;
	}
}
