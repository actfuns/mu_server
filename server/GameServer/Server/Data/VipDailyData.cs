using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class VipDailyData
	{
		
		[ProtoMember(1)]
		public int PriorityType = 0;

		
		[ProtoMember(2)]
		public int DayID = 0;

		
		[ProtoMember(3)]
		public int UsedTimes = 0;
	}
}
