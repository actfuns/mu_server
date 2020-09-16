using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	
	[ProtoContract]
	public class ActivityData
	{
		
		[ProtoMember(1)]
		public int ActivityType = 0;

		
		[ProtoMember(2)]
		public bool ActivityIsOpen = false;

		
		[ProtoMember(3)]
		public DateTime TimeBegin = DateTime.MinValue;

		
		[ProtoMember(4)]
		public DateTime TimeEnd = DateTime.MinValue;

		
		[ProtoMember(5)]
		public DateTime TimeAwardBegin = DateTime.MinValue;

		
		[ProtoMember(6)]
		public DateTime TimeAwardEnd = DateTime.MinValue;
	}
}
