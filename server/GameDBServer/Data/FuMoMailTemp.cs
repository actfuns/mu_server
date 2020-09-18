using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	
	[ProtoContract]
	public class FuMoMailTemp
	{
		
		[ProtoMember(1)]
		public int TodayID = 0;

		
		[ProtoMember(2)]
		public int SenderRID = 0;

		
		[ProtoMember(3)]
		public string ReceiverRID = "";

		
		[ProtoMember(4)]
		public int Accept = 0;

		
		[ProtoMember(5)]
		public int Give = 0;
	}
}
