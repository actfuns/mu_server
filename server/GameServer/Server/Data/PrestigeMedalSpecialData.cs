using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PrestigeMedalSpecialData
	{
		
		[ProtoMember(2)]
		public int SpecialID = 0;

		
		[ProtoMember(1)]
		public int MedalID = 0;

		
		[ProtoMember(3)]
		public double DoubleAttack = 0.0;

		
		[ProtoMember(4)]
		public double DiDouble = 0.0;
	}
}
