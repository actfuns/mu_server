using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BHAttackLog
	{
		
		[ProtoMember(1)]
		public int BHID;

		
		[ProtoMember(2)]
		public string BHName;

		
		[ProtoMember(3)]
		public long BHInjure;

		
		public Dictionary<int, long> RoleInjure;
	}
}
