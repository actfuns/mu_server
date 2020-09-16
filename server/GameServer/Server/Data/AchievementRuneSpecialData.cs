using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AchievementRuneSpecialData
	{
		
		[ProtoMember(2)]
		public int SpecialID = 0;

		
		[ProtoMember(1)]
		public int RuneID = 0;

		
		[ProtoMember(3)]
		public double ZhuoYue = 0.0;

		
		[ProtoMember(4)]
		public double DiKang = 0.0;
	}
}
