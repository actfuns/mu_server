using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RebornRankInfo
	{
		
		[ProtoMember(1)]
		public int Key = 0;

		
		[ProtoMember(2)]
		public int Value = 0;

		
		[ProtoMember(3)]
		public string Param1 = "";

		
		[ProtoMember(4)]
		public string Param2 = "";

		
		[ProtoMember(5)]
		public int UserPtID = 0;
	}
}
