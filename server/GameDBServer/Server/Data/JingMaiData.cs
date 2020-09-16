using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingMaiData
	{
		
		[ProtoMember(1)]
		public int DbID = 0;

		
		[ProtoMember(2)]
		public int JingMaiID = 0;

		
		[ProtoMember(3)]
		public int JingMaiLevel = 0;

		
		[ProtoMember(4)]
		public int JingMaiBodyLevel = 0;
	}
}
