using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KingRoleGetData
	{
		
		[ProtoMember(1)]
		public int KingType;
	}
}
