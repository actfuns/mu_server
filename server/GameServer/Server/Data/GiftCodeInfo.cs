using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GiftCodeInfo
	{
		
		[ProtoMember(1)]
		public string GiftCodeTypeID = "";

		
		[ProtoMember(2)]
		public string GiftCodeName = "";

		
		[ProtoMember(3)]
		public List<string> ChannelList = new List<string>();

		
		[ProtoMember(4)]
		public List<int> PlatformList = new List<int>();

		
		[ProtoMember(5)]
		public DateTime TimeBegin = DateTime.MinValue;

		
		[ProtoMember(6)]
		public DateTime TimeEnd = DateTime.MinValue;

		
		[ProtoMember(7)]
		public List<int> ZoneList = new List<int>();

		
		[ProtoMember(8)]
		public GiftCodeUserType UserType = GiftCodeUserType.All;

		
		[ProtoMember(9)]
		public int UseCount = 0;

		
		[ProtoMember(10)]
		public List<GoodsData> GoodsList = new List<GoodsData>();

		
		[ProtoMember(11)]
		public List<GoodsData> ProGoodsList = new List<GoodsData>();

		
		[ProtoMember(12)]
		public string Description = "";
	}
}
