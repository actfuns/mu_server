using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LuoLanChengZhanRequestInfoEx
	{
		
		[ProtoMember(1)]
		public int Site = 0;

		
		[ProtoMember(2)]
		public int BHID = 0;

		
		[ProtoMember(3)]
		public int BidMoney = 0;

		
		[ProtoMember(4)]
		public string BHName = "";
	}
}
