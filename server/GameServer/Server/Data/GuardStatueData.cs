using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GuardStatueData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int Level = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public int Suit = 1;

		
		[ProtoMember(3, IsRequired = true)]
		public int HasGuardPoint = 0;

		
		[ProtoMember(4, IsRequired = true)]
		public List<GuardSoulData> GuardSoulList = new List<GuardSoulData>();
	}
}
