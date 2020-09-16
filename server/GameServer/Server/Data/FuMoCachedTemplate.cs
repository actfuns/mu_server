using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuMoCachedTemplate
	{
		
		[ProtoMember(1)]
		public int Result;

		
		[ProtoMember(2)]
		public int RoleID;

		
		[ProtoMember(3)]
		public int DbID;

		
		[ProtoMember(4)]
		public List<int> AttrTypeValue;
	}
}
