using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MarriageData
	{
		
		[ProtoMember(1)]
		public int nSpouseID = -1;

		
		[ProtoMember(2)]
		public sbyte byMarrytype = -1;

		
		[ProtoMember(3)]
		public int nRingID = -1;

		
		[ProtoMember(4)]
		public int nGoodwillexp = 0;

		
		[ProtoMember(5)]
		public sbyte byGoodwillstar = 0;

		
		[ProtoMember(6)]
		public sbyte byGoodwilllevel = 0;

		
		[ProtoMember(7)]
		public int nGivenrose = 0;

		
		[ProtoMember(8)]
		public string strLovemessage = "";

		
		[ProtoMember(9)]
		public sbyte byAutoReject = 0;

		
		[ProtoMember(10)]
		public string ChangTime = "1900-01-01 12:00:00";
	}
}
