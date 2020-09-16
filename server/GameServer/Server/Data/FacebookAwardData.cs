using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FacebookAwardData
	{
		
		[ProtoMember(1)]
		public int AwardID = 0;

		
		[ProtoMember(2)]
		public string AwardName = "";

		
		[ProtoMember(3)]
		public string DbKey = "";

		
		[ProtoMember(4)]
		public int DayMaxNum = 0;

		
		[ProtoMember(5)]
		public int OnlyNum = 0;

		
		[ProtoMember(6)]
		public List<GoodsData> AwardGoods = null;

		
		[ProtoMember(7)]
		public string MailTitle = "";

		
		[ProtoMember(8)]
		public string MailContent = "";

		
		[ProtoMember(9)]
		public int State = 0;

		
		[ProtoMember(10)]
		public int DbID = 0;

		
		[ProtoMember(11)]
		public int RoleID = 0;

		
		[ProtoMember(12)]
		public string MailUser = "";
	}
}
