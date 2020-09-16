using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleArmorData
	{
		
		[ProtoMember(1)]
		public int Armor;

		
		[ProtoMember(2)]
		public int Exp;
	}
}
