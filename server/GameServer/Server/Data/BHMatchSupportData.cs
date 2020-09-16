using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BHMatchSupportData
	{
		
		[ProtoMember(1)]
		public int season;

		
		[ProtoMember(2)]
		public int round;

		
		[ProtoMember(3)]
		public int bhid1;

		
		[ProtoMember(4)]
		public int bhid2;

		
		[ProtoMember(5)]
		public int guess;

		
		[ProtoMember(6)]
		public byte isaward;

		
		[ProtoMember(7)]
		public int rid;
	}
}
