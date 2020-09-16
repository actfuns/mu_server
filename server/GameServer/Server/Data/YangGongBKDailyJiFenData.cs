using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YangGongBKDailyJiFenData
	{
		
		[ProtoMember(1)]
		public int DayID = 0;

		
		[ProtoMember(2)]
		public int JiFen = 0;

		
		[ProtoMember(3)]
		public long AwardHistory = 0L;
	}
}
