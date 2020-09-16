using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BHLingDiOwnData
	{
		
		[ProtoMember(1)]
		public int LingDiID = 0;

		
		[ProtoMember(2)]
		public int ZoneID = 0;

		
		[ProtoMember(3)]
		public int BHID = 0;

		
		[ProtoMember(4)]
		public string BHName = "";

		
		[ProtoMember(5)]
		public string BangQiName = "";

		
		[ProtoMember(6)]
		public int BangQiLevel = 0;
	}
}
