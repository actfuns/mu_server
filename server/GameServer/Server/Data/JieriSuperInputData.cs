using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriSuperInputData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public int MutiNum;

		
		[ProtoMember(3)]
		public int PurchaseNum;

		
		[ProtoMember(4)]
		public int FullPurchaseNum;

		
		[ProtoMember(5)]
		public DateTime BeginTime;

		
		[ProtoMember(6)]
		public DateTime EndTime;
	}
}
