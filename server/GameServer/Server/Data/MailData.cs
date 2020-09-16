using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MailData
	{
		
		[ProtoMember(1)]
		public int MailID = 0;

		
		[ProtoMember(2)]
		public int SenderRID = 0;

		
		[ProtoMember(3)]
		public string SenderRName = "";

		
		[ProtoMember(4)]
		public string SendTime = "";

		
		[ProtoMember(5)]
		public int ReceiverRID = 0;

		
		[ProtoMember(6)]
		public string ReveiverRName = "";

		
		[ProtoMember(7)]
		public string ReadTime = "1900-01-01 12:00:00";

		
		[ProtoMember(8)]
		public int IsRead = 0;

		
		[ProtoMember(9)]
		public int MailType = 0;

		
		[ProtoMember(10)]
		public int Hasfetchattachment = 0;

		
		[ProtoMember(11)]
		public string Subject = "";

		
		[ProtoMember(12)]
		public string Content = "";

		
		[ProtoMember(13)]
		public int Yinliang = 0;

		
		[ProtoMember(14)]
		public int Tongqian = 0;

		
		[ProtoMember(15)]
		public int YuanBao = 0;

		
		[ProtoMember(16)]
		public List<MailGoodsData> GoodsList = null;
	}
}
