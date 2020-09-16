using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCPropertyChange
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public int MoneyType;

		
		[ProtoMember(3)]
		public long Value;
	}
}
