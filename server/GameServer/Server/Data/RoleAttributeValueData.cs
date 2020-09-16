using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleAttributeValueData
	{
		
		[ProtoMember(1)]
		public int RoleAttribyteType;

		
		[ProtoMember(2)]
		public int AddVAlue;

		
		[ProtoMember(3)]
		public int Targetvalue;
	}
}
