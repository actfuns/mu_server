using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiLingDiItemData
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
		public string WarRequest = "";

		
		[ProtoMember(7)]
		public int AwardFetchDay = -1;
	}
}
