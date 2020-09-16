using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HorseData
	{
		
		[ProtoMember(1)]
		public int DbID = 0;

		
		[ProtoMember(2)]
		public int HorseID = 0;

		
		[ProtoMember(3)]
		public int BodyID = 0;

		
		[ProtoMember(4)]
		public string PropsNum = "";

		
		[ProtoMember(5)]
		public string PropsVal = "";

		
		[ProtoMember(6)]
		public long AddDateTime = 0L;

		
		[ProtoMember(7)]
		public int JinJieFailedNum = 0;

		
		[ProtoMember(8)]
		public long JinJieTempTime = 0L;

		
		[ProtoMember(9)]
		public int JinJieTempNum = 0;

		
		[ProtoMember(10)]
		public int JinJieFailedDayID = 0;
	}
}
