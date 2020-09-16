using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingDiShouWeiData
	{
		
		[ProtoMember(1)]
		public int State;

		
		[ProtoMember(2)]
		public DateTime FreeBuShuTime;

		
		[ProtoMember(3)]
		public int ZuanShiCost;
	}
}
