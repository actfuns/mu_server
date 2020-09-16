using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleBianShenData
	{
		
		[ProtoMember(1)]
		public int BianShen;

		
		[ProtoMember(2)]
		public int Exp;
	}
}
