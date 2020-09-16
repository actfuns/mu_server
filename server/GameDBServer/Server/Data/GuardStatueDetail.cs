using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GuardStatueDetail
	{
		
		[ProtoMember(1, IsRequired = true)]
		public bool IsActived = false;

		
		[ProtoMember(2, IsRequired = true)]
		public int LastdayRecoverPoint = 0;

		
		[ProtoMember(3, IsRequired = true)]
		public int LastdayRecoverOffset = 0;

		
		[ProtoMember(4, IsRequired = true)]
		public int ActiveSoulSlot = 0;

		
		[ProtoMember(5, IsRequired = true)]
		public GuardStatueData GuardStatue = new GuardStatueData();
	}
}
