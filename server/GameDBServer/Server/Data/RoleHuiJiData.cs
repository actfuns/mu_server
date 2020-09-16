using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleHuiJiData
	{
		
		[ProtoMember(1)]
		public int huiji;

		
		[ProtoMember(2)]
		public int Exp;
	}
}
