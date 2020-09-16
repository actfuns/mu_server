using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuMoMailData
	{
		
		[ProtoMember(1)]
		public int MaillID = 0;

		
		[ProtoMember(2)]
		public int SenderRID = 0;

		
		[ProtoMember(3)]
		public string SenderRName = "";

		
		[ProtoMember(4)]
		public int SenderJob = 0;

		
		[ProtoMember(5)]
		public string SendTime = "";

		
		[ProtoMember(6)]
		public int ReceiverRID = 0;

		
		[ProtoMember(7)]
		public int IsRead = 0;

		
		[ProtoMember(8)]
		public string ReadTime = "1900-01-01 12:00:00";

		
		[ProtoMember(9)]
		public int FuMoMoney = 0;

		
		[ProtoMember(10)]
		public string Content = "";
	}
}
