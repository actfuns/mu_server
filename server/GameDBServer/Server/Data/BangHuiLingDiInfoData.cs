using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiLingDiInfoData
	{
		
		[ProtoMember(1)]
		public int LingDiID = 0;

		
		[ProtoMember(2)]
		public int BHID = 0;

		
		[ProtoMember(3)]
		public int ZoneID = 0;

		
		[ProtoMember(4)]
		public string BHName = "";

		
		[ProtoMember(5)]
		public int LingDiTax = 0;

		
		[ProtoMember(6)]
		public int TakeDayID = 0;

		
		[ProtoMember(7)]
		public int TakeDayNum = 0;

		
		[ProtoMember(8)]
		public int YestodayTax = 0;

		
		[ProtoMember(9)]
		public int TaxDayID = 0;

		
		[ProtoMember(10)]
		public int TodayTax = 0;

		
		[ProtoMember(11)]
		public int TotalTax = 0;

		
		[ProtoMember(12)]
		public string WarRequest = "";

		
		[ProtoMember(13)]
		public int AwardFetchDay = 0;
	}
}
