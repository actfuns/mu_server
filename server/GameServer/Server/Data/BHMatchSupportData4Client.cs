using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BHMatchSupportData4Client
	{
		
		[ProtoMember(1)]
		public int round;

		
		[ProtoMember(2)]
		public int right;

		
		[ProtoMember(3)]
		public int jifen;

		
		public int season;
	}
}
