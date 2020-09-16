using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class QiangGouItemData
	{
		
		[ProtoMember(1)]
		public int QiangGouID = 0;

		
		[ProtoMember(2)]
		public int Group = 0;

		
		[ProtoMember(3)]
		public int ItemID = 0;

		
		[ProtoMember(4)]
		public int GoodsID = 0;

		
		[ProtoMember(5)]
		public string StartTime = "";

		
		[ProtoMember(6)]
		public string EndTime = "";

		
		[ProtoMember(7)]
		public int IsTimeOver = 0;

		
		[ProtoMember(8)]
		public int SinglePurchase = 0;

		
		[ProtoMember(9)]
		public int FullPurchase = 0;

		
		[ProtoMember(10)]
		public int FullHasPurchase = 0;

		
		[ProtoMember(11)]
		public int SingleHasPurchase = 0;

		
		[ProtoMember(12)]
		public int CurrentRoleID = 0;

		
		[ProtoMember(13)]
		public int DaysTime = 0;

		
		[ProtoMember(14)]
		public int Price = 0;

		
		[ProtoMember(15)]
		public int Random = 0;

		
		[ProtoMember(16)]
		public int OrigPrice = 0;

		
		[ProtoMember(17)]
		public int Type = 0;
	}
}
