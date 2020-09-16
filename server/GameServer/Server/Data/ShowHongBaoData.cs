using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShowHongBaoData
	{
		
		[ProtoMember(1)]
		public int type;

		
		[ProtoMember(2)]
		public int hongBaoID;

		
		[ProtoMember(3)]
		public string sender;

		
		[ProtoMember(4)]
		public string message;

		
		[ProtoMember(5)]
		public int yiLingNum;

		
		[ProtoMember(6)]
		public int SumHongBaoNum;

		
		[ProtoMember(7)]
		public int result;

		
		[ProtoMember(8)]
		public int tips;
	}
}
