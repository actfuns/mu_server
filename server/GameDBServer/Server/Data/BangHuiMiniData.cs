using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMiniData
	{
		
		[ProtoMember(1)]
		public int BHID = 0;

		
		[ProtoMember(2)]
		public string BHName = "";

		
		[ProtoMember(3)]
		public int ZoneID = 0;
	}
}
