using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AddGoodsData
	{
		
		[ProtoMember(1)]
		public int roleID = 0;

		
		[ProtoMember(2)]
		public int id = 0;

		
		[ProtoMember(3)]
		public int goodsID = 0;

		
		[ProtoMember(4)]
		public int forgeLevel = 0;

		
		[ProtoMember(5)]
		public int quality = 0;

		
		[ProtoMember(6)]
		public int goodsNum = 0;

		
		[ProtoMember(7)]
		public int binding = 0;

		
		[ProtoMember(8)]
		public int site = 0;

		
		[ProtoMember(9)]
		public string jewellist = "";

		
		[ProtoMember(10)]
		public int newHint = 0;

		
		[ProtoMember(11)]
		public string newEndTime = "";

		
		[ProtoMember(12)]
		public int addPropIndex = 0;

		
		[ProtoMember(13)]
		public int bornIndex = 0;

		
		[ProtoMember(14)]
		public int lucky = 0;

		
		[ProtoMember(15)]
		public int strong = 0;

		
		[ProtoMember(16)]
		public int ExcellenceProperty = 0;

		
		[ProtoMember(17)]
		public int nAppendPropLev = 0;

		
		[ProtoMember(18)]
		public int ChangeLifeLevForEquip = 0;

		
		[ProtoMember(19)]
		public int bagIndex = 0;

		
		[ProtoMember(20)]
		public List<int> washProps = null;

		
		[ProtoMember(21)]
		public List<int> ElementhrtsProps;

		
		[ProtoMember(22)]
		public int juHunLevel;

		
		[ProtoMember(23)]
		public int InsureCount;

		
		[ProtoMember(24)]
		public int PackUp;

		
		[ProtoMember(25)]
		public string prop;
	}
}
