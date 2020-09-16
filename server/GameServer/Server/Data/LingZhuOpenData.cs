using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingZhuOpenData
	{
		
		[ProtoMember(1)]
		public TimeSpan BeginTime;

		
		[ProtoMember(2)]
		public TimeSpan EndTime;

		
		[ProtoMember(3)]
		public int OpenType;

		
		[ProtoMember(4)]
		public DateTime DoubleOpenEndTime;

		
		[ProtoMember(5)]
		public int LeftCount;
	}
}
